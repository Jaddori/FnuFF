local CARET_BLINK_FREQUENCY = 0.35

EditorTextbox =
{
	fontIndex = -1,
	fontHeight = -1,
	font = nil,
	textureIndex = -1,
	position = Vec2.create({0,0}),
	size = Vec2.create({0,0}),
	depth = 0,
	color = Vec4.create({0.4, 0.4, 0.4, 1.0}),
	hoverColor = Vec4.create({0.75, 0.75, 0.75, 1}),
	pressColor = Vec4.create({0.3, 0.3, 0.3, 1}),
	focusColor = Vec4.create({ 0.65, 0.65, 0.15, 1}),
	textColor = Vec4.create({1.0, 1.0, 1.0, 1.0}),
	selectionColor = Vec4.create({ 0.35, 0.35, 0.35, 0.5 }),
	disabledColor = Vec4.create({0.45, 0.45, 0.45, 1}),
	disabledTextColor = Vec4.create({0.75, 0.75, 0.75, 1.0}),
	text = "",
	readOnly = false,
	disabled = false,
	
	hovered = false,
	pressed = false,
	focus = false,
	
	caretVisible = false,
	caretElapsed = 0,
	caretIndex = 0,
	
	selectionStart = 0,
	selectionEnd = 0,

	onFocus = nil,
	onFinish = nil,
	onTextChanged = nil,
}

function EditorTextbox.create( position, size )
	if EditorTextbox.textureIndex < 0 then
		EditorTextbox.textureIndex = Assets.loadTexture( GUI_DEFAULT_BACKGROUND_TEXTURE )
	end
	
	if EditorTextbox.fontIndex < 0 then
		EditorTextbox.fontIndex = Assets.loadFont( GUI_DEFAULT_FONT_INFO, GUI_DEFAULT_FONT_TEXTURE )
		EditorTextbox.font = Assets.getFont( EditorTextbox.fontIndex )
		EditorTextbox.fontHeight = EditorTextbox.font:getHeight()
	end
	
	local textbox = {}
	setmetatable( textbox, { __index = EditorTextbox } )
	
	--textbox.position = tableVal( position )
	textbox.position = position and position:copy() or Vec2.create({0,0})
	--textbox.size = tableVal( size )
	textbox.size = size and size:copy() or Vec2.create({0,0})
	textbox.depth = 0
	textbox.hovered = false
	textbox.pressed = false
	textbox.focus = false
	textbox.readOnly = false
	textbox.disabled = false
	textbox.caretElapsed = 0
	
	return textbox
end

function EditorTextbox.createWithWidth( width )
	assert( isnumber( width ), "Width must be a number." )

	return EditorTextbox.create( nil, Vec2.create({width,GUI_BUTTON_HEIGHT}) )
end

function EditorTextbox.createDefault()
	return EditorTextbox.create( nil, Vec2.create({0, GUI_BUTTON_HEIGHT}) )
end

function EditorTextbox:setPosition( position )
	self.position = position:copy()
end

function EditorTextbox:setSize( size )
	self.size = size:copy()
end

function EditorTextbox:setDepth( depth )
	self.depth = depth
end

function EditorTextbox:checkCapture( capture, mousePosition )
	if capture.depth < self.depth then
		if insideRect( self.position, self.size, mousePosition ) then
			capture.depth = self.depth
			capture.item = self
			capture.focusItem = self
		end
	end
end

function EditorTextbox:updateMouseInput( deltaTime, mousePosition )
	if not self.disabled and not self.readOnly then
		self.pressed = insideRect( self.position, self.size, mousePosition )
	end
end

function EditorTextbox:press( mousePosition )
	if not self.disabled and not self.readOnly then
		self.hovered = true
		self.caretIndex = self.text:len()
	end
end

function EditorTextbox:release( mousePosition )
	self.hovered = false
	self.pressed = false
	
	if insideRect( self.position, self.size, mousePosition ) then
		self:setFocus()
	else
		self:unsetFocus()
	end
end

function EditorTextbox:setFocus()
	if not self.disabled and not self.readOnly then
		self.caretVisible = true
		self.focus = true
	end
end

function EditorTextbox:unsetFocus()
	if self.focus then
		self:onFinish()
	end

	self.focus = false
	self.caretVisible = false
	self.selectionStart = self.caretIndex
	self.selectionEnd = self.caretIndex
end

function EditorTextbox:updateKeyboardInput()
	local stillFocused = true

	if self.disabled or self.readOnly then
		stillFocused = false
	else
		local textChanged = false

		-- get text input
		local textInput = Input.getTextInput()

		if textInput:len() > 0 then
			-- if there was a selection, replace the selected text
			if self.selectionStart ~= self.selectionEnd then
				local first = math.min( self.selectionStart, self.selectionEnd )
				local last = math.max( self.selectionStart, self.selectionEnd )
				
				local preText = self.text:sub( 1, first )
				local postText = self.text:sub( last+1, self.text:len() )
				
				self.text = preText .. postText
				self.caretIndex = first
				self.selectionStart = first
				self.selectionEnd = first
			end

			-- add the new text at the caret position
			if self.caretIndex <= 0 then
				self.text = textInput .. self.text
			elseif self.caretIndex >= self.text:len() then
				self.text = self.text .. textInput
			else
				local first = self.text:sub(1, self.caretIndex)
				local second = self.text:sub(self.caretIndex+1, self.text:len())
				
				self.text = first .. textInput .. second
			end
			result = true
			
			self.caretIndex = self.caretIndex + textInput:len()
			
			self.selectionStart = self.caretIndex
			self.selectionEnd = self.caretIndex

			textChanged = true
		end

		-- delete input if Backspace is pressed
		local textLength = self.text:len()
		if Input.keyRepeated( Keys.Backspace ) and textLength > 0 then
			-- delete selection
			if self.selectionStart ~= self.selectionEnd then
				local first = math.min( self.selectionStart, self.selectionEnd )
				local last = math.max( self.selectionStart, self.selectionEnd )
				
				local preSelectionText = ""
				if first > 0 then preSelectionText = self.text:sub( 1, first ) end
				
				local postSelectionText = ""
				if last < #self.text then postSelectionText = self.text:sub( last+1, textLength ) end
				
				self.text = preSelectionText .. postSelectionText
				
				self.caretIndex = first
				self.selectionStart = first
				self.selectionEnd = first
			
			-- delete from caret index
			elseif self.caretIndex > 0 then
				if self.caretIndex >= textLength then
					self.text = self.text:sub( 1, textLength-1 )
				else
					local first = self.text:sub(1, self.caretIndex-1)
					local second = self.text:sub(self.caretIndex+1, textLength)
					
					self.text = first .. second
				end
				
				self.caretIndex = self.caretIndex - 1
			end
			
			textLength = self.text:len()
			textChanged = true
		end
		
		-- delete input if Delete is pressed
		if Input.keyRepeated( Keys.Delete ) then
			-- delete selection
			if self.selectionStart ~= self.selectionEnd then
				local first = math.min( self.selectionStart, self.selectionEnd )
				local last = math.max( self.selectionStart, self.selectionEnd )
				
				local preSelectionText = ""
				if first > 0 then preSelectionText = self.text:sub( 1, first ) end
				
				local postSelectionText = ""
				if last < #self.text then postSelectionText = self.text:sub( last+1, textLength ) end
				
				self.text = preSelectionText .. postSelectionText
				
				self.caretIndex = first
				self.selectionStart = first
				self.selectionEnd = first

				textChanged = true
			
			-- delete from caret index
			elseif self.caretIndex < textLength then
				local preText = self.text:sub( 1, self.caretIndex )
				local postText = self.text:sub( self.caretIndex+2 )
				
				self.text = preText .. postText

				textChanged = true
			end
		end
		
		-- move the caret with arrow keys
		if Input.keyRepeated( Keys.Left ) and self.caretIndex > 0 then
			self.caretIndex = self.caretIndex - 1
			
			if Input.keyDown( Keys.LeftShift ) or Input.keyDown( Keys.RightShift ) then
				self.selectionEnd = self.caretIndex
			else
				self.selectionStart = self.caretIndex
				self.selectionEnd = self.selectionStart
			end
		end
		
		if Input.keyRepeated( Keys.Right ) then
			if self.caretIndex < textLength then
				self.caretIndex = self.caretIndex + 1
			end
			
			if Input.keyDown( Keys.LeftShift ) or Input.keyDown( Keys.RightShift ) then
				self.selectionEnd = self.caretIndex
			else
				self.selectionStart = self.caretIndex
				self.selectionEnd = self.selectionStart
			end
		end

		-- move the caret with Home/End keys
		if Input.keyRepeated( Keys.Home ) then
			self.caretIndex = 0

			if Input.keyDown( Keys.LeftShift ) or Input.keyDown( Keys.RightShift ) then
				self.selectionEnd = self.caretIndex
			else
				self.selectionStart = self.caretIndex
				self.selectionEnd = self.selectionStart
			end
		end

		if Input.keyRepeated( Keys.End ) then
			self.caretIndex = #self.text

			if Input.keyDown( Keys.LeftShift ) or Input.keyDown( Keys.RightShift ) then
				self.selectionEnd = self.caretIndex
			else
				self.selectionStart = self.caretIndex
				self.selectionEnd = self.selectionStart
			end
		end

		-- select all text
		if Input.keyDown( Keys.LeftControl ) or Input.keyDown( Keys.RightControl ) then
			if Input.keyPressed( Keys.A ) then
				self:selectAll()
			end
		end

		-- remove focus if Return is pressed
		if Input.keyReleased( Keys.Return) or Input.keyReleased( Keys.KeypadReturn ) then
			self:release({-1,-1})
			stillFocused = false
		end

		if textChanged and self.onTextChanged then
			self:onTextChanged()
		end
	end

	return stillFocused
end

function EditorTextbox:update( deltaTime, mousePosition )
	if not self.disabled and not self.readOnly then
		self.hovered = insideRect( self.position, self.size, mousePosition )
	end

	-- blink the caret
	if self.focus then
		self.caretElapsed = self.caretElapsed + deltaTime
		if self.caretElapsed >= CARET_BLINK_FREQUENCY then
			self.caretElapsed = self.caretElapsed - CARET_BLINK_FREQUENCY
			
			self.caretVisible = not self.caretVisible
		end
	end
end

function EditorTextbox:render()
	local color = self.color
	local textColor = self.textColor

	if self.disabled then
		color = self.disabledColor
		textColor = self.disabledTextColor
	elseif self.pressed then
		color = self.pressColor
	elseif self.hovered then
		color = self.hoverColor
	elseif self.focus then
		color = self.focusColor
	end
	
	local textPadding = 8
	
	-- draw background
	Graphics.queueQuad( self.textureIndex, self.position, self.size, self.depth, color )
	
	-- draw text
	local textPosition = Vec2.create({self.position[1] + textPadding, self.position[2]})
	Graphics.queueText( self.fontIndex, self.text, textPosition, self.depth + GUI_DEPTH_SMALL_INC, textColor )
	if self.caretVisible then
		local xoffset = self.font:measureText( self.text:sub(1, self.caretIndex) )[1] - 2
		Graphics.queueText( self.fontIndex, "|", {textPosition[1]+xoffset, textPosition[2]}, self.depth + GUI_DEPTH_SMALL_INC*2, self.textColor )
	end
	
	-- draw selection
	if self.selectionStart ~= self.selectionEnd then
		local preSelectionWidth = 0
		--local postSelectionWidth = 0
		
		local first = self.selectionStart
		local last = self.selectionEnd
		
		-- make sure first is actually the first
		if last < first then last, first = first, last end
		
		if first > 0 then
			local preSelectionText = self.text:sub( 1, first )
			preSelectionWidth = self.font:measureText( preSelectionText )[1]
		end
		
		local selectionText = self.text:sub( first+1, last )
		local selectionWidth = self.font:measureText( selectionText )[1]
		
		local position = Vec2.create({ self.position[1] + preSelectionWidth + textPadding, self.position[2]+2 })
		local size = Vec2.create({ selectionWidth, self.size[2]-4 })
		
		Graphics.queueQuad( self.textureIndex, position, size, self.depth + GUI_DEPTH_SMALL_INC*3, self.selectionColor )
	end
end

function EditorTextbox:onFinish()
end

function EditorTextbox:setText( text )
	self.text = tostring( text )
	self.caretIndex = self.text:len()
	self.selectionStart = self.caretIndex
	self.selectionEnd = self.caretIndex
end

function EditorTextbox:selectText( first, last )
	self.selectionStart = first
	if self.selectionStart < 0 then
		self.selectionStart = 0
	end

	self.selectionEnd = last
	if self.selectionEnd > self.text:len() then
		self.selectionEnd = self.text:len()
	end

	self.caretIndex = self.selectionEnd
end

function EditorTextbox:selectAll()
	self:selectText( 0, self.text:len() )
end