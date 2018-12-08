local DEFAULT_TEXTURE = "./assets/textures/white.dds"
local MESH_LIST_PANEL_WIDTH = 128
local MESH_LIST_PANEL_HEIGHT = 512

ComponentMesh =
{
	name = "Mesh",
	transform = nil,
	meshIndex = -1,
	meshName = "",
	parent = nil,
	boundingBox = nil,
	worldBox = nil,
}

ComponentMeshWindow =
{
	window = {},
	component = {},
	meshNames = {},
	meshIndices = {},
	meshBoundingBoxes = {},
}

function ComponentMesh.create( parent )
	local result =
	{
		parent = parent,
		meshIndex = -1,
		boundingBox = nil,
		worldBox = nil,
		meshName = "",
	}

	if result.parent then
		result.transform = Transform.create()
		result.transform:setPosition( parent.position )
		result.transform:setOrientation( parent.quatOrientation )
	end

	if #ComponentMeshWindow.meshIndices > 0 then
		result.meshIndex = ComponentMeshWindow.meshIndices[1]
		result.meshName = ComponentMeshWindow.meshNames[1]
		result.boundingBox = ComponentMeshWindow.meshBoundingBoxes[1]
	end
	
	setmetatable( result, { __index = ComponentMesh } )

	result:calculateWorldBox()
	
	return result
end

function ComponentMesh:write( file, level, prefabName )
	local location = ""

	if self.parent then -- entity
		location = self.parent.name .. "_component"
		writeIndent( file, level, "local " .. location .. " = ComponentMesh.create( " .. self.parent.name .. " )\n" )
	else -- prefab
		location = "Prefabs[\"" .. prefabName .. "\"].components[\"" .. self.name .. "\"]"
		writeIndent( file, level, location .. " = ComponentMesh.create()\n" )
	end

	writeIndent( file, level, location .. ":loadMesh( \"" .. self.meshName .. "\" )\n" )

	if self.parent then
		writeIndent( file, level, self.parent.name .. ":addComponent( " .. location .. " )\n" )
	end
end

function ComponentMesh:compile( file, level )
	local position = self.parent.position
	local quatOrientation = self.parent.quatOrientation
	local scale = self.parent.scale

	writeIndent( file, level, "Props:add( {" .. stringVec( position ) .. "}, {" .. stringVec( quatOrientation ) .. "}, {" .. stringVec( scale ) .. "}, \"" .. self.meshName .. "\" )\n" )
end

function ComponentMesh:copy( parent )
	local result = self.create( parent )
	
	result.meshIndex = self.meshIndex
	result.boundingBox = self.boundingBox
	result.meshName = self.meshName
	
	return result
end

function ComponentMesh:loadMesh( path )
	self.meshIndex = Assets.loadMesh( "./assets/models/" .. path )
	self.meshName = path
	if self.meshIndex then
		local mesh = Assets.getMesh( self.meshIndex )
		self.boundingBox = mesh:getBoundingBox()
	else
		self.boundingBox = nil
	end
end

function ComponentMesh:calculateWorldBox()
	if self.parent and self.boundingBox then
		self.worldBox = self.worldBox or {}

		self.worldBox.minPosition = self.boundingBox.minPosition:mul( self.parent.scale )
		self.worldBox.maxPosition = self.boundingBox.maxPosition:mul( self.parent.scale )

		local euler = self.parent.orientation
		local rotationQuat = eulerQuat( {math.rad(-euler[1]), math.rad(-euler[2]), math.rad(-euler[3]) } )
		local rotationMatrix = quatToMat( rotationQuat )

		self.worldBox.minPosition = self.worldBox.minPosition:mulMat( rotationMatrix )
		self.worldBox.maxPosition = self.worldBox.maxPosition:mulMat( rotationMatrix )

		self.worldBox.minPosition = self.worldBox.minPosition:add( self.parent.position )
		self.worldBox.maxPosition = self.worldBox.maxPosition:add( self.parent.position )
	end
end

function ComponentMesh:parentMoved()
	self.transform:setPosition( self.parent.position )
	self:calculateWorldBox()
end

function ComponentMesh:parentOriented()
	self.transform:setOrientation( self.parent.quatOrientation )
	self:calculateWorldBox()
end

function ComponentMesh:parentScaled()
	self.transform:setScale( self.parent.scale )
	self:calculateWorldBox()
end

function ComponentMesh:select( ray )
	local result = -1

	if self.boundingBox then
		--local worldBox =
		--{
		--	minPosition = self.boundingBox.minPosition:mul( self.parent.scale ),
		--	maxPosition = self.boundingBox.maxPosition:mul( self.parent.scale )
		--}
--
		--worldBox.minPosition = worldBox.minPosition:add( self.parent.position )
		--worldBox.maxPosition = worldBox.maxPosition:add( self.parent.position )
		--
		--local hit = {}
		--if Physics.rayAABB( ray, worldBox, hit ) then
		--	result = hit.length
		--end

		local hit = {}
		if Physics.rayAABB( ray, self.worldBox, hit ) then
			result = hit.length
		end
	end
	
	return result
end

function ComponentMesh:update( deltaTime )
end

function ComponentMesh:render()
	if self.meshIndex >= 0 then
		Graphics.queueMesh( self.meshIndex, self.transform )
		
		if self.boundingBox then
			if self.parent.selected or self.parent.hovered then
				local color = Vec4.create({0,1,0,1})
				if self.parent.hovered then
					color[1] = 1
				end

				--local worldBox =
				--{
				--	minPosition = self.boundingBox.minPosition:mul( self.parent.scale ),
				--	maxPosition = self.boundingBox.maxPosition:mul( self.parent.scale )
				--}
--
				--worldBox.minPosition = worldBox.minPosition:add( self.parent.position )
				--worldBox.maxPosition = worldBox.maxPosition:add( self.parent.position )
--
				--DebugShapes.addAABB( worldBox.minPosition, worldBox.maxPosition, color, false )

				DebugShapes.addAABB( self.worldBox.minPosition, self.worldBox.maxPosition, color, false )
			end
		end
	end
	
	return ( self.meshIndex >= 0 )
end

function ComponentMesh:showInfoWindow()
	if ComponentMeshWindow.window.visible then
		ComponentMeshWindow:hide()
	else
		ComponentMeshWindow:show( self )
	end
end

-- WINDOW
function ComponentMeshWindow:show( component )
	self.component = component
	self.window.visible = true
	self.window.focused = true
	if self.window.onFocus then self.window:onFocus() end

	-- update items
	local meshName = "N/A"
	for i=1, #self.meshIndices do
		if self.meshIndices[i] == component.meshIndex then
			meshName = self.meshNames[i]
			break
		end
	end
	self.meshInput.textbox:setText( meshName )
end

function ComponentMeshWindow:hide()
	self.window.visible = false
end

function ComponentMeshWindow:refresh( entity )
	if self.window.visible then
		if entity.components[ComponentMesh.name] then
			self:show( entity.components[ComponentMesh.name] )
		else
			self.window.visible = false
		end
	end
end

function ComponentMeshWindow:load()
	self.window = EditorWindow.create( "Mesh Component" )
	self.window.position[1] = WINDOW_WIDTH - GUI_PANEL_WIDTH - self.window.size[1] - 8
	self.window.position[2] = GUI_MENU_HEIGHT + 8
	self.window.visible = false

	-- load meshes
	self.meshNames = Filesystem.getFiles( "./assets/models/*" )
	if self.meshNames then
		for i=1, #self.meshNames do
			self.meshIndices[i] = Assets.loadMesh( "./assets/models/" .. self.meshNames[i] )

			local mesh = Assets.getMesh( self.meshIndices[i] )
			self.meshBoundingBoxes[i] = mesh:getBoundingBox()
		end
	end

	-- layout
	local layout = EditorLayoutTopdown.create( Vec2.create({0,0}), self.window.size[1] )

	-- mesh name
	local meshInput = EditorInputbox.createWithText( "Mesh:" )
	meshInput.textbox.readOnly = true
	layout:addItem( meshInput )

	-- mesh list
	local meshList = EditorListbox.create( nil, Vec2.create({0, MESH_LIST_PANEL_HEIGHT}) )
	for i=1, #self.meshNames do
		meshList:addItem( self.meshNames[i], i )
	end

	meshList.onItemSelected = function( list, item )
		local index = item.tag

		self.component.meshIndex = self.meshIndices[index]
		self.component.boundingBox = self.meshBoundingBoxes[index]
		self.component.meshName = self.meshNames[index]

		self.meshInput.textbox:setText( self.meshNames[index] )
	end

	layout:addItem( meshList )

	self.window:addItem( layout )

	-- set table references for easy access
	self.meshInput = meshInput
end

function ComponentMeshWindow:update( deltaTime, mousePosition )
	self.window:update( deltaTime, mousePosition )
end

function ComponentMeshWindow:render()
	self.window:render()
end

ComponentMeshWindow:load()

return ComponentMesh, ComponentMeshWindow