local DEFAULT_TEXTURE = "./assets/textures/white.dds"

ComponentWalkable =
{
	name = "Walkable",
	size = Vec2.create({20,20}),
	interval = 1,
	parent = nil,
	nodes = {},
}

ComponentWalkableWindow =
{
	window = {},
	component = {},
}

function ComponentWalkable.create( parent )
	local result =
	{
		parent = parent,
		size = Vec2.create({20,20}),
		interval = 1,
		nodes = {},
	}

	setmetatable( result, { __index = ComponentWalkable } )

	return result
end

function ComponentWalkable:calculate()
	local count = #self.nodes
	for i=1, count do self.nodes[i] = nil end

	local startX = self.parent.position[1]
	local endX = self.parent.position[1] + self.size[1]

	local startZ = self.parent.position[3]
	local endZ = self.parent.position[3] + self.size[2]

	local boundingBoxes = {}
	for _,v in pairs(Editor.entities) do
		if v.components[ComponentBoundingBox.name] then
			boundingBoxes[#boundingBoxes+1] = v.components[ComponentBoundingBox.name]
		end
	end

	for x = startX, endX, self.interval do
		for z = startZ, endZ, self.interval do
			local newNode = 
			{
				center = Vec3.create({x, 0, z}),
				radius = self.interval*0.5,
				color = Vec4.create({0,1,0,1}),
				vacant = true,
			}

			local aabb =
			{
				minPosition = Vec3.create({x-self.interval*0.5, -self.interval*0.5, z-self.interval*0.5 }),
				maxPosition = Vec3.create({x+self.interval*0.5, self.interval*0.5, z+self.interval*0.5 })
			}

			for _,box in pairs(boundingBoxes) do
				if box.type == BOUNDING_TYPE_AABB then
					if Physics.aabbAABB( box.aabb, aabb ) then
						newNode.vacant = false
						break
					end
				elseif box.type == BOUNDING_TYPE_SPHERE then
					if Physics.sphereSphere( box.sphere, newNode ) then
						newNode.vacant = false
						break
					end
				end
			end

			if not newNode.vacant then
				newNode.color = Vec4.create({1,0,0,1})
			end
			self.nodes[#self.nodes+1] = newNode
		end
	end
end

function ComponentWalkable:write( file, level, prefabName )
	local location = ""

	if self.parent then -- entity
		location = self.parent.name .. "_component"
		writeIndent( file, level, "local " .. location .. " = ComponentWalkable.create( " .. self.parent.name .. " )\n" )
	else -- prefab
		location = "Prefabs[\"" .. prefabName .. "\"].components[\"" .. self.name .. "\"]"
		writeIndent( file, level, location .. " = ComponentWalkable.create()\n" )
	end

	writeIndent( file, level, location .. ".size = {" .. stringVec( self.size ) .. "}\n" )
	writeIndent( file, level, location .. ".interval = " .. tostring( self.interval ) .. "\n" )

	if self.parent then
		writeIndent( file, level, self.parent.name .. ":addComponent( " .. location .. " )\n" )
	end
end

function ComponentWalkable:compile( file, level )
	if #self.nodes <= 0 then
		self:calculate()
	end

	--[[local name = self.parent.name .. "_component"
	
	writeIndent( file, level, "local " .. name .. " = ComponentWalkable.create( " .. self.parent.name .. " )\n" )
	writeIndent( file, level, name .. ".size = {" .. stringVec( self.size ) .. "}\n" )
	writeIndent( file, level, name .. ".interval = " .. tostring( self.interval ) .. "\n" )--]]
end

function ComponentWalkable:copy( parent )
	local result = self.create( parent )

	copyVec( self.size, result.size )
	result.interval = self.interval

	return result
end

function ComponentWalkable:select( ray )
	local result = -1

	local minPosition = Vec3.create({self.parent.position[1], -0.5, self.parent.position[3]})
	local maxPosition = Vec3.create({self.parent.position[1]+self.size[1], 0.5, self.parent.position[3]+self.size[2]})
	local boundingBox = Physics.createAABB( minPosition, maxPosition )

	local hit = {}
	if Physics.rayAABB( ray, boundingBox, hit ) then
		result = hit.length
	end

	return result
end

function ComponentWalkable:update( deltaTime )
end

function ComponentWalkable:render()
	-- render bounds
	local color = Vec4.create({0,1,0,1})
	if self.parent.hovered then
		color[1] = 1
	end
	local minPosition = Vec3.create({self.parent.position[1], -0.5, self.parent.position[3]})
	local maxPosition = Vec3.create({self.parent.position[1]+self.size[1], 0.5, self.parent.position[3]+self.size[2]})

	DebugShapes.addAABB( minPosition, maxPosition, color )

	if self.parent.selected then
		-- render nodes
		if self.nodes then
			for _,v in pairs(self.nodes) do
				DebugShapes.addSphere( v.center, v.radius, v.color )
			end
		end
	end

	return true
end

function ComponentWalkable:showInfoWindow()
	if ComponentWalkableWindow.window.visible then
		ComponentWalkableWindow:hide()
	else
		ComponentWalkableWindow:show( self )
	end
end

-- WINDOW
function ComponentWalkableWindow:show( component )
	self.component = component
	self.window.visible = true

	-- update items
	self.sizeInput.textbox:setText( stringVec( component.size ) )
	self.intervalInput.textbox:setText( component.interval )
end

function ComponentWalkableWindow:hide()
	self.window.visible = false
end

function ComponentWalkableWindow:refresh( entity )
	if self.window.visible then
		if entity.components[ComponentWalkable.name] then
			self:show( entity.components[ComponentWalkable.name] )
		else
			self.window.visible = false
		end
	end
end

function ComponentWalkableWindow:load()
	-- window
	self.window = EditorWindow.create( "Walkable Component" )
	self.window.position[1] = WINDOW_WIDTH - GUI_PANEL_WIDTH - self.window.size[1] - 8
	self.window.position[2] = GUI_MENU_HEIGHT + 8
	self.window.visible = false
	self.window.focused = true
	if self.window.onFocus then self.window:onFocus() end

	-- layout
	local layout = EditorLayoutTopdown.create( Vec2.create({0,0}), self.window.size[1] )

	-- size
	local sizeInput = EditorInputbox.createWithText( "Size:" )
	sizeInput.textbox.onFinish = function( textbox )
		self.component.size = vecString( textbox.text )
	end
	layout:addItem( sizeInput )
	
	-- interval
	local intervalInput = EditorInputbox.createWithText( "Interval:" )
	intervalInput.textbox.onFinish = function( textbox )
		self.component.interval = tonumber( textbox.text )
	end
	layout:addItem( intervalInput )

	-- calculate
	local calculateButton = EditorButton.createWithText( "Calculate" )
	calculateButton.onClick = function( button )
		self.component:calculate()
	end
	layout:addItem( calculateButton )

	self.window:addItem( layout )

	-- set table references for easy access
	self.sizeInput = sizeInput
	self.intervalInput = intervalInput
end

function ComponentWalkableWindow:update( deltaTime, mousePosition )
	self.window:update( deltaTime, mousePosition )
end

function ComponentWalkableWindow:render()
	self.window:render()
end

ComponentWalkableWindow:load()

return ComponentWalkable, ComponentWalkableWindow