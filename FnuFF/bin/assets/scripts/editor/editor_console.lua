local CONSOLE_MAX_HISTORY = 10

CONSOLE_DEPTH = 0.8
local console =
{
	textureIndex = -1,
	position = Vec2.create({0,GUI_MENU_HEIGHT}),
	size = Vec2.create({WINDOW_WIDTH-GUI_PANEL_WIDTH,256}),
	depth = CONSOLE_DEPTH,
	visible = false,
	color = Vec2.create({0.25, 0.25, 0.25, 1.0}),
	textColor = Vec2.create({1,1,1,1}),
	inputTextbox = nil,
	history = {},
	curHistory = 0,
}

function console:load()
	self.textureIndex = Assets.loadTexture( "./assets/textures/white.dds" )
	
	self.inputTextbox = EditorTextbox.create( Vec2.create({0,self.position[2]+self.size[2]+1}), Vec2.create({WINDOW_WIDTH-GUI_PANEL_WIDTH, GUI_BUTTON_HEIGHT}) )
	self.inputTextbox.onFinish = function( self )
		local text = self.text
		if text:len() > 0 then
			self:setText( "" )
			
			local func = load(text)
			local succeeded, errorMessage = pcall( func )
			
			if not succeeded then
				Log.error( errorMessage )
			end
			
			self.focus = true
			
			-- store command in command history
			console.history[#console.history+1] = text
			console.curHistory = 0
		end
	end
end

function console:checkCapture( capture, mousePosition )
	if self.visible then
		capture.depth = self.depth
		capture.item = self
		capture.focusItem = self
	end
end

function console:updateMouseInput( deltaTime, mousePosition )
end

function console:press( mousePosition )
end

function console:release( mousePosition )
end

function console:setFocus()
	self.inputTextbox:setFocus()
end

function console:unsetFocus()
end

function console:updateKeyboardInput()
	local stillFocused = self.visible

	self.inputTextbox:updateKeyboardInput()

	-- manipulate command history
	if Input.keyRepeated( Keys.Up ) then
		self.curHistory = self.curHistory - 1
		if self.curHistory < 1 then
			self.curHistory = #self.history
		end
		
		self.inputTextbox:setText( self.history[self.curHistory] )

		capture.mouseCaptured = true
	end
	
	if Input.keyRepeated( Keys.Down ) then
		self.curHistory = self.curHistory + 1
		if self.curHistory > #self.history then
			self.curHistory = 1
		end
		
		self.inputTextbox:setText( self.history[self.curHistory] )

		capture.mouseCaptured = true
	end

	return stillFocused
end

function console:update( deltaTime, mousePosition )
	if Input.keyReleased( Keys.Tilde ) then
		self.visible = not self.visible
		self.inputTextbox.focus = self.visible

		if self.visible then
			self.inputTextbox:setFocus()
		end
	end

	if self.visible then
		self.inputTextbox:update( deltaTime, mousePosition )
	end
end

function console:render()
	if self.visible then
		-- background
		Graphics.queueQuad( self.textureIndex, self.position, self.size, self.depth, self.color )
		
		-- textbox
		self.inputTextbox:render()
	end
end

return console