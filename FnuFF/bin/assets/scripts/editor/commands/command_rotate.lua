CommandRotate = 
{
	oldOrientation = Vec4.create( {0,0,0,1} ),
	newOrientation = Vec4.create( {0,0,0,1} ),
	entity = nil,
}

function CommandRotate.create( oldOrientation, newOrientation, entity )
	assert( oldOrientation, "OldOrientation was nil." )
	assert( newOrientation, "NewOrientation was nil." )
	assert( entity, "Entity was nil." )

	local command = 
	{
		oldOrientation = Vec4.create( {0,0,0} ),
		newOrientation = Vec4.create( {0,0,0} ),
		entity = entity
	}

	copyVec( oldOrientation, command.oldOrientation )
	copyVec( newOrientation, command.newOrientation )

	setmetatable( command, { __index = CommandRotate } )

	return command
end

function CommandRotate:undo()
	copyVec( self.oldOrientation, self.entity.orientation )
end

function CommandRotate:redo()
	copyVec( self.newOrientation, self.entity.orientation )
end