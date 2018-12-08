local DEFAULT_FONT_INFO = "./assets/fonts/verdana12.bin"
local DEFAULT_FONT_TEXTURE = "./assets/fonts/verdana12.dds"

EditorLabel =
{
	fontIndex = -1,
	position = Vec2.create({0,0}),
	size = Vec2.create({0,0}),
	depth = 0,
	textColor = Vec2.create({1.0, 1.0, 1.0, 1.0}),
	alignText = {},
}

function EditorLabel.create( position, size, text )
	if EditorLabel.fontIndex < 0 then
		EditorLabel.fontIndex = Assets.loadFont( DEFAULT_FONT_INFO, DEFAULT_FONT_TEXTURE )
	end
	
	local label = {}
	setmetatable( label, { __index = EditorLabel } )
	
	--label.position = tableVal( position )
	label.position = position and position:copy() or Vec2.create({0,0})
	--label.size = tableVal( size )
	label.size = size and size:copy() or Vec2.create({0,0})
	label.depth = 0
	label.alignText = EditorAlignText.create( label, EditorLabel.fontIndex, text )
	label.alignText:align( ALIGN_NEAR, ALIGN_NEAR )
	
	return label
end

function EditorLabel.createWithText( text )
	assert( isstring( text ), "Text must be a string." )

	return EditorLabel.create( nil, Vec2.create({0, GUI_BUTTON_HEIGHT}), text )
end

function EditorLabel:setPosition( position )
	self.position = position:copy()
	self.alignText:align()
end

function EditorLabel:setSize( size )
	self.size = size:copy()
	self.alignText:align()
end

function EditorLabel:setDepth( depth )
	self.depth = depth
end

function EditorLabel:setTextAlignment( horizontal, vertical )
	self.alignText:align( horizontal, vertical )
end

function EditorLabel:checkCapture( capture, mousePosition )
end

function EditorLabel:updateMouseInput( deltaTime )
end

function EditorLabel:update( deltaTime )
end

function EditorLabel:render()
	self.alignText:render( self.depth, self.textColor )
end

function EditorLabel:loadFont( info, texture )
	local fontIndex = Assets.loadFont( info, texture )
	self.alignText:setFontIndex( fontIndex )
end

function EditorLabel:setText( text )
	self.alignText:setText( text )
end