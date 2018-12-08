local DEFAULT_TEXTURE = "./assets/textures/white.dds"
local DEFAULT_CROSS_TEXTURE = "./assets/textures/cross.dds"
local DEFAULT_FONT_INFO = "./assets/fonts/verdana12.bin"
local DEFAULT_FONT_TEXTURE = "./assets/fonts/verdana12.dds"
EDITOR_WINDOW_TITLEBAR_HEIGHT = 24
EDITOR_WINDOW_DEFAULT_WIDTH = 256
EDITOR_WINDOW_DEFAULT_HEIGHT = 128
EDITOR_WINDOW_CROSS_SIZE = 16

EditorWindow =
{
	visible = false,
	title = "",
	position = Vec2.create({0,0}),
	size = Vec2.create({0,0}),
	depth = 0,
	titlebarSize = Vec2.create({0,EDITOR_WINDOW_TITLEBAR_HEIGHT}),

	crossSize = Vec2.create({EDITOR_WINDOW_CROSS_SIZE, EDITOR_WINDOW_CROSS_SIZE}),
	crossCaptured = false,
	crossHovered = false,
	crossPressed = false,
	crossColor = Vec4.create({1,1,1,1}),
	crossHoverColor = Vec4.create({1,0,0,1}),
	crossPressColor = Vec4.create({0.5,0,0,1}),

	textureIndex = -1,
	crossTextureIndex = -1,
	backgroundColor = Vec4.create({0.35, 0.35, 0.35, 1.0}),

	fontIndex = -1,
	titlebarColor = Vec4.create({0.45, 0.45, 0.45, 1.0}),
	titlebarTextColor = Vec4.create({1,1,1,1}),

	items = {},

	hovered = false,
	pressed = false,
	focused = false,
	movementOffset = Vec2.create({0,0}),

	padding = 4,
	onFocus = nil,
	onClose = nil,
}

function EditorWindow.create( title, position, size )
	if EditorWindow.textureIndex < 0 then
		EditorWindow.textureIndex = Assets.loadTexture( DEFAULT_TEXTURE )
		EditorWindow.crossTextureIndex = Assets.loadTexture( DEFAULT_CROSS_TEXTURE )
		EditorWindow.fontIndex = Assets.loadFont( DEFAULT_FONT_INFO, DEFAULT_FONT_TEXTURE )
	end

	local result =
	{
		visible = true,
		title = title,
		depth = 0,
		titlebarSize = Vec2.create({0, EDITOR_WINDOW_TITLEBAR_HEIGHT}),

		items = {},

		hovered = false,
		pressed = false,
		focused = false,
		movementOffset = Vec2.create({0,0}),

		crossSize = Vec2.create({EDITOR_WINDOW_CROSS_SIZE, EDITOR_WINDOW_CROSS_SIZE}),
		crossCaptured = false,
	}

	if position then
		result.position = position:copy()
	else
		result.position = {0, GUI_MENU_HEIGHT}
	end

	if size then
		result.size = size:copy()
	else
		result.size = {EDITOR_WINDOW_DEFAULT_WIDTH, EDITOR_WINDOW_DEFAULT_HEIGHT}
	end

	result.titlebarSize[1] = result.size[1]

	setmetatable( result, { __index = EditorWindow } )

	return result
end

function EditorWindow:setPosition( position )
	self.position = position:copy()

	self:layout()
end

function EditorWindow:setSize( size )
	self.size = size:copy()

	self:layout()
end

function EditorWindow:setDepth( depth )
	self.depth = depth
	for _,v in pairs(self.items) do
		v:setDepth( self.depth + GUI_DEPTH_SMALL_INC )
	end
end

function EditorWindow:addItem( item )
	item:setDepth( self.depth + GUI_DEPTH_SMALL_INC )
	self.items[#self.items+1] = item

	self:layout()
end

function EditorWindow:layout()
	local penultimateHeight = 0
	local yoffset = self.titlebarSize[2] + self.padding
	for _,v in pairs(self.items) do
		v:setPosition( Vec2.create({self.position[1], self.position[2] + yoffset}) )
		v:setSize( Vec2.create({self.size[1], v.size[2]}) )

		yoffset = yoffset + v.size[2] + self.padding
		penultimateHeight = yoffset + self.padding
	end

	self.size[2] = penultimateHeight
end

function EditorWindow:close()
	self.visible = false
	
	if self.onClose then
		self:onClose()
	end
end

function EditorWindow:checkCapture( capture, mousePosition )
	if capture.depth < self.depth then
		if self.visible then
			for _,v in pairs(self.items) do
				v:checkCapture( capture, mousePosition )
			end
			
			if insideRect( self.position, self.size, mousePosition ) then
				if capture.depth < self.depth then
					capture.depth = self.depth
					capture.item = self
				end
			end
		end
	end
end

function EditorWindow:updateMouseInput( deltaTime, mousePosition )
	if self.crossCaptured then
		local crossPosition = Vec2.create({ self.position[1] + self.size[1] - self.crossSize[1], self.position[2] })
		self.crossPressed = insideRect( crossPosition, self.crossSize, mousePosition )
	elseif self.titlebarCaptured then
		--self.position = subVec( mousePosition, self.movementOffset )
		self.position = mousePosition:sub( movementOffset )

		-- clamp position to be inside window
		local minx = 0
		local maxx = ( WINDOW_WIDTH - GUI_PANEL_WIDTH - self.size[1] )
		local miny = GUI_MENU_HEIGHT
		local maxy = ( WINDOW_HEIGHT - self.size[2] )

		if self.position[1] < minx then
			self.position[1] = minx
		elseif self.position[1] > maxx then
			self.position[1] = maxx
		end

		if self.position[2] < miny then
			self.position[2] = miny
		elseif self.position[2] > maxy then
			self.position[2] = maxy
		end

		-- update item positions
		self:layout()
	end
end

function EditorWindow:press( mousePosition )
	self.crossCaptured = false
	self.titlebarCaptured = false

	-- check interation with cross
	local crossPosition = Vec2.create({ self.position[1] + self.size[1] - self.crossSize[1], self.position[2] })
	if insideRect( crossPosition, self.crossSize, mousePosition ) then
		self.crossCaptured = true
		self.crossHovered = true

	-- check interaction with titlebar
	elseif insideRect( self.position, self.titlebarSize, mousePosition ) then
		self.titlebarCaptured = true
		self.movementOffset = subVec( mousePosition, self.position )
	end

	if insideRect( self.position, self.size, mousePosition ) then
		if self.onFocus then
			self:onFocus()
		end
	end
end

function EditorWindow:release( mousePosition )
	local closed = false

	if self.crossCaptured then
		local crossPosition = Vec2.create({ self.position[1] + self.size[1] - self.crossSize[1], self.position[2] })
		if insideRect( crossPosition, self.crossSize, mousePosition ) then
			self:close()
			closed = true
		end
	end

	if not closed then
		if insideRect( self.position, self.size, mousePosition ) then
			self.focused = true
		end
	end

	self.crossCaptured = false
	self.crossHovered = false
	self.crossPressed = false
	self.titlebarCaptured = false
end

function EditorWindow:updateKeyboardInput()
	local result = false

	for _,v in pairs(self.items) do
		if v.updateKeyboardInput then
			result = v:updateKeyboardInput() or result
		end
	end

	return result
end

function EditorWindow:update( deltaTime, mousePosition )
	if self.visible then
		local crossPosition = Vec2.create({ self.position[1] + self.size[1] - self.crossSize[1], self.position[2] })
		if insideRect( crossPosition, self.crossSize, mousePosition ) then
			self.crossHovered = true
		else
			self.crossHovered = false
		end

		-- update items
		for _,v in pairs(self.items) do
			v:update( deltaTime, mousePosition )
		end
	end
end

function EditorWindow:render()
	if self.visible then
		-- render background
		Graphics.queueQuad( self.textureIndex, self.position, self.size, self.depth, self.backgroundColor )
		Graphics.queueQuad( self.textureIndex, self.position, self.titlebarSize, self.depth + GUI_DEPTH_SMALL_INC, self.titlebarColor )

		-- render title
		local padding = 4
		local textPosition = Vec2.create({self.position[1] + padding, self.position[2]})
		Graphics.queueText( self.fontIndex, self.title, textPosition, self.depth + GUI_DEPTH_SMALL_INC*2, self.titlebarTextColor )

		-- render cross
		local crossColor = self.crossColor
		if self.crossPressed then
			crossColor = self.crossPressColor
		elseif self.crossHovered then
			crossColor = self.crossHoverColor
		end
		padding = ( EDITOR_WINDOW_TITLEBAR_HEIGHT - self.crossSize[1] ) * 0.5
		local crossPosition = Vec2.create({self.position[1] + self.size[1] - self.crossSize[1] - padding, self.position[2] + padding })
		Graphics.queueQuad( self.crossTextureIndex, crossPosition, self.crossSize, self.depth + GUI_DEPTH_SMALL_INC*2, crossColor )

		-- render items
		for _,v in pairs(self.items) do
			v:render()
		end
	end
end