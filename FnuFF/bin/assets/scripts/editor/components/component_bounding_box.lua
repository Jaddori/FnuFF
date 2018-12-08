BOUNDING_TYPE_RAY = 1
BOUNDING_TYPE_SPHERE = 2
BOUNDING_TYPE_AABB = 3

ComponentBoundingBox =
{
	name = "Bounding Box",
	parent = nil,
	type = BOUNDING_TYPE_SPHERE,
	color = Vec4.create({1,0,1,1}),
	offset = Vec3.create({0,0,0}),

	-- ray
	ray =
	{
		start = Vec3.create({0,0,0}),
		length = 1.0,
		direction = Vec3.normalize({1,1,1}),
	},

	-- sphere
	sphere =
	{
		center = Vec3.create({0,0,0}),
		radius = 2.0,
	},

	-- aabb
	aabb =
	{
		minPosition = Vec3.create({-2,-2,-2}),
		maxPosition = Vec3.create({2,2,2}),
		minOffset = Vec3.create({-2,-2,-2}),
		maxOffset = Vec3.create({2,2,2}),
	},
}

ComponentBoundingBoxWindow =
{
	window = {},
	component = {},
}

function ComponentBoundingBox.create( parent )
	local result =
	{
		parent = parent,
		type = BOUNDING_TYPE_SPHERE,
		offset = Vec3.create({0,0,0}),

		ray =
		{
			start = Vec3.create({0,0,0}),
			length = 5.0,
			direction = Vec3.normalize({1,1,1}),
		},
		sphere =
		{
			center = Vec3.create({0,0,0}),
			radius = 2.0,
		},
		aabb =
		{
			minPosition = Vec3.create({-2,-2,-2}),
			maxPosition = Vec3.create({2,2,2}),
			minOffset = Vec3.create({-2,-2,-2}),
			maxOffset = Vec3.create({2,2,2}),
		}
	}

	setmetatable( result, { __index = ComponentBoundingBox} )

	if result.parent then
		result:parentMoved()
	end

	return result
end

function ComponentBoundingBox:write( file, level, prefabName )
	local location = ""

	if self.parent then -- entity
		location = self.parent.name .. "_component"

		writeIndent( file, level, "local " .. location .. " = ComponentBoundingBox.create( " .. self.parent.name .. " )\n" )
	else -- prefab
		location = "Prefabs[\"" .. prefabName .. "\"].components[\"" .. self.name .. "\"]"

		writeIndent( file, level, location .. " = ComponentBoundingBox.create()\n" )
	end

	writeIndent( file, level, location .. ".type = " .. tostring( self.type ) .. "\n" )
	if not equalsVec( self.color, ComponentBoundingBox.color ) then
		writeIndent( file, level, location .. ".color = {" .. stringVec( self.color ) .. "}\n" )
	end
	writeIndent( file, level, location .. ".offset = {" .. stringVec( self.offset ) .. "}\n" )

	-- ray
	writeIndent( file, level, location .. ".ray.start = {" .. stringVec( self.ray.start ) .. "}\n" )
	writeIndent( file, level, location .. ".ray.length = " .. tostring( self.ray.length ) .. "\n" )
	writeIndent( file, level, location .. ".ray.direction = {" .. stringVec( self.ray.direction ) .. "}\n" )

	-- sphere
	writeIndent( file, level, location .. ".sphere.center = {" .. stringVec( self.sphere.center ) .. "}\n" )
	writeIndent( file, level, location .. ".sphere.radius = " .. tostring( self.sphere.radius ) .. "\n" )

	-- aabb
	writeIndent( file, level, location .. ".aabb.minPosition = {" .. stringVec( self.aabb.minPosition ) .. "}\n" )
	writeIndent( file, level, location .. ".aabb.maxPosition = {" .. stringVec( self.aabb.maxPosition ) .. "}\n" )
	writeIndent( file, level, location .. ".aabb.minOffset = {" .. stringVec( self.aabb.minOffset ) .. "}\n" )
	writeIndent( file, level, location .. ".aabb.maxOffset = {" .. stringVec( self.aabb.maxOffset ) .. "}\n" )

	if self.parent then
		writeIndent( file, level, self.parent.name .. ":addComponent( " .. location .. " )\n" )
	end
end

function ComponentBoundingBox:compile( file, level )
	writeIndent( file, level, "local boundingBox =\n" )
	writeIndent( file, level, "{\n" )

	level = level + 1
	if self.type == BOUNDING_TYPE_RAY then
		writeIndent( file, level, "start = {" .. stringVec( self.ray.start ) .. "},\n" )
		writeIndent( file, level, "length = " .. tostring( self.ray.length ) .. ",\n" )
		writeIndent( file, level, "direction = {" .. stringVec( self.ray.direction ) .. "}\n" )
	elseif self.type == BOUNDING_TYPE_SPHERE then
		writeIndent( file, level, "center = {" .. stringVec( self.sphere.center ) .. "},\n" )
		writeIndent( file, level, "radius = " .. tostring( self.sphere.radius ) .. "\n" )
	else -- BOUNDING_TYPE_AABB
		writeIndent( file, level, "minPosition = {" .. stringVec( self.aabb.minPosition ) .. "},\n" )
		writeIndent( file, level, "maxPosition = {" .. stringVec( self.aabb.maxPosition ) .. "},\n" )

		local center = Physics.getAABBCenter( self.aabb )
		writeIndent( file, level, "center = {" .. stringVec( center ) .. "},\n" )

		local extents = subVec( center, self.aabb.minPosition )
		writeIndent( file, level, "extents = {" .. stringVec( extents ) .. "},\n" )
	end

	level = level - 1
	writeIndent( file, level, "}\n" )

	if self.type == BOUNDING_TYPE_RAY then
		writeIndent( file, level, "BoundingBoxes:addRay( boundingBox )\n" )
	elseif self.type == BOUNDING_TYPE_SPHERE then
		writeIndent( file, level, "BoundingBoxes:addSphere( boundingBox )\n" )
	else -- BOUNDING_TYPE_AABB
		writeIndent( file, level, "BoundingBoxes:addAABB( boundingBox )\n" )
	end
end

function ComponentBoundingBox:copy( parent )
	local result = self.create( parent )

	--result.type = self.type
	--copyVec( self.color, result.color )
	--result.ray.length = self.ray.length
	--copyVec( self.ray.direction, result.ray.direction )
	--result.sphere.radius = self.sphere.radius
	--copyVec( self.aabb.minPosition, result.aabb.minPosition )
	--copyVec( self.aabb.maxPosition, result.aabb.maxPosition )

	result.type = self.type
	result.color = self.color:copy()
	result.ray.length = self.ray.length
	result.ray.direction = self.ray.direction:copy()
	result.sphere.radius = self.sphere.radius
	result.aabb.minPosition = self.aabb.minPosition:copy()
	result.aabb.maxPosition = self.aabb.maxPosition:copy()

	if self.parent then
		self:parentMoved()
	end

	return result
end

function ComponentBoundingBox:confineOffset()
	local tempMin =
	{
		math.min( self.aabb.minOffset[1], self.aabb.maxOffset[1] ),
		math.min( self.aabb.minOffset[2], self.aabb.maxOffset[2] ),
		math.min( self.aabb.minOffset[3], self.aabb.maxOffset[3] )
	}

	self.aabb.maxOffset[1] = math.max( self.aabb.minOffset[1], self.aabb.maxOffset[1] )
	self.aabb.maxOffset[2] = math.max( self.aabb.minOffset[2], self.aabb.maxOffset[2] )
	self.aabb.maxOffset[3] = math.max( self.aabb.minOffset[3], self.aabb.maxOffset[3] )

	self.aabb.minOffset[1] = tempMin[1]
	self.aabb.minOffset[2] = tempMin[2]
	self.aabb.minOffset[3] = tempMin[3]
end

function ComponentBoundingBox:parentMoved()
	--local center = addVec( self.parent.position, self.offset )
	local center = self.parent.position:add( self.offset )

	--copyVec( center, self.ray.start )
	--copyVec( center, self.sphere.center )

	self.ray.start = center:copy()
	self.sphere.center = center:copy()

	self:confineOffset()

	--self.aabb.minPosition = addVec( center, self.aabb.minOffset )
	--self.aabb.maxPosition = addVec( center, self.aabb.maxOffset )

	self.aabb.minPosition = center:add( self.aabb.minOffset )
	self.aabb.maxPosition = center:add( self.aabb.maxOffset )
end

function ComponentBoundingBox:select( ray )
	local result = -1

	local hit = {}
	-- sphere
	if self.type == BOUNDING_TYPE_SPHERE then
		if Physics.raySphere( ray, self.sphere, hit ) then
			result = hit.length
		end
	-- aabb
	elseif self.type == BOUNDING_TYPE_AABB then
		if Physics.rayAABB( ray, self.aabb, hit ) then
			result = hit.length
		end
	end

	return result
end

function ComponentBoundingBox:update( deltaTime )
end

function ComponentBoundingBox:render()
	local result = true

	local color = self.color
	if self.parent.hovered then
		color = Vec4.create({1,1,0,1})
	end

	-- ray
	if self.type == BOUNDING_TYPE_RAY then
		--local rayEnd =
		--{
		--	self.ray.direction[1] * self.ray.length,
		--	self.ray.direction[2] * self.ray.length,
		--	self.ray.direction[3] * self.ray.length
		--}
		--rayEnd = addVec( self.ray.start, rayEnd )

		local rayEnd = self.ray.direction:mul( self.ray.length )
		rayEnd = self.ray.start:add( rayEnd )

		DebugShapes.addLine( self.ray.start, rayEnd, color )

		result = false -- super hard to select a ray, just show the normal sphere
	-- sphere
	elseif self.type == BOUNDING_TYPE_SPHERE then
		DebugShapes.addSphere( self.sphere.center, self.sphere.radius, color )
	-- aabb
	else
		DebugShapes.addAABB( self.aabb.minPosition, self.aabb.maxPosition, color )
	end

	return result
end

function ComponentBoundingBox:showInfoWindow()
	if ComponentBoundingBoxWindow.window.visible then
		ComponentBoundingBoxWindow:hide()
	else
		ComponentBoundingBoxWindow:show( self )
	end
end

function ComponentBoundingBox:changeType( type )
	if self.type ~= type then
		self.type = type
		self:parentMoved()
	end
end

-- WINDOW
function ComponentBoundingBoxWindow:show( component )
	self.component = component
	self.window.visible = true
	self.window.focused = true
	if self.window.onFocus then self.window:onFocus() end

	-- update items
	local count = #self.window.items
	for i=1, count do
		self.window.items[i] = nil
	end

	-- layout
	local layout = EditorLayoutTopdown.create( Vec2.create({0,0}), self.window.size[1] )

	-- type
	local typeLabel = EditorLabel.createWithText( "Type:" )
	layout:addItem( typeLabel )

	local typeDropdown = EditorDropdown.create( Vec2.create({0,0}), Vec2.create({0, GUI_BUTTON_HEIGHT}) )
	typeDropdown:addItem( "Ray", BOUNDING_TYPE_RAY )
	typeDropdown:addItem( "Sphere", BOUNDING_TYPE_SPHERE )
	typeDropdown:addItem( "AABB", BOUNDING_TYPE_AABB )
	typeDropdown.selectedIndex = self.component.type
	typeDropdown.onItemSelected = function( dropdown, item )
		self.component:changeType( item.tag )
		self:show( self.component )
	end
	layout:addItem( typeDropdown )

	-- ray
	if self.component.type == BOUNDING_TYPE_RAY then
		-- direction
		local directionInputbox = EditorInputbox.createWithText( "Direction:" )
		directionInputbox.textbox:setText( stringVec( self.component.ray.direction ) )
		directionInputbox.textbox.onFinish = function( textbox )
			local dir = normalizeVec( vecString( textbox.text ) )
			self.component.ray.direction = dir
			textbox:setText( stringVec( dir ) )
		end
		layout:addItem( directionInputbox )

		-- length
		local lengthInputbox = EditorInputbox.createWithText( "Length:" )
		lengthInputbox.textbox:setText( self.component.ray.length )
		lengthInputbox.textbox.onFinish = function( textbox )
			self.component.ray.length = tonumber( textbox.text )
		end
		layout:addItem( lengthInputbox )
	-- sphere
	elseif self.component.type == BOUNDING_TYPE_SPHERE then
		-- radius
		local radiusInputbox = EditorInputbox.createWithText( "Radius:" )
		radiusInputbox.textbox:setText( self.component.sphere.radius )
		radiusInputbox.textbox.onFinish = function( textbox )
			self.component.sphere.radius = tonumber( textbox.text )
		end
		layout:addItem( radiusInputbox )
	-- aabb
	else
		-- min position
		local minPositionInputbox = EditorInputbox.createWithText( "Min. position:" )
		minPositionInputbox.textbox:setText( stringVec( self.component.aabb.minOffset ) )
		minPositionInputbox.textbox.onFinish = function( textbox )
			self.component.aabb.minOffset = vecString( textbox.text )
			self.component:parentMoved()
		end
		layout:addItem( minPositionInputbox )

		-- max position
		local maxPositionInputbox = EditorInputbox.createWithText( "Max. position:" )
		maxPositionInputbox.textbox:setText( stringVec( self.component.aabb.maxOffset ) )
		maxPositionInputbox.textbox.onFinish = function( textbox )
			self.component.aabb.maxOffset = vecString( textbox.text )
			self.component:parentMoved()
		end
		layout:addItem( maxPositionInputbox )

		-- match mesh
		local matchButton = EditorButton.createWithText( "Match Mesh" )
		matchButton.onClick = function( button )
			local localBox = self.component.parent.components[ComponentMesh.name].worldBox

			self.component.aabb.minOffset = subVec( localBox.minPosition, self.component.parent.position )
			self.component.aabb.maxOffset = subVec( localBox.maxPosition, self.component.parent.position )

			self.component:parentMoved()
		end

		local meshComponent = self.component.parent.components[ComponentMesh.name]
		matchButton.disabled = ( meshComponent == nil )
		layout:addItem( matchButton )
	end

	self.window:addItem( layout )
end

function ComponentBoundingBoxWindow:hide()
	self.window.visible = false
end

function ComponentBoundingBoxWindow:refresh( entity )
	if self.window.visible then
		if entity.components[ComponentBoundingBox.name] then
			self:show( entity.components[ComponentBoundingBox.name] )
		else
			self.window.visible = false
		end
	end
end

function ComponentBoundingBoxWindow:load()
	-- window	
	self.window = EditorWindow.create( "Bounding Box Component" )
	self.window.position[1] = WINDOW_WIDTH - GUI_PANEL_WIDTH - self.window.size[1] - 8
	self.window.position[2] = GUI_MENU_HEIGHT + 8
	self.window.visible = false

	-- layout
	local layout = EditorLayoutTopdown.create( Vec2.create({0,0}), self.window.size[1] )

	-- type
	local typeLabel = EditorLabel.createWithText( "Type:" )
	layout:addItem( typeLabel )

	local typeDropdown = EditorDropdown.create( Vec2.create({0,0}), Vec2.create({0, GUI_BUTTON_HEIGHT}) )
	typeDropdown:addItem( "Ray", BOUNDING_TYPE_RAY )
	typeDropdown:addItem( "Sphere", BOUNDING_TYPE_SPHERE )
	typeDropdown:addItem( "AABB", BOUNDING_TYPE_AABB )
	typeDropdown.selectedIndex = 2
	typeDropdown.onItemSelected = function( dropdown, item )
		self.component:changeType( item.tag )
		self:show( self.component )
	end
	layout:addItem( typeDropdown )

	self.window:addItem( layout )
end

function ComponentBoundingBoxWindow:update( deltaTime, mousePosition )
	self.window:update( deltaTime, mousePosition )
end

function ComponentBoundingBoxWindow:render()
	self.window:render()
end

ComponentBoundingBoxWindow:load()

return ComponentBoundingBox, ComponentBoundingBoxWindow