ALIGN_NEAR = 1
ALIGN_MIDDLE = 2
ALIGN_FAR = 3

EditorAlignText =
{
	parent = {},
	fontIndex = -1,
	text = "",
	position = Vec2.create({0,0}),
	horizontal = ALIGN_NEAR,
	vertical = ALIGN_NEAR,
	horizontalPadding = 4,
	verticalPadding = 0,
}

function EditorAlignText.create( parent, fontIndex, text )
	local result =
	{
		parent = parent,
		fontIndex = fontIndex,
		text = text,
		position = Vec2.create({0,0}),
		horizontal = ALIGN_NEAR,
		vertical = ALIGN_NEAR,
	}

	setmetatable( result, { __index = EditorAlignText } )

	return result
end

function EditorAlignText:align( horizontal, vertical )
	self.horizontal = horizontal or self.horizontal
	self.vertical = vertical or self.vertical

	local font = Assets.getFont( self.fontIndex )
	local bounds = font:measureText( self.text )

	local position = self.parent.position
	local size = self.parent.size

	-- horizontal
	if self.horizontal == ALIGN_NEAR then
		self.position[1] = position[1] + self.horizontalPadding
	elseif self.horizontal == ALIGN_MIDDLE then
		self.position[1] = position[1] + size[1] * 0.5 - bounds[1] * 0.5
	else -- ALIGN_FAR
		self.position[1] = position[1] + size[1] - bounds[1] - self.horizontalPadding
	end

	-- vertical
	if self.vertical == ALIGN_NEAR then
		self.position[2] = position[2] + self.verticalPadding
	elseif self.vertical == ALIGN_MIDDLE then
		self.position[2] = position[2] + size[2] * 0.5 - bounds[2] * 0.5
	else -- ALIGN_FAR
		self.position[2] = position[2] + size[2] - bounds[2] - self.verticalPadding
	end
end

function EditorAlignText:render( depth, color )
	Graphics.queueText( self.fontIndex, self.text, self.position, depth, color )
end

function EditorAlignText:setFontIndex( fontIndex )
	self.fontIndex = fontIndex
	self:align()
end

function EditorAlignText:setText( text )
	self.text = text
	self:align()
end

function EditorAlignText:setPadding( horizontal, vertical )
	self.horizontalPadding = horizontal
	self.verticalPadding = vertical
	self:align()
end