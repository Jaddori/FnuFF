MODE_TRANSLATE = 1
MODE_SCALE = 2
MODE_ROTATE = 3

Editor =
{
	name = "Editor",

	camera = {},
	
	gui = nil,	
	entities = {},
	selectedEntity = nil,
	selectedEntityIndex = -1,
	hoveredEntity = nil,
	
	gizmo = nil,
	console = nil,
	grid = nil,
	commandHistory = nil,

	mode = MODE_TRANSLATE,
	command = { old = {}, new = {} },

	selectedPrefab = {},

	priorityQueue = {},

	capture =
	{
		depth = -1,
		button = -1,
		item = nil,
		focusItem = nil,
		entity = nil,
		axis = -1,
	},
}

function Editor:load()
	self.camera = doscript( "editor/editor_camera.lua" )
	self.camera:load()
	
	self.gui = doscript( "editor/gui/editor_gui.lua" )
	self.gui:load()
	self.gui.menu.file.onNew = function() self:reset() end
	self.gui.menu.file.onOpen = function() self:openLevel() end
	self.gui.menu.file.onSave = function() self:saveLevel() end
	self.gui.menu.file.onSaveAs = function() self:saveAsLevel() end
	self.gui.menu.file.onCompile = function() self:compileLevel() end
	self.gui.menu.file.onExit = function() Core.exit() end
	self.gui.menu.settings.onShowGrid = function() self.grid.showGrid = not self.grid.showGrid end
	self.gui.menu.settings.onShowOrigo = function() self.grid.showOrigo = not self.grid.showOrigo end
	self.gui.menu.settings.onEnableLighting = function() Graphics.setLightingEnabled( not Graphics.getLightingEnabled() ) end
	
	self.gizmo = doscript( "editor/editor_gizmo.lua" )
	self.gizmo:load()
	self.gizmo:setPosition( Vec3.create() )
	
	self.console = doscript( "editor/editor_console.lua" )
	self.console:load()
	
	self.grid = doscript( "editor/editor_grid.lua" )

	self.commandHistory = doscript( "editor/editor_command_history.lua" )
	self.commandHistory:load()
	
	doscript( "editor/entity.lua" )
	doscript( "editor/prefab.lua" )

	for _,v in pairs(Entity.windowList) do
		v.window.onFocus = function( window )
			self.gui:focusWindow( window )
		end

		v.window.onClose = function( window )
			self.gui:focusWindow()
		end
	end

	-- gui component list
	self.gui.componentList.onClick = function( button )
		local component = button.tag

		if self.selectedEntity then
			local newComponent = component.create( self.selectedEntity )
			self.selectedEntity:addComponent( newComponent )
			self.gui.panel.tabs[GUI_TAB_INFO]:setEntity( self.selectedEntity )
		end
	end
	
	-- gui context menu
	self.gui.contextMenu:addItem( "New Entity", { index = 1 } )
	self.gui.contextMenu:addItem( "Place Prefab", { index = 2 } )
	self.gui.contextMenu.onClick = function( button )
		-- new entity
		if button.tag.index == 1 then
			local ray = self.camera.camera:createRay()
			local plane = Physics.createPlane( {0,1,0}, 0 )
			
			local hit = {}
			if Physics.rayPlane( ray, plane, hit ) then
				hit.position[2] = 0
				self:createEntity( hit.position )
			end
		-- place prefab
		elseif button.tag.index == 2 then
			local ray = self.camera.camera:createRay()
			local plane = Physics.createPlane( {0,1,0}, 0 )

			local hit = {}
			if Physics.rayPlane( ray, plane, hit ) then
				hit.position[2] = 0
				local entity = self.selectedPrefab:instantiate( hit.position )
				self:addEntity( entity )
			end
		end

		self.gui.contextMenu.visible = false
	end
	
	-- gui info panel
	self.gui.panel.tabs[GUI_TAB_INFO].visibleCheckbox.onCheck = function( checkbox )
		if self.selectedEntity then
			self.selectedEntity.visible = checkbox.checked
		end
	end

	self.gui.panel.tabs[GUI_TAB_INFO].nameInputbox.textbox.onFinish = function( self )
		if self.text:len() > 0 then
			Editor.selectedEntity.name = self.text
		end
	end
	
	self.gui.panel.tabs[GUI_TAB_INFO].positionInputbox.textbox.onFinish = function( textbox )
		if textbox.text:len() > 0 then
			local newPosition = Vec3.create( vecString( textbox.text ) )

			--copyVec( self.selectedEntity.position, self.command.old)
			--copyVec( newPosition, self.command.new )
			self.command.old = self.selectedEntity.position:copy()
			self.command.new = newPosition:copy()

			local moveCommand = CommandMove.create( self.command.old, self.command.new, self.selectedEntity )
			self.commandHistory:addCommand( moveCommand )
			
			self.selectedEntity.position = newPosition
			self.gizmo:setPosition( self.selectedEntity.position )
		end
	end

	self.gui.panel.tabs[GUI_TAB_INFO].orientationInputbox.textbox.onFinish = function( textbox )
		if textbox.text:len() > 0 then
			local newOrientation = Vec3.create( vecString( textbox.text ) )

			for i=1, #newOrientation do
				while newOrientation[i] >= 360 do
					newOrientation[i] = newOrientation[i] - 360
				end

				while newOrientation[i] < 0 do
					newOrientation[i] = newOrientation[i] + 360
				end
			end

			textbox:setText( stringVec( newOrientation ) )

			--copyVec( self.selectedEntity.orientation, self.command.old )
			--copyVec( newOrientation, self.command.new )
			self.command.old = self.selectedEntity.orientation:copy()
			self.command.new = newOrientation:copy()

			local rotateCommand = CommandRotate.create( self.command.old, self.command.new, self.selectedEntity )
			self.commandHistory:addCommand( rotateCommand )

			self.selectedEntity.orientation = newOrientation
			self.gizmo:setOrientation( self.selectedEntity.orientation )
		end
	end

	self.gui.panel.tabs[GUI_TAB_INFO].scaleInputbox.textbox.onFinish = function( textbox )
		if textbox.text:len() > 0 then
			local newScale = Vec3.create( vecString( textbox.text ) )

			--copyVec( self.selectedEntity.scale, self.command.old )
			--copyVec( newScale, self.command.new )
			self.command.old = self.selectedEntity.scale:copy()
			self.command.new = newScale:copy()

			local scaleCommand = CommandScale.create( self.command.old, self.command.new, self.selectedEntity )
			self.commandHistory:addCommand( scaleCommand )

			self.selectedEntity.scale = newScale
			self.gizmo:setScale( self.selectedEntity.scale )
		end
	end

	self.gui.panel.tabs[GUI_TAB_INFO].onCreatePrefab = function()
		self.gui.panel.tabs[GUI_TAB_INFO]:showPrefabNameWindow()

		local window = self.gui.panel.tabs[GUI_TAB_INFO].prefabNameWindow
		self:pushPriorityItem( window )
	end

	self.gui.panel.tabs[GUI_TAB_INFO].onUpdatePrefab = function()
		if self.selectedEntity and self.selectedEntity.prefab then
			self.selectedEntity.prefab:update( self.selectedEntity )
		end
	end

	self.gui.panel.tabs[GUI_TAB_INFO].onRevertToPrefab = function()
		if self.selectedEntity and self.selectedEntity.prefab then
			self.selectedEntity.prefab:revert( self.selectedEntity )
		end
	end

	self.gui.panel.tabs[GUI_TAB_INFO].prefabNameWindow.onConfirm = function( name )
		if self.selectedEntity then
			local prefab = Prefab.create( name, self.selectedEntity )

			self.gui.panel.tabs[GUI_TAB_PREFABS]:addPrefab( prefab )
			self.gui.panel.tabs[GUI_TAB_INFO]:setEntity( self.selectedEntity )

			self.selectedPrefab = prefab
		end
	end

	self.gui.panel.tabs[GUI_TAB_INFO].prefabNameWindow.onClose = function( window )
		self:popPriorityItem()
	end

	self.gui.panel.tabs[GUI_TAB_INFO].onDetachPrefab = function()
		if self.selectedEntity then
			self.selectedEntity.prefab:removeInstance( self.selectedEntity )
			self.selectedEntity.prefab = nil

			self.gui.panel.tabs[GUI_TAB_INFO]:setEntity( self.selectedEntity )
		end
	end

	-- gui entities panel
	self.gui.panel.tabs[GUI_TAB_ENTITIES].onSelect = function( button )
		self:selectEntity( button.tag )
	end

	-- gui prefabs panel
	self.gui.panel.tabs[GUI_TAB_PREFABS].onSelect = function( button )
		self.selectedPrefab = button.tag
	end
end

function Editor:unload()
end

function Editor:update( deltaTime )
	-- only update prioritized items
	if #self.priorityQueue > 0 then
		local mousePosition = Input.getMousePosition()

		if self.capture.button == -1 then
			if Input.buttonPressed( Buttons.Left ) then
				self.capture.button = Buttons.Left
			elseif Input.buttonPressed( Buttons.Right ) then
				self.capture.button = Buttons.Right
			end

			if self.capture.button > -1 then
				self.priorityQueue[#self.priorityQueue]:checkCapture( self.capture, mousePosition )

				if self.capture.item then
					if self.capture.item.press then
						self.capture.item:press( mousePosition )
					end
				end
			end
		else
			if Input.buttonReleased( self.capture.button ) then
				if self.capture.item then
					if self.capture.item.release then
						self.capture.item:release( mousePosition )
					end
				end

				self.capture.depth = -1
				self.capture.item = nil
				self.capture.button = -1
			end
		end

		if self.capture.item then
			if self.capture.item.updateMouseInput then
				self.capture.item:updateMouseInput( deltaTime, mousePosition )
			end
		else
			if #self.priorityQueue > 0 then
				self.priorityQueue[#self.priorityQueue]:update( deltaTime, mousePosition )
			end

			if self.capture.focusItem then
				local stillFocused = self.capture.focusItem:updateKeyboardInput()
				if not stillFocused then
					self.capture.focusItem = nil
				end
			end
		end

		return ------------------------------------------------------------------
	end
	
	self.gizmo:update( deltaTime )
	
	local mousePosition = Input.getMousePosition()
	local guiCaptured = false

	-- update GUI elements
	if not self.capture.entity and self.capture.axis < 0 then
		--Log.debug( "Capture button: " .. tostring( self.capture.button ) )
		if self.capture.button == -1 then
			if Input.buttonPressed( Buttons.Left ) then
				self.capture.button = Buttons.Left
			elseif Input.buttonPressed( Buttons.Right ) then
				self.capture.button = Buttons.Right
			end

			if self.capture.button > -1 then
				local prevFocusItem = self.capture.focusItem

				self.console:checkCapture( self.capture, mousePosition )
				self.gui:checkCapture( self.capture, mousePosition )

				if self.capture.focusItem ~= self.capture.item then
					self.capture.focusItem = nil
				end

				if prevFocusItem and prevFocusItem ~= self.capture.focusItem then
					if prevFocusItem.unsetFocus then
						prevFocusItem:unsetFocus()
					end
				end

				if self.capture.focusItem then
					if self.capture.focusItem.setFocus then
						self.capture.focusItem:setFocus()
					end
				end

				if self.capture.item then
					if self.capture.item.press then
						self.capture.item:press( mousePosition )
					end
				end
			end
		else
			if Input.buttonReleased( self.capture.button ) then
				if self.capture.item then
					if self.capture.item.release then
						self.capture.item:release( mousePosition )
					end

					guiCaptured = true
				end
		
				self.capture.depth = -1
				self.capture.item = nil
				self.capture.button = -1
			end
		end

		if self.capture.item then
			if self.capture.item.updateMouseInput then
				self.capture.item:updateMouseInput( deltaTime, mousePosition )
			end
		else
			if self.capture.focusItem then
				local stillFocused = self.capture.focusItem:updateKeyboardInput()
				if not stillFocused then
					self.capture.focusItem = nil
				end
			end

			self.console:update( deltaTime, mousePosition )
			if self.console.visible then
				self.capture.focusItem = self.console
			end

			self.gui:update( deltaTime, mousePosition )
		end

		if not self.capture.item and not self.capture.focusItem then
			self.camera:update( deltaTime )
		end
	else
		self.capture.button = -1
	end

	-- manipulate entities
	if not self.capture.item and not guiCaptured then
		local ray = self.camera.camera:createRay()

		-- un-hovered the entity that was hovered last frame
		if self.hoveredEntity then
			self.hoveredEntity.hovered = false
		end

		self.hoveredEntity  = self:findEntity( ray )

		-- hover the entity that is hovered this frame
		if self.hoveredEntity then
			self.hoveredEntity.hovered = true
		end

		if self.capture.axis < 0 then
			if Input.buttonPressed( Buttons.Left ) then
				-- check interaction with gizmo
				if self.selectedEntity then
					self.capture.axis = -1

					-- x-axis
					if Physics.rayAABB( ray, self.gizmo.xbounds ) then
						self.capture.axis = 1

						self.xplane = Physics.createPlane( {0,0,1}, self.selectedEntity.position[3] )

						local hit = {}
						if Physics.rayPlane( ray, self.xplane, hit ) then
							self.xoffset = hit.position[1] - self.selectedEntity.position[1]
							self.xscale = hit.position[1] - self.selectedEntity.scale[1]
							self.xrotate = hit.position[1] - self.selectedEntity.orientation[1]
							self.gizmo.selectedAxis = 1

							-- TODO: save commmand
						end
					-- y-axis
					elseif Physics.rayAABB( ray, self.gizmo.ybounds ) then
						self.capture.axis = 2

						local forward = self.camera.camera:getForward()
						local xnormal = Vec3.create({1,0,0})
						local znormal = Vec3.create({0,0,1})

						local xdot = Vec3.dot( xnormal, forward )
						local zdot = Vec3.dot( znormal, forward )

						if xdot < zdot then
							self.yplane = Physics.createPlane( xnormal, self.selectedEntity.position[1] )
						else
							self.yplane = Physics.createPlane( znormal, self.selectedEntity.position[3] )
						end

						local hit = {}
						if Physics.rayPlane( ray, self.yplane, hit ) then
							self.yoffset = hit.position[2] - self.selectedEntity.position[2]
							self.yscale = hit.position[2] - self.selectedEntity.scale[2]
							self.yrotate = hit.position[2] - self.selectedEntity.orientation[2]
							self.gizmo.selectedAxis = 2
							
							-- TODO: save command
						end
					-- z-axis
					elseif Physics.rayAABB( ray, self.gizmo.zbounds ) then
						self.capture.axis = 3

						self.zplane = Physics.createPlane( {1,0,0}, self.selectedEntity.position[1] )

						local hit = {}
						if Physics.rayPlane( ray, self.zplane, hit ) then
							self.zoffset = hit.position[3] - self.selectedEntity.position[3]
							self.zscale = hit.position[3] - self.selectedEntity.scale[3]
							self.zrotate = hit.position[3] - self.selectedEntity.orientation[3]
							self.gizmo.selectedAxis = 3

							-- TODO: save command
						end
					end
				end

				self.capture.entity = self.hoveredEntity

			-- check if an entity was clicked
			elseif Input.buttonReleased( Buttons.Left ) then
				if self.selectedEntity then
					self.selectedEntity.selected = false
				end

				if self.capture.entity and self.capture.entity == self.hoveredEntity then
					self.selectedEntity = self.capture.entity
					self.selectedEntity.selected = true
					self.selectedEntity:refreshInfoWindows()

					self.gizmo:setPosition( self.selectedEntity.position )
					self.gizmo.visible = true
					self.gizmo.selectedAxis = -1
				else
					self.selectedEntity = nil
					self.gizmo.visible = false
				end

				self.gui.panel.tabs[GUI_TAB_INFO]:setEntity( self.selectedEntity )

				self.capture.entity = nil
				self.capture.axis = -1
			end
		else
			if self.selectedEntity then
				local entityMoved = false
				local entityRotated = false
				local entityScaled = false

				if Input.buttonDown( Buttons.Left ) then
					local snap = Input.keyDown( Keys.LeftControl )

					if self.capture.axis == 1 then
						local hit = {}
						if Physics.rayPlane( ray, self.xplane, hit ) then
							if self.mode == MODE_TRANSLATE then
								self.selectedEntity.position[1] = hit.position[1] - self.xoffset

								if snap then
									self.selectedEntity.position[1] = math.floor( self.selectedEntity.position[1] + 0.5 )
								end

								entityMoved = true
							elseif self.mode == MODE_ROTATE then
								local rot = hit.position[1] - self.xrotate

								-- clamp rotation
								while rot >= 360 do
									rot = rot - 360
								end

								while rot < 0 do
									rot = rot + 360
								end

								self.selectedEntity.orientation[1] = rot

								entityRotated = true
							elseif self.mode == MODE_SCALE then
								self.selectedEntity.scale[1]= hit.position[1] - self.xscale

								if snap then
									self.selectedEntity.scale[1] = math.floor( self.selectedEntity.scale[1] + 0.5 )
								end

								entityScaled = true
							end
						end
					elseif self.capture.axis == 2 then
						local hit = {}
						if Physics.rayPlane( ray, self.yplane, hit ) then
							if self.mode == MODE_TRANSLATE then
								self.selectedEntity.position[2] = hit.position[2] - self.yoffset

								if snap then
									self.selectedEntity.position[2] = math.floor( self.selectedEntity.position[2] + 0.5 )
								end

								entityMoved = true
							elseif self.mode == MODE_ROTATE then
								local rot = hit.position[2] - self.yrotate

								-- clamp rotation
								while rot >= 360 do
									rot = rot - 360
								end

								while rot < 0 do
									rot = rot + 360
								end

								self.selectedEntity.orientation[2] = rot

								entityRotated = true
							elseif self.mode == MODE_SCALE then
								self.selectedEntity.scale[2] = hit.position[2] - self.yscale

								if snap then
									self.selectedEntity.scale[2] = math.floor( self.selectedEntity.scale[2] + 0.5 )
								end

								entityScaled = true
							end
						end
					elseif self.capture.axis == 3 then
						local hit = {}
						if Physics.rayPlane( ray, self.zplane, hit ) then
							if self.mode == MODE_TRANSLATE then
								self.selectedEntity.position[3] = hit.position[3] - self.zoffset

								if snap then
									self.selectedEntity.position[3] = math.floor( self.selectedEntity.position[3] + 0.5 )
								end

								entityMoved = true
							elseif self.mode == MODE_ROTATE then
								local rot = hit.position[3] - self.zrotate

								-- clamp rotation
								while rot >= 360 do
									rot = rot - 360
								end

								while rot < 0 do
									rot = rot + 360
								end

								self.selectedEntity.orientation[3] = rot

								entityRotated = true
							elseif self.mode == MODE_SCALE then
								self.selectedEntity.scale[3] = hit.position[3] - self.zscale

								if snap then
									self.selectedEntity.scale[3] = math.floor( self.selectedEntity.scale[3] + 0.5 )
								end

								entityScaled = true
							end
						end
					end
				elseif Input.buttonReleased( Buttons.Left ) then
					self.capture.axis = -1
					self.gizmo.selectedAxis = -1
				end

				-- update gizmo if entity was moved
				if entityMoved then
					self.gizmo:setPosition( self.selectedEntity.position )
					self.gui.panel.tabs[GUI_TAB_INFO]:refresh()
				elseif entityRotated then
					self.gui.panel.tabs[GUI_TAB_INFO]:refresh()
				elseif entityScaled then
					self.gizmo:setScale( self.selectedEntity.scale )
					self.gui.panel.tabs[GUI_TAB_INFO]:refresh()
				end
			end
		end
	end

	if not self.capture.focusItem then
		-- copy current entity
		if self.selectedEntity then
			if Input.keyReleased( Keys.D ) and Input.keyDown( Keys.LeftControl ) then
				local entity = self:copyEntity()

				local command = CommandCopyEntity.create( self, entity )
				self.commandHistory:addCommand( command )
			end
		end

		-- remove current entity
		if self.selectedEntity then
			if Input.keyReleased( Keys.Delete ) then
				local command = CommandRemoveEntity.create( self, self.selectedEntity )
				self.commandHistory:addCommand( command )

				self:removeEntity( self.selectedEntity )
			end
		end

		-- undo/redo
		if Input.keyDown( Keys.LeftControl ) then
			local undoRedo = false
			if Input.keyRepeated( Keys.Z ) then
				self.commandHistory:undo()
				undoRedo = true
			elseif Input.keyRepeated( Keys.Y ) then
				self.commandHistory:redo()
				undoRedo = true
			end

			if self.selectedEntity and undoRedo then
				self.gizmo:setPosition( self.selectedEntity.position )
				self.gui.panel.tabs[GUI_TAB_INFO]:refresh()
			end
		end

		-- select mode
		if Input.keyPressed( Keys.W ) then
			self.mode = MODE_TRANSLATE
		elseif Input.keyPressed( Keys.E ) then
			self.mode = MODE_SCALE
		elseif Input.keyPressed( Keys.R ) then
			self.mode = MODE_ROTATE
		end
	end

	-- entities
	for _,v in pairs(self.entities) do
		v:update( deltaTime )
	end
end

function Editor:render()
	self.gui:render()
	self.gizmo:render()
	self.console:render()
	self.grid:render()
	
	-- entities
	for _,v in pairs(self.entities) do
		v:render()
	end
end

function Editor:findEntity( ray )
	local closestEntity = nil
	local closestDistance = 999999
	for _,v in pairs(self.entities) do
		local distance = v:select( ray )
		if distance > 0 then
			if distance < closestDistance then
				closestDistance = distance
				closestEntity = v
			end
		end
	end

	return closestEntity
end

function Editor:selectEntity( entity )
	if self.selectedEntity then
		self.selectedEntity.selected = false
	end

	self.gui.panel.tabs[GUI_TAB_INFO]:setEntity( entity )
	self.selectedEntity = entity
	entity.selected = true
	
	self.gizmo:setPosition( Editor.selectedEntity.position )
	self.gizmo.visible = true
	self.gizmo.selectedAxis = -1
end

function Editor.onEntitySelected( button )
	local entity = button.tag
	
	if Editor.selectedEntity ~= entity then
		Editor:selectEntity( entity )
	end
end

function Editor:copyEntity()
	local position = self.selectedEntity.position
	local entity = Entity.create( "NewEntity", Vec3.create({position[1]+1, position[2], position[3]+1}) )
	
	for _,v in pairs(self.selectedEntity.components) do
		local component = v:copy( entity )
		entity:addComponent( component )
	end
	
	self.entities[#self.entities+1] = entity
	
	self.gui.panel.tabs[GUI_TAB_ENTITIES]:addEntity( entity, self.onEntitySelected )

	self:selectEntity( entity )

	return entity
end

function Editor:createEntity( position )
	local entity = Entity.create( "NewEntity", position )
	self.entities[#self.entities+1] = entity
	
	self.gui.panel.tabs[GUI_TAB_ENTITIES]:addEntity( entity, self.onEntitySelected )

	self:selectEntity( entity )
end

function Editor:addEntity( entity )
	self.entities[#self.entities+1] = entity

	self:selectEntity( entity )
end

function Editor:removeEntity( entity )
	local index = 0
	for i=1, #self.entities do
		if self.entities[i] == entity then
			index = i
			break
		end
	end

	if index > 0 then
		self.selectedEntity = nil
		self.gui.panel.tabs[GUI_TAB_INFO]:setEntity( self.selectedEntity )
		self.gizmo.visible = false
		self.gui.panel.tabs[GUI_TAB_ENTITIES]:removeEntity( entity )

		self.entities[index] = nil
	end
end

function Editor:reset()
	local count = #self.entities
	for i=1, count do self.entities[i] = nil end

	for k,_ in pairs(Prefabs) do
		Prefabs[k] = nil
	end
	Prefabs = {}

	self.selectedEntity = nil
	self.gizmo.visible = false

	self.gui.panel.tabs[GUI_TAB_ENTITIES]:clear()
	self.gui.panel.tabs[GUI_TAB_PREFABS]:clear()
end

function Editor:openLevel()
	self:reset()

	local filepath = Filesystem.openFileDialog()
	if filepath and filepath:len() > 0 then
		local chunk, error = loadfile( filepath )
		if chunk then
			local status, value = pcall(chunk)
			if status then
				self.entities = value

				for _,v in pairs(self.entities) do
					self.gui.panel.tabs[GUI_TAB_ENTITIES]:addEntity( v, self.onEntitySelected )
				end

				for _,v in pairs(Prefabs) do
					Log.debug( "ADDING PREFAB" )
					self.gui.panel.tabs[GUI_TAB_PREFABS]:addPrefab( v )
				end
			else
				Log.error( "Failed to load entities:" )
				Log.error( value )
			end
		else
			Log.error( "Failed to load level:" )
			Log.error( error )
		end
	end
end

function Editor:saveLevel()
	if self.currentLevelPath then
		local file = io.open( self.currentLevelPath, "w" )
		if file then
			-- write camera position
			local cameraPositionText = "{" .. stringVec( self.camera.camera:getPosition() ) .. "}"
			local cameraDirectionText = "{" .. stringVec( self.camera.camera:getDirection() ) .. "}"
			writeIndent( file, 0, "-- camera\n" )
			writeIndent( file, 0, "Editor.camera.camera:setPosition( " .. cameraPositionText .. " )\n" )
			writeIndent( file, 0, "Editor.camera.camera:setDirection( " .. cameraDirectionText .. " )\n\n" )

			-- write prefabs
			writeIndent( file, 0, "-- prefabs\n" )
			for _,v in pairs(Prefabs) do
				v:write( file )
			end

			writeIndent( file, 0, "\n--entities\n" )

			-- write entities
			writeIndent( file, 0, "local entities = {}\n\n" )

			for _,v in pairs(self.entities) do
				v:write( file )
			end

			writeIndent( file, 0, "return entities\n" )

			file:close()
		end
	else
		self:saveAsLevel()
	end
end

function Editor:saveAsLevel()
	local filepath = Filesystem.saveFileDialog()
	if filepath and filepath:len() > 0 then
		self.currentLevelPath = filepath

		self:saveLevel()
	end
end

function Editor:compileLevel()
	local filepath = Filesystem.saveFileDialog()
	if filepath and filepath:len() > 0 then
		local file = io.open( filepath, "w" )
		if file then
			for _,v in pairs(self.entities) do
				v:compile( file )
			end

			file:close()
		end
	end
end

function Editor:pushPriorityItem( item )
	self.priorityQueue[#self.priorityQueue+1] = item
end

function Editor:popPriorityItem()
	self.priorityQueue[#self.priorityQueue] = nil
end