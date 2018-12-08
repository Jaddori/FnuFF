Props =
{
	items = {},
}

function Props:add( position, orientation, scale, mesh )
	assert( istable( position ) and #position == 3, "Position must be a table with 3 components." )
	assert( istable( orientation ) and #orientation == 4, "Orientation must be a table with 4 components." )
	assert( istable( scale ) and #scale == 3, "Scale must be a table with 3 components." )
	assert( isstring( mesh ), "Mesh should be a path to a mesh.")

	local meshIndex = Assets.loadMesh( "./assets/models/" .. mesh )
	local transform = Transform.create()

	transform:setPosition( position )
	transform:setOrientation( orientation )
	transform:setScale( scale )

	local item =
	{
		transform = transform,
		meshIndex = meshIndex,
	}

	self.items[#self.items+1] = item
end

function Props:render()
	for _,v in pairs(self.items) do
		Graphics.queueMesh( v.meshIndex, v.transform )
	end
end