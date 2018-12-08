CommandScale = 
{
	oldScale = Vec3.create( {0,0,0} ),
	newScale = Vec3.create( {0,0,0} ),
	entity = nil,
}

function CommandScale.create( oldScale, newScale, entity )
	assert( oldScale, "OldScale was nil." )
	assert( newScale, "NewScale was nil." )
	assert( entity, "Entity was nil." )

	local command = 
	{
		oldScale = Vec3.create( {0,0,0} ),
		newScale = Vec3.create( {0,0,0} ),
		entity = entity
	}

	copyVec( oldScale, command.oldScale )
	copyVec( newScale, command.newScale )

	setmetatable( command, { __index = CommandScale } )

	return command
end

function CommandScale:undo()
	copyVec( self.oldScale, self.entity.scale )
end

function CommandScale:redo()
	copyVec( self.newScale, self.entity.scale )
end