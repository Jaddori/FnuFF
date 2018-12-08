ComponentDirectionalLight =
{
	name = "Directional Light",
	parent = nil,
	direction = Vec3.create({1,0,0}),
	color = Vec3.create({1,1,1}),
	intensity = 2,
}

ComponentDirectionalLightWindow =
{
	window = {},
	component = {},
}

function ComponentDirectionalLight.create( parent )
	local result =
	{
		parent = parent,
		direction = Vec3.create({1,0,0}),
		color = Vec3.create({1,1,1}),
		intensity = 2,
	}

	setmetatable( result, { __index = ComponentDirectionalLight } )

	return result
end

function ComponentDirectionalLight:write( file, level, prefabName )
	local location = ""

	if self.parent then -- entity
		location = self.parent.name .. "_component"
		writeIndent( file, level, "local " .. location .. " = ComponentDirectionalLight.create( " .. self.parent.name .. " )\n" )
	else -- prefab
		location = "Prefabs[\"" .. prefabName .. "\"].components[\"" .. self.name .. "\"]"
		writeIndent( file, level, location .. " = ComponentDirectionalLight.create()\n" )
	end

	writeIndent( file, level, location .. ".direction = {" .. stringVec( self.direction ) .. "}\n" )
	writeIndent( file, level, location .. ".color = {" .. stringVec( self.color ) .. "}\n" )
	writeIndent( file, level, location .. ".intensity = " .. tostring( self.intensity ) .. "\n" )

	if self.parent then
		writeIndent( file, level, self.parent.name .. ":addComponent( " .. location .. " )\n" )
	end
end

function ComponentDirectionalLight:compile( file, level )
	writeIndent( file, level, "local light =\n" )
	writeIndent( file, level, "{\n" )

	level = level + 1
	writeIndent( file, level, "direction = {" .. stringVec( self.direction ) .. "},\n" )
	writeIndent( file, level, "color = {" .. stringVec( self.color ) .. "},\n" )
	writeIndent( file, level, "intensity = " .. tostring( self.intensity ) .. "\n" )

	level = level - 1
	writeIndent( file, level, "}\n" )

	writeIndent( file, level, "Lights:addDirectionalLight( light )\n" )
end

function ComponentDirectionalLight:copy( parent )
	local result = self.create( parent )

	--copyVec( self.direction, result.direction )
	--copyVec( self.color, result.color )
	result.direction = self.direction:copy()
	result.color = self.color:copy()
	result.intensity = self.intensity

	return result
end

function ComponentDirectionalLight:select( ray )
	return -1
end

function ComponentDirectionalLight:update( deltaTime )
end

function ComponentDirectionalLight:render()
	Graphics.queueDirectionalLight( self.direction, self.color, self.intensity )

	local color = Vec4.create({0,0,0,0})
	--copyVec( self.color, color )
	color[1] = self.color[1]
	color[2] = self.color[2]
	color[3] = self.color[3]
	color[4] = 1

	for i=1, 4 do
		--local rayStart =
		--{
		--	self.parent.position[1] + self.direction[1] * i,
		--	self.parent.position[2] + self.direction[2] * i,
		--	self.parent.position[3] + self.direction[3] * i,
		--}
		--local rayEnd =
		--{
		--	rayStart[1] + self.direction[1]*0.5,
		--	rayStart[2] + self.direction[2]*0.5,
		--	rayStart[3] + self.direction[3]*0.5
		--}

		local rayStart = self.parent.position:add( self.direction:mul( i ) )
		local rayEnd = rayStart:add( self.direction:mul( 0.5 ) )
		
		DebugShapes.addLine( rayStart, rayEnd, color )
	end

	return false
end

function ComponentDirectionalLight:showInfoWindow()
	if ComponentDirectionalLightWindow.window.visible then
		ComponentDirectionalLightWindow:hide()
	else
		ComponentDirectionalLightWindow:show( self )
	end
end

-- WINDOW
function ComponentDirectionalLightWindow:show( component )
	self.component = component
	self.window.visible = true
	self.window.focused = true
	if self.window.onFocus then self.window:onFocus() end

	-- update items
	self.directionInputbox.textbox:setText( stringVec( component.direction ) )
	self.colorInputbox.textbox:setText( stringVec( component.color ) )
	self.intensityInputbox.textbox:setText( component.intensity )
end

function ComponentDirectionalLightWindow:hide()
	self.window.visible = false
end

function ComponentDirectionalLightWindow:refresh( entity )
	if self.window.visible then
		if entity.components[ComponentDirectionalLight.name] then
			self:show( entity.components[ComponentDirectionalLight.name] )
		else
			self.window.visible = false
		end
	end
end

function ComponentDirectionalLightWindow:load()
	self.window = EditorWindow.create( "Directional Light component" )
	self.window.position[1] = WINDOW_WIDTH - GUI_PANEL_WIDTH - self.window.size[1] - 8
	self.window.position[2] = GUI_MENU_HEIGHT + 8
	self.window.visible = false

	-- layout
	local layout = EditorLayoutTopdown.create( Vec2.create({0,0}), self.window.size[1] )

	-- direction
	local directionInputbox = EditorInputbox.createWithText( "Direction:" )
	directionInputbox.textbox.onFinish = function( textbox )
		self.component.direction = vecString( textbox.text )
	end
	layout:addItem( directionInputbox )

	-- color
	local colorInputbox = EditorInputbox.createWithText( "Color:" )
	colorInputbox.textbox.onFinish = function( textbox )
		self.component.color = vecString( textbox.text )
	end
	layout:addItem( colorInputbox )

	-- intensity
	local intensityInputbox =EditorInputbox.createWithText( "Intensity:" )
	intensityInputbox.textbox.onFinish = function( textbox )
		self.component.intensity = tonumber( textbox.text )
	end
	layout:addItem( intensityInputbox )

	self.window:addItem( layout )

	-- set table references for easy access
	self.directionInputbox = directionInputbox
	self.colorInputbox = colorInputbox
	self.intensityInputbox = intensityInputbox
end

function ComponentDirectionalLightWindow:update( deltaTime, mousePosition )
	self.window:update( deltaTime, mousePosition )
end

function ComponentDirectionalLightWindow:render()
	self.window:render()
end

ComponentDirectionalLightWindow:load()

return ComponentDirectionalLight, ComponentDirectionalLightWindow