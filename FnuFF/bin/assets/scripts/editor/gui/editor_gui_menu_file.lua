local file =
{
	visible = false,
	items = {},
	fileButton = nil,
	newButton = nil,
	openButton = nil,
	saveButton = nil,
	saveAsButton = nil,
	compileButton = nil,
	exitButton = nil,

	onNew = nil,
	onOpen = nil,
	onSave = nil,
	onSaveAs = nil,
	onCompile = nil,
	onExit = nil,
}

function file:load( xoffset, items, depth )
	local width = 64
	
	self.fileButton = EditorButton.create( Vec2.create({xoffset, 0}), Vec2.create({width, GUI_MENU_HEIGHT}), "File" )
	self.fileButton.depth = depth + GUI_DEPTH_INC
	self.fileButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	self.fileButton.onClick = function( button )
		self.visible = true
		self.fileButton.color = {0.4, 0.4, 0.4, 1.0}
	end
	items[#items+1] = self.fileButton
	
	-- drop down menu
	local yoffset = GUI_MENU_HEIGHT
	self.newButton = EditorButton.create( Vec2.create({xoffset, yoffset}), Vec2.create({GUI_MENU_FILE_BUTTON_WIDTH, GUI_BUTTON_HEIGHT}), "New" )
	self.newButton.depth = self.fileButton.depth + GUI_DEPTH_INC
	self.newButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	self.newButton.onClick = function( button )
		if self.onNew then
			self.onNew()
		end
	end
	yoffset = yoffset + GUI_BUTTON_HEIGHT
	
	self.openButton = EditorButton.create( Vec2.create({xoffset, yoffset}), Vec2.create({GUI_MENU_FILE_BUTTON_WIDTH, GUI_BUTTON_HEIGHT}), "Open" )
	self.openButton.depth = self.fileButton.depth + GUI_DEPTH_INC
	self.openButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	self.openButton.onClick = function( button )
		if self.onOpen then
			self.onOpen()
		end
	end
	yoffset = yoffset + GUI_BUTTON_HEIGHT
	
	self.saveButton = EditorButton.create( Vec2.create({xoffset, yoffset}), Vec2.create({GUI_MENU_FILE_BUTTON_WIDTH, GUI_BUTTON_HEIGHT}), "Save" )
	self.saveButton.depth = self.fileButton.depth + GUI_DEPTH_INC
	self.saveButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	self.saveButton.onClick = function( button )
		if self.onSave then
			self.onSave()
		end
	end
	yoffset = yoffset + GUI_BUTTON_HEIGHT
	
	self.saveAsButton = EditorButton.create( Vec2.create({xoffset, yoffset}), Vec2.create({GUI_MENU_FILE_BUTTON_WIDTH, GUI_BUTTON_HEIGHT}), "Save As" )
	self.saveAsButton.depth = self.fileButton.depth + GUI_DEPTH_INC
	self.saveAsButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	self.saveAsButton.onClick = function( button )
		if self.onSaveAs then
			self.onSaveAs()
		end
	end
	yoffset = yoffset + GUI_BUTTON_HEIGHT

	self.compileButton = EditorButton.create( Vec2.create({xoffset, yoffset}), Vec2.create({GUI_MENU_FILE_BUTTON_WIDTH, GUI_BUTTON_HEIGHT}), "Compile" )
	self.compileButton.depth = self.fileButton.depth + GUI_DEPTH_INC
	self.compileButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	self.compileButton.onClick = function( button )
		if self.onCompile then
			self.onCompile()
		end
	end
	yoffset = yoffset + GUI_BUTTON_HEIGHT
	
	self.exitButton = EditorButton.create( Vec2.create({xoffset, yoffset}), Vec2.create({GUI_MENU_FILE_BUTTON_WIDTH, GUI_BUTTON_HEIGHT}), "Exit" )
	self.exitButton.depth = self.fileButton.depth + GUI_DEPTH_INC
	self.exitButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	self.exitButton.onClick = function( button )
		self.visible = false

		if self.onExit then
			self.onExit()
		end
	end
	yoffset = yoffset + GUI_BUTTON_HEIGHT
	
	self.items[#self.items+1] = self.newButton
	self.items[#self.items+1] = self.openButton
	self.items[#self.items+1] = self.saveButton
	self.items[#self.items+1] = self.saveAsButton
	self.items[#self.items+1] = self.compileButton
	self.items[#self.items+1] = self.exitButton
	
	return width
end

function file:checkCapture( capture, mousePosition )
	if self.visible then
		local localCapture =
		{
			depth = capture.depth,
			button = capture.button,
			item = nil,
			focusItem = nil
		}

		for _,v in pairs(self.items) do
			v:checkCapture( localCapture, mousePosition )
		end

		if localCapture.item then
			capture.depth = localCapture.depth
			capture.item = localCapture.item
			capture.focusItem = localCapture.focusItem
		else
			self.visible = false
			self.fileButton.color = nil
		end
	end
end

function file:update( deltaTime, mousePosition )
	if self.visible then
		for _,v in pairs(self.items) do
			v:update( deltaTime, mousePosition )
		end
	end
end

function file:render()
	if self.visible then
		for _,v in pairs(self.items) do
			v:render()
		end
	end
end

return file