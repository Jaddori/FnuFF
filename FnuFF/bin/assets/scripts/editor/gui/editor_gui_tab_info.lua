local GUI_DEFAULT_CROSS_TEXTURE = "./assets/textures/cross.dds"
local PREFAB_BUTTON_CREATE = 1
local PREFAB_BUTTON_UPDATE = 2

local info =
{
	items = {},
	subItems = {},
	entity = nil,
	depth = 0,
	crossTextureIndex = -1,

	position = Vec2.create({0,0}),
	size = Vec2.create({0,0}),
	
	layout = {},
	subLayout = {},
	nameInputbox = {},
	prefabInputbox = {},
	prefabDetachButton = {},
	positionInputbox = {},
	orientationInputbox = {},
	scaleInputbox = {},
	visibleLabel = {},
	visibleCheckbox = {},
	createPrefabButton = {},
	revertPrefabButton = {},
	componentsLabel = {},
	addComponentButton = {},

	prefabNameWindow = {},

	onCreatePrefab = nil,
	onUpdatePrefab = nil,
	onRevertToPrefab = nil,
	onAddComponent = nil,
	onDetachPrefab = nil,
}

function info:load( position, size, depth )
	self.position = position:copy()
	self.size = size:copy()

	local padding = 4
	local yoffset = 0

	self.depth = depth + GUI_DEPTH_INC

	self.crossTextureIndex = Assets.loadTexture( GUI_DEFAULT_CROSS_TEXTURE )

	-- layout
	self.layout = EditorLayoutTopdown.create( position, size[1] )

	-- name
	self.nameInputbox = EditorInputbox.createWithText( "Name:" )
	self.nameInputbox:setDepth( self.depth )
	self.layout:addItem( self.nameInputbox )
	yoffset = yoffset + self.nameInputbox.size[2] + padding

	-- prefab name
	local detachBounds = GUI_BUTTON_HEIGHT
	self.prefabInputbox = EditorInputbox.createWithText( "Prefab:" )
	self.prefabInputbox.textbox.readOnly = true
	self.prefabInputbox:setDepth( self.depth )
	yoffset = yoffset + self.prefabInputbox.size[2] + padding
	
	-- prefab detach button
	local prefabPosition = self.prefabInputbox.textbox.position
	local prefabSize = self.prefabInputbox.textbox.size
	local prefabLabelHeight = self.prefabInputbox.label.size[2]
	local detachSize = 16
	self.prefabDetachButton = EditorButton.create( Vec2.create({0, prefabLabelHeight + (detachBounds-detachSize)*0.5}), Vec2.create({detachSize, detachSize}), "" )
	self.prefabDetachButton.disabled = true
	self.prefabDetachButton.textureIndex = Assets.loadTexture( GUI_DEFAULT_CROSS_TEXTURE )
	self.prefabDetachButton.color = Vec4.create({1,1,1,1})
	self.prefabDetachButton.hoverColor = Vec4.create({1.0, 0.35, 0.35, 1.0})
	self.prefabDetachButton:setDepth( self.depth )
	self.prefabDetachButton.onClick = function( button )
		if self.onDetachPrefab then
			self.onDetachPrefab()
		end
	end

	self.layout:addItem( {self.prefabInputbox, self.prefabDetachButton} )

	-- position
	self.positionInputbox = EditorInputbox.createWithText( "Position:" )
	self.positionInputbox:setDepth( self.depth )
	self.layout:addItem( self.positionInputbox )
	yoffset = yoffset + self.positionInputbox.size[2] + padding

	-- orientation
	self.orientationInputbox = EditorInputbox.createWithText( "Orientation:" )
	self.orientationInputbox:setDepth( self.depth )
	
	self.layout:addItem( self.orientationInputbox )
	yoffset = yoffset + self.orientationInputbox.size[2] + padding

	-- scale
	self.scaleInputbox = EditorInputbox.createWithText( "Scale:" )
	self.scaleInputbox:setDepth( self.depth )
	self.layout:addItem( self.scaleInputbox )
	yoffset = yoffset + self.scaleInputbox.size[2] + padding

	-- visible
	self.visibleCheckbox = EditorCheckbox.createWithText( "Visible" )
	self.visibleCheckbox:setDepth( self.depth )
	
	self.layout:addItem( self.visibleCheckbox )
	yoffset = yoffset + self.visibleCheckbox.size[2] + padding

	-- create prefab
	self.createPrefabButton = EditorButton.createWithText( "Create Prefab" )
	self.createPrefabButton.disabled = true
	self.createPrefabButton.mode = PREFAB_BUTTON_CREATE
	self.createPrefabButton:setDepth( self.depth )
	self.createPrefabButton.onClick = function( button )
		-- create
		if button.mode == PREFAB_BUTTON_CREATE then
			if self.onCreatePrefab then
				self.onCreatePrefab()
			end
		-- update
		elseif button.mode == PREFAB_BUTTON_UPDATE then
			if self.onUpdatePrefab then
				self.onUpdatePrefab()
			end
		end
	end
	
	self.layout:addItem( self.createPrefabButton )
	yoffset = yoffset + self.createPrefabButton.size[2] + padding

	-- revert to prefab
	self.revertPrefabButton = EditorButton.createWithText( "Revert to Prefab" )
	self.revertPrefabButton.disabled = true
	self.revertPrefabButton:setDepth( self.depth )
	self.revertPrefabButton.onClick = function( button )
		if self.onRevertToPrefab then
			self.onRevertToPrefab()
		end
	end
	
	self.layout:addItem( self.revertPrefabButton )
	yoffset = yoffset + self.revertPrefabButton.size[2] + padding

	-- prefab name window
	self.prefabNameWindow = EditorWindow.create( "Create Prefab" )
	self.prefabNameWindow.visible = false

	local prefabNameInputbox = EditorInputbox.create( nil, nil, "Name:" )
	prefabNameInputbox.textbox.onTextChanged = function( textbox )
		local len = textbox.text:len()
		self.prefabNameWindow.items[2].disabled = not (len > 0)
	end
	self.prefabNameWindow:addItem( prefabNameInputbox )

	local prefabNameCreateButton = EditorButton.create( nil, Vec2.create({0, GUI_BUTTON_HEIGHT}), "Create" )
	prefabNameCreateButton.onClick = function( button )
		self.prefabNameWindow:close()

		if self.prefabNameWindow.onConfirm then
			self.prefabNameWindow.onConfirm( self.prefabNameWindow.items[1].textbox.text )
		end
	end
	self.prefabNameWindow:addItem( prefabNameCreateButton )

	-- components
	self.componentsLabel = EditorLabel.createWithText( "Components:" )
	self.componentsLabel:setDepth( self.depth )
	
	self.layout:addItem( self.componentsLabel )
	yoffset = yoffset + self.componentsLabel.size[2] + padding

	-- add component
	self.addComponentButton = EditorButton.createWithText( "Add Component" )
	self.addComponentButton.disabled = true
	self.addComponentButton:setDepth( self.depth )
	self.addComponentButton.onClick = function( button )
		if self.onAddComponent then
			self.onAddComponent()
		end
	end

	self.layout:addItem( self.addComponentButton )
	yoffset = yoffset + GUI_BUTTON_HEIGHT + padding

	-- create sub layout
	self.subLayout = EditorLayoutTopdown.create( Vec2.create({position[1], position[2] + self.layout.size[2]}), size[1] )
end

function info:showPrefabNameWindow()
	local s = self.prefabNameWindow.size

	local view = { WINDOW_WIDTH - GUI_PANEL_WIDTH, WINDOW_HEIGHT - GUI_MENU_HEIGHT }

	local x = view[1]*0.5 - s[1]*0.5
	local y = view[2]*0.5 - s[2]*0.5

	self.prefabNameWindow.items[1].textbox:setText("")
	self.prefabNameWindow.items[2].disabled = true

	self.prefabNameWindow:setPosition( Vec2.create({x,y}) )
	self.prefabNameWindow.visible = true
end

function info:setEntity( entity )
	-- clear items
	self.subLayout:clear()
	
	if entity then
		-- set name and position
		self.nameInputbox.textbox:setText( entity.name )
		if entity.prefab then
			self.prefabInputbox.textbox:setText( entity.prefab.name )
			self.prefabDetachButton.disabled = false
			self.revertPrefabButton.disabled = false

			self.createPrefabButton:setText( "Update Prefab" )
			self.createPrefabButton.mode = PREFAB_BUTTON_UPDATE
		else
			self.prefabInputbox.textbox:setText( "" )
			self.prefabDetachButton.disabled = true
			self.revertPrefabButton.disabled = true

			self.createPrefabButton:setText( "Create Prefab" )
			self.createPrefabButton.mode = PREFAB_BUTTON_CREATE
		end
		self.positionInputbox.textbox:setText( stringVec( entity.position ) )
		self.orientationInputbox.textbox:setText( stringVec( entity.orientation ) )
		self.scaleInputbox.textbox:setText( stringVec( entity.scale ) )
		self.visibleCheckbox.checked = entity.visible

		-- create new items
		self.entity = entity
		
		local padding = 4
		local yoffset = self.addComponentButton.position[2] + self.addComponentButton.size[2] + 8
		local removeSize = 16
		for _,v in pairs(self.entity.components) do
			local button = EditorButton.createWithText( v.name )
			button:setDepth( self.depth )
			button:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
			button.onClick = function( button )
				v:showInfoWindow()
			end

			local removePadding = ( button.size[2] - removeSize ) * 0.5
			local removeButton = EditorButton.create( Vec2.create({0, removePadding}), Vec2.create({removeSize, removeSize}), "" )
			removeButton:setDepth( self.depth )
			removeButton.textureIndex = self.crossTextureIndex
			removeButton.color = Vec4.create({1,1,1,1})
			removeButton.hoverColor = Vec4.create({1.0, 0.35, 0.35, 1.0})
			removeButton.onClick = function( button )
				self.entity.components[v.name] = nil
				self:setEntity( self.entity )
			end

			self.subLayout:addItem( Vec2.create({button, removeButton}) )

			yoffset = yoffset + GUI_BUTTON_HEIGHT + padding
		end

		self.addComponentButton.disabled = false
		self.createPrefabButton.disabled = false
	else
		self.nameInputbox.textbox:setText( "" )
		self.prefabInputbox.textbox:setText( "" )
		self.prefabDetachButton.disabled = true
		self.positionInputbox.textbox:setText( "" )
		self.orientationInputbox.textbox:setText( "" )
		self.scaleInputbox.textbox:setText( "" )
		self.addComponentButton.disabled = true
		self.visibleCheckbox.checked = false

		self.createPrefabButton.disabled = true
		self.createPrefabButton:setText( "Create Prefab" )
		self.createPrefabButton.mode = PREFAB_BUTTON_CREATE
		self.revertPrefabButton.disabled = true
	end
end

function info:refresh()
	self.positionInputbox.textbox:setText( stringVec( self.entity.position ) )
	self.orientationInputbox.textbox:setText( stringVec( self.entity.orientation ) )
	self.scaleInputbox.textbox:setText( stringVec( self.entity.scale ) )
end

function info:checkCapture( capture, mousePosition )
	-- check items
	self.layout:call( "checkCapture", capture, mousePosition )

	-- check sub items
	self.subLayout:call( "checkCapture", capture, mousePosition )

	-- check prefab name window
	self.prefabNameWindow:checkCapture( capture, mousePosition )

	if capture.depth < self.depth then
		if insideRect( self.position, self.size, mousePosition ) then
			capture.depth = self.depth
			capture.item = self
		end
	end
end

function info:update( deltaTime, mousePosition )
	-- update items
	self.layout:call( "update", deltaTime, mousePosition )

	-- update sub items
	self.subLayout:call( "update", deltaTime, mousePosition )

	-- update prefab name window
	self.prefabNameWindow:update( deltaTime, mousePosition )
end

function info:render()
	-- render items
	self.layout:call( "render" )

	-- render sub items
	self.subLayout:call( "render" )

	-- render prefab name window
	self.prefabNameWindow:render()
end

return info