EditorInputbox =
{
	label = {},
	textbox = {},
	position = Vec2.create({0,0}),
	size = Vec2.create({0,0}),
	depth = 0,
}

function EditorInputbox.create( position, width, text )
	local result = 
	{
		--position = tableVal( position ),
		position = position and position:copy() or Vec2.create({0,0}),
		size = Vec2.create({0,0}),
		depth = 0,
		label = {},
		textbox = {},
	}

	setmetatable( result, { __index = EditorInputbox } )

	local yoffset = 0
	result.label = EditorLabel.create( Vec2.create({result.position[1], result.position[2] + yoffset}), Vec2.create({width or 0, GUI_BUTTON_HEIGHT}), text )
	yoffset = yoffset + GUI_BUTTON_HEIGHT

	result.textbox = EditorTextbox.create( Vec2.create({result.position[1], result.position[2] + yoffset}), Vec2.create({width or 0, GUI_BUTTON_HEIGHT}) )
	result.textbox.onFocus = function( textbox )
		textbox:selectAll()
	end
	yoffset = yoffset + GUI_BUTTON_HEIGHT

	result.size = Vec2.create({width, yoffset})

	return result
end

function EditorInputbox.createWithText( text )
	assert( isstring( text ), "Text must be a string." )

	return EditorInputbox.create( nil, 0, text )
end

function EditorInputbox:setPosition( position )
	self.label:setPosition( position )
	self.textbox:setPosition( Vec2.create({position[1], position[2] + self.label.size[2]}) )
end

function EditorInputbox:setSize( size )
	self.label:setSize( Vec2.create({size[1], GUI_BUTTON_HEIGHT}) )
	self.textbox:setSize( Vec2.create({size[1], GUI_BUTTON_HEIGHT}) )
end

function EditorInputbox:setDepth( depth )
	self.depth = depth
	self.label.depth = depth
	self.textbox.depth = depth
end

function EditorInputbox:checkCapture( capture, mousePosition )
	self.textbox:checkCapture( capture, mousePosition )
end

function EditorInputbox:update( deltaTime, mousePosition )
	self.label:update( deltaTime, mousePosition )
	self.textbox:update( deltaTime, mousePosition )
end

function EditorInputbox:render()
	self.label:render()
	self.textbox:render()
end