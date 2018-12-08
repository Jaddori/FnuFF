CommandAddEntity =
{
	editor = nil,
	entity = nil,
	position = Vec3.create( {0,0,0} ),
}

function CommandAddEntity.create( editor, entity )
	assert( editor, "Editor was nil." )
	assert( entity, "Entity was nil." )

	local command =
	{
		editor = editor,
		entity = entity,
	}

	copyVec( command.position, entity.position )

	setmetatable( command, { __index = CommandAddEntity } )

	return command
end

function CommandAddEntity:undo()
	editor:removeEntity( self.entity )
end

function CommandAddEntity:redo()
	editor:createEntity( self.position )
end