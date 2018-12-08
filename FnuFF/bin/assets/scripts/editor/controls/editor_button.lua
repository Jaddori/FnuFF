EditorButton =
{
	textureIndex = -1,
	position = Vec3.create({0,0}),
	size = Vec2.create({0,0}),
	depth = 0,
	color = Vec3.create({0.6,0.6,0.6,1}),
	hoverColor = Vec3.create({ 0.75, 0.75, 0.75, 1 }),
	pressColor = Vec3.create({ 0.35, 0.35, 0.35, 1 }),
	disabledColor = Vec3.create({ 0.35, 0.35, 0.35, 1.0 }),
	
	fontIndex = -1,
	alignText = {},
	textColor = Vec4.create({1,1,1,1}),
	disabledTextColor = Vec4.create({0.75, 0.75, 0.75, 1.0}),

	disabled = false,
	hovered = false,
	pressed = false,
}

function EditorButton.create( position, size, text )
	if EditorButton.textureIndex < 0 then
		EditorButton.textureIndex = Assets.loadTexture( GUI_DEFAULT_BACKGROUND_TEXTURE )
	end
	
	if EditorButton.fontIndex < 0 then
		EditorButton.fontIndex = Assets.loadFont( GUI_DEFAULT_FONT_INFO, GUI_DEFAULT_FONT_TEXTURE )
	end
	
	local button = {}
	setmetatable( button, { __index = EditorButton } )
	
	if position then
		button.position = position:copy()
	else
		button.position = Vec2.create({0,0})
	end

	if size then
		button.size = size:copy()
	else
		button.size = Vec2.create({0,0})
	end
	
	button.depth = 0
	button.hovered = false
	button.pressed = false
	--button.text = text or "Button"
	button.alignText = EditorAlignText.create( button, EditorButton.fontIndex, text or "Button" )
	button.alignText:align( ALIGN_MIDDLE, ALIGN_NEAR )
	button.disabled = false
	
	return button
end

function EditorButton.createWithText( text )
	assert( isstring( text ), "Text must be a string." )

	return EditorButton.create( nil, Vec2.create({0, GUI_BUTTON_HEIGHT}), text )
end

function EditorButton:setPosition( position )
	self.position = position:copy()
	self.alignText:align()
end

function EditorButton:setSize( size )
	self.size = size:copy()
	self.alignText:align()
end

function EditorButton:setDepth( depth )
	self.depth = depth
end

function EditorButton:setText( text )
	self.alignText:setText( text )
end

function EditorButton:setTextAlignment( horizontal, vertical )
	self.alignText:align( horizontal, vertical )
end

function EditorButton:checkCapture( capture, mousePosition )
	if capture.depth < self.depth then
		if insideRect( self.position, self.size, mousePosition ) then
			capture.depth = self.depth
			capture.item = self
		end
	end
end

function EditorButton:updateMouseInput( deltaTime, mousePosition )
	if not self.disabled then
		self.pressed = insideRect( self.position, self.size, mousePosition ) 
	end
end

function EditorButton:press( mousePosition )
	self.hovered = true
end

function EditorButton:release( mousePosition )
	if not self.disabled then
		if insideRect( self.position, self.size, mousePosition ) then
			self:onClick()
		else
			self:onUnclicked()
		end

		self.hovered = false
		self.pressed = false
	end
end

function EditorButton:update( deltaTime, mousePosition )
	if not self.disabled then
		self.hovered = insideRect( self.position, self.size, mousePosition )
	end
end

function EditorButton:render()
	local color = self.color
	local textColor = self.textColor

	if self.disabled then
		color = self.disabledColor
		textColor = self.disabledTextColor
	elseif self.pressed then
		color = self.pressColor
	elseif self.hovered then
		color = self.hoverColor
	end
	
	-- render background
	Graphics.queueQuad( self.textureIndex, self.position, self.size, self.depth, color )
	
	-- render text
	self.alignText:render( self.depth + GUI_DEPTH_SMALL_INC, textColor )
end

function EditorButton:onClick()
end

function EditorButton:onUnclicked()
end