local history =
{
	commands = {},
	index = 0,
	maxCommands = 100, -- TODO: Enforce max size
}

function history:load()
	-- load all command scripts
	local commandScripts = Filesystem.getFiles( "./assets/scripts/editor/commands/*" )
	for _,v in pairs(commandScripts) do
		doscript( "editor/commands/" .. v )
	end
end

function history:addCommand( command )
	-- remove items after the current index
	if self.index < #self.commands then
		for i=self.index+1, #self.commands do
			self.commands[i] = nil
		end
	end

	-- add command to list
	self.commands[#self.commands+1] = command
	self.index = self.index + 1
end

function history:undo()
	if self.index > 0 then
		self.commands[self.index]:undo()
		self.index = self.index - 1
	end
end

function history:redo()
	if self.index < #self.commands then
		self.index = self.index + 1
		self.commands[self.index]:redo()
	end
end

return history