Prefab =
{
	name = "Prefab",
	components = {},
	instances = {},
}

Prefabs = {}

function Prefab.create( name, entity )
	assert( name, "Prefab must have a name." )
	assert( name:len() > 0, "Prefab name must not be empty." )
	assert( entity, "Prefab must be created with a template entity." )
	assert( entity.components, "Template entity must have components." )
	assert( Prefabs[name] == nil, "Prefab name must be unique" )

	local result =
	{
		name = name,
		components = {},
		instances = { entity },
	}

	for k,v in pairs(entity.components) do
		result.components[k] = v:copy()
	end

	setmetatable( result, { __index = Prefab } )

	Prefabs[name] = result
	entity.prefab = result

	return result
end

function Prefab:write( file, level )
	level = level or 0

	writeIndent( file, level, "Prefabs[\"" .. self.name .. "\"] =\n" )
	writeIndent( file, level, "{\n" )

	level = level + 1
	writeIndent( file, level, "name = \"" .. self.name .. "\",\n" )
	writeIndent( file, level, "instances = {},\n" )
	writeIndent( file, level, "components = {},\n" )

	level = level - 1
	writeIndent( file, level, "}\n" )
	writeIndent( file, level, "setmetatable( Prefabs[\"" .. self.name .. "\"], { __index = Prefab } )\n\n" )

	for _,v in pairs(self.components) do
		v:write( file, level, self.name )
	end
end

function Prefab:instantiate( position )
	local entity = Entity.create( self.name, position )

	for _,v in pairs(self.components) do
		local component = v:copy( entity )
		entity:addComponent( component )
	end

	entity.prefab = self
	self.instances[#self.instances+1] = entity

	return entity
end

function Prefab:removeInstance( instance )
	local index = 0
	for i=1, #self.instances do
		if self.instances[i] == instance then
			index = i
			break
		end
	end

	if index > 0 then
		self.instances[index] = nil
	end
end

function Prefab:update( entity )
	-- remove prefab components
	for k,_ in pairs(self.components) do
		self.components[k] = nil
	end

	-- copy updated components from template entity
	for k,v in pairs(entity.components) do
		self.components[k] = v:copy()
	end

	-- update components of all instances
	for _,entity in pairs(self.instances) do
		entity:clearComponents()

		for _,component in pairs(self.components) do
			local c = component:copy( entity )
			entity:addComponent( c )
		end
	end
end

function Prefab:revert( entity )
	-- remove entities components
	entity:clearComponents()

	-- add prefabs components to entity
	for _,v in pairs(self.components) do
		local component = v:copy( entity )
		entity:addComponent( component )
	end
end