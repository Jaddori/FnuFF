CommandRemoveEntity =
{
	editor = nil,
	entity = nil,
}

function CommandRemoveEntity.create( editor, entity )
	assert( editor, "Editor was nil." )
	assert( entity, "Entity was nil." )

	local command =
	{
		editor = editor,
		entity = entity,
	}

	setmetatable( command, { __index = CommandRemoveEntity } )

	return command
end

function CommandRemoveEntity:undo()
	self.editor:addEntity( self.entity )
end

function CommandRemoveEntity:redo()
	self.editor:removeEntity( self.entity )
end