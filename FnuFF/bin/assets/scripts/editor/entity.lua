Entity = 
{
	visible = true,
	name = "",
	components = {},
	selected = false,
	hovered = false,
	
	position = Vec3.create({0,0,0}),
	prevPosition = Vec3.create({0,0,0}),
	
	orientation = Vec3.create({0,0,0}),
	prevOrientation = Vec3.create({0,0,0}),

	quatOrientation = Vec3.create({0,0,0,1}),
	
	scale = Vec3.create({1,1,1}),
	prevScale = Vec3.create({1,1,1}),

	componentList = {},
	windowList = {},

	prefab = nil,
}

function Entity.create( name, position, orientation, scale)
	local result =
	{
		position = position and position:copy() or Vec3.create({0,0,0}),
		prevPosition = Vec3.create({0,0,0}),
		
		orientation = orientation and orientation:copy() or Vec3.create({0,0,0}),
		prevOrientation = Vec3.create({0,0,0}),
		
		scale = scale and scale:copy() or Vec3.create({1,1,1}),
		prevScale = Vec3.create({1,1,1}),
	
		visible = true,
		name = name,
		selected = false,
		components = {},
	}

	result.quatOrientation = eulerQuat( result.orientation )
	
	setmetatable( result, { __index = Entity } )
	
	return result
end

function Entity:addComponent( component )
	if self.components[component.name] then
		Log.debug( "Entity already has a \"" .. component.name .. "\" component." )
	else
		self.components[component.name] = component
	end
end

function Entity:removeComponent( name )
	self.components[name] = nil
end

function Entity:clearComponents()
	for k,_ in pairs(self.components) do
		self.components[k] = nil
	end
end

function Entity:refreshInfoWindows()
	for _,v in pairs(Entity.windowList) do
		v:refresh( self )
	end
end

function Entity:write( file, level )
	level = level or 0
	
	writeIndent( file, level, "-- " .. self.name .. "\n" )

	writeIndent( file, level, "local " .. self.name .. " = Entity.create( \"" .. self.name .. "\", {" .. stringVec( self.position ) .. "}, {" .. stringVec( self.orientation ) .. "}, {" .. stringVec( self.scale ) .. "} )\n" )
	writeIndent( file, level, self.name .. ".visible = " .. tostring( self.visible ) .. "\n" )

	if self.prefab then
		writeIndent( file, level, self.name .. ".prefab = Prefabs[\"" .. self.prefab.name .. "\"]\n" )
		writeIndent( file, level, "Prefabs[\"" .. self.prefab.name .. "\"].instances[#Prefabs[\"" .. self.prefab.name .. "\"].instances+1] = " .. self.name .. "\n" )
	end

	for _,v in pairs(self.components) do
		v:write( file, level )
	end

	writeIndent( file, level, "local " .. self.name .. "_component = nil\n" )
	writeIndent( file, level, "entities[#entities+1] = " .. self.name .. "\n" )

	writeIndent( file, level, "-- " .. self.name .. "\n\n" )
end

function Entity:read( file )
	
end

function Entity:compile( file, level )
	level = level or 0

	writeIndent( file, level, "-- " .. self.name .. "\n" )
	for _,v in pairs(self.components) do
		v:compile( file, level )
		writeIndent( file, level, "\n" )
	end
end

function Entity:select( ray )
	local result = -1
	
	for _,v in pairs(self.components) do
		local distance = v:select( ray )
		if distance > 0 and ( distance < result or result < 0 ) then
			result = distance
		end
	end
	
	if result < 0 then
		local sphere = Physics.createSphere( self.position, 1.0 )

		local hit = {}
		if Physics.raySphere( ray, sphere, hit ) then
			result = hit.length
		end
	end
	
	return result
end

function Entity:executeOnComponents( functionName )
	for _,v in pairs(self.components) do
		if v[functionName] then
			v[functionName]( v )
		end
	end
end

function Entity:update( deltaTime )
	-- check if position has changed
	if not equalsVec( self.position, self.prevPosition ) then
		self:executeOnComponents( "parentMoved" )
		copyVec( self.position, self.prevPosition )
	end
	
	-- check if orientation has changed
	if not equalsVec( self.orientation, self.prevOrientation ) then
		self.quatOrientation = eulerQuat
		({
			math.rad( self.orientation[1] ),
			math.rad( self.orientation[2] ),
			math.rad( self.orientation[3] )
		})
		self:executeOnComponents( "parentOriented" )
		copyVec( self.orientation, self.prevOrientation )
	end
	
	-- check if scale has changed
	if not equalsVec( self.scale, self.prevScale ) then
		self:executeOnComponents( "parentScaled" )
		copyVec( self.scale, self.prevScale )
	end

	-- update components
	for _,v in pairs(self.components) do
		v:update( deltaTime )
	end
end

function Entity:render()
	local rendered = false

	if self.visible then
		for _,v in pairs(self.components) do
			if v:render() then
				rendered = true
			end
		end
	end

	if not rendered or self.hovered or self.selected then
		local color = Vec4.create({0,1,0,1})
		if self.hovered then
			color[1] = 1
		end

		if not self.visible then
			color[4] = 0.25
		end

		DebugShapes.addSphere( self.position, 1.0, color )
	end
end

local componentScripts = Filesystem.getFiles( "./assets/scripts/editor/components/*" )
for _,v in pairs(componentScripts) do
	local component, window = doscript( "editor/components/" .. v )

	Entity.componentList[#Entity.componentList+1] = component
	Entity.windowList[#Entity.windowList+1] = window

	Editor.gui.componentList:addItem( component.name, component )
end