local DEFAULT_TEXTURE = "./assets/textures/white.dds"
local DEFAULT_FONT_INFO = "./assets/fonts/verdana12.bin"
local DEFAULT_FONT_TEXTURE = "./assets/fonts/verdana12.dds"
EDITOR_LISTBOX_ITEM_HEIGHT = 24
EDITOR_LISTBOX_SCROLLBAR_WIDTH = 12
EDITOR_LISTBOX_SCROLLBAR_HEIGHT = 48

EditorListbox =
{
	fontIndex = -1,
	textureIndex = -1,

	visible = false,
	position = Vec2.create({0,0}),
	size = Vec2.create({0,0}),
	depth = 0,
	itemSize = Vec2.create({0,0}),
	backgroundColor = Vec4.create({0.25, 0.25, 0.25, 1.0}),
	itemColor = Vec4.create({0.5, 0.5, 0.5, 1.0}),
	itemHoverColor = Vec4.create({0.75, 0.75, 0.75, 1.0}),
	itemPressColor = Vec4.create({0.4, 0.4, 0.4, 1.0}),
	textColor = Vec4.create({1,1,1,1}),

	scrollbar =
	{
		position = Vec2.create({0,4}),
		size = Vec2.create({EDITOR_LISTBOX_SCROLLBAR_WIDTH, EDITOR_LISTBOX_SCROLLBAR_HEIGHT}),
		depth = 0,
		color = Vec4.create({ 0.5, 0.5, 0.5, 1.0 }),
		hoverColor = Vec4.create({ 0.75, 0.75, 0.75, 1.0 }),
		pressColor = Vec4.create({ 0.4, 0.4, 0.4, 1.0 }),
		captured = false,
		captureOffset = Vec2.create({0,0}),
	},

	gutter =
	{
		position = Vec2.create({0,4}),
		size = Vec2.create({EDITOR_LISTBOX_SCROLLBAR_WIDTH, 0}),
		depth = 0,

		color = Vec2.create({ 0.15, 0.15, 0.15, 1.0 }),
	},

	padding = 4,
	items = {},
	itemOffset = 0,
	itemCaptured = -1,
	visibleItems = 0,

	onItemSelected = nil,
}

function EditorListbox.create( position, size )
	if EditorListbox.fontIndex < 0 then
		EditorListbox.fontIndex = Assets.loadFont( DEFAULT_FONT_INFO, DEFAULT_FONT_TEXTURE )
		EditorListbox.textureIndex = Assets.loadTexture( DEFAULT_TEXTURE )
	end

	local result =
	{
		--position = tableVal( position ),
		position = position and position:copy() or Vec2.create({0,0}),
		--size = tableVal( size ),
		size = size and size:copy() or Vec2.create({0,0}),
		depth = 0,
		itemSize = Vec2.create({0,EDITOR_LISTBOX_ITEM_HEIGHT}),
		visible = true,
		items = {},
		itemOffset = 0,
		visibleItems = 0,
		padding = 4,

		scrollbar =
		{
			position = Vec2.create({0,4}),
			size = Vec2.create({EDITOR_LISTBOX_SCROLLBAR_WIDTH, EDITOR_LISTBOX_SCROLLBAR_HEIGHT}),
			depth = 0,
			captured = false,
			captureOffset = Vec2.create({0,0}),
			color = Vec4.create({ 0.5, 0.5, 0.5, 1.0 }),
			hoverColor = Vec4.create({ 0.75, 0.75, 0.75, 1.0 }),
			pressColor = Vec4.create({ 0.4, 0.4, 0.4, 1.0 }),
		},

		gutter =
		{
			position = Vec4.create({0,4}),
			size = Vec4.create({EDITOR_LISTBOX_SCROLLBAR_WIDTH, 0}),
			depth = 0,
			color = Vec4.create({ 0.15, 0.15, 0.15, 1.0 }),
		},
	}

	result.itemSize[1] = result.size[1]-result.padding*2

	setmetatable( result, { __index = EditorListbox } )

	return result
end

function EditorListbox.createWithHeight( height )
	assert( isnumber( height ), "Height must be a number." )

	return EditorListbox.create( nil, Vec2.create({0, height}) )
end

function EditorListbox:setPosition( position )
	self.position = position:copy()

	self:calculateItemOffset()
end

function EditorListbox:setSize( size )
	self.size = size:copy()

	self.itemSize[1] = self.size[1] - EDITOR_LISTBOX_SCROLLBAR_WIDTH - self.padding*3
	self.scrollbar.position[1] = self.size[1] - self.scrollbar.size[1] - self.padding
	self.gutter.position[1] = self.size[1] - self.scrollbar.size[1] - self.padding
	self.gutter.size[2] = self.size[2] - self.padding*2

	self:calculateItemOffset()
end

function EditorListbox:setDepth( depth )
	self.depth = depth
	self.gutter.depth = depth + GUI_DEPTH_SMALL_INC
	self.scrollbar.depth = self.gutter.depth + GUI_DEPTH_SMALL_INC
end

function EditorListbox:addItem( text, tag )
	local item =
	{
		text = text,
		tag = tag,
		hovered = false,
		pressed = false,
	}
	self.items[#self.items+1] = item

	self:calculateItemOffset()
end

function EditorListbox:getItemPosition( index )
	local position = Vec2.create({ self.position[1] + self.padding, self.position[2] + self.padding + ((index-1)*(EDITOR_LISTBOX_ITEM_HEIGHT+self.padding)) })

	return position
end

function EditorListbox:calculateItemOffset()
	self.visibleItems = math.floor( self.gutter.size[2] / self.itemSize[2] )
	local excessItems = #self.items - self.visibleItems
	if excessItems > 0 then
		local chunk = (self.gutter.size[2] - self.scrollbar.size[2]) / excessItems;

		local offset = math.floor( self.scrollbar.position[2] / chunk )

		self.itemOffset = offset
	end

	if self.visibleItems > #self.items then
		self.visibleItems = #self.items
	end

	if self.itemOffset < 0 then
		self.itemOffset = 0
	end
end

function EditorListbox:checkCapture( capture, mousePosition )
	if capture.depth < self.depth + GUI_DEPTH_SMALL_INC*2 then
		if insideRect( self.position, self.size, mousePosition ) then
			-- check against items
			for i=1, self.visibleItems do
				local position = self:getItemPosition( i )
				local itemIndex = i + self.itemOffset

				if insideRect( position, self.itemSize, mousePosition ) then
					capture.depth = self.depth + GUI_DEPTH_SMALL_INC*2
					capture.item = self
					break
				end
			end

			-- if not items captured, check against background
			if capture.depth < self.depth then
				capture.depth = self.depth
				capture.item = self
			end
		end
	end
end

function EditorListbox:updateMouseInput( deltaTime, mousePosition )
	if self.scrollbar.captured then
		local localMousePosition = subVec( mousePosition, self.position )
		self.scrollbar.position[2] = localMousePosition[2] - self.scrollbar.captureOffset[2]

		-- make sure scrollbar is inside the gutter
		local miny = self.gutter.position[2]
		local maxy = miny + self.gutter.size[2] - self.scrollbar.size[2]

		if self.scrollbar.position[2] < miny then
			self.scrollbar.position[2] = miny
		elseif self.scrollbar.position[2] > maxy then
			self.scrollbar.position[2] = maxy
		end

		-- update items
		self:calculateItemOffset()
	elseif self.itemCaptured > 0 then
		local position = self:getItemPosition( self.itemCaptured - self.itemOffset )

		self.items[self.itemCaptured].pressed = insideRect( position, self.itemSize, mousePosition )
	end
end

function EditorListbox:press( mousePosition )
	-- check against scrollbar
	local localMousePosition = subVec( mousePosition, self.position )
	if insideRect( self.scrollbar.position, self.scrollbar.size, localMousePosition ) then
		self.scrollbar.captured = true
		self.scrollbar.captureOffset = subVec( localMousePosition, self.scrollbar.position )
	else
	-- check against items
		for i=1, self.visibleItems do
			local position = self:getItemPosition( i )
			local itemIndex = i + self.itemOffset

			if insideRect( position, self.itemSize, mousePosition ) then
				self.items[itemIndex].hovered = true
				self.itemCaptured = itemIndex
			else
				self.items[itemIndex].hovered = false
			end
		end
	end
end

function EditorListbox:release( mousePosition )
	if self.scrollbar.captured then
		self.scrollbar.captured = false
	elseif self.itemCaptured > 0 then
		local position = self:getItemPosition( self.itemCaptured - self.itemOffset )
		
		if insideRect( position, self.itemSize, mousePosition ) then
			if self.onItemSelected then
				self:onItemSelected( self.items[self.itemCaptured] )
			end
		end

		self.items[self.itemCaptured].hovered = false
		self.items[self.itemCaptured].pressed = false
	end
end

function EditorListbox:update( deltaTime, mousePosition )
	if insideRect( self.position, self.size, mousePosition ) then
		for i=1, self.visibleItems do
			local position = self:getItemPosition( i )
			local itemIndex = i + self.itemOffset

			if insideRect( position, self.itemSize, mousePosition ) then
				self.items[itemIndex].hovered = true
			else
				self.items[itemIndex].hovered = false
			end
		end
	end
end

function EditorListbox:render()
	-- render background
	Graphics.queueQuad( self.textureIndex, self.position, self.size, self.depth, self.backgroundColor )

	-- render items
	for i=1, self.visibleItems do
		local position = self:getItemPosition( i )

		local itemIndex = i + self.itemOffset
		local itemColor = self.itemColor
		if self.items[itemIndex].pressed then
			itemColor = self.itemPressColor
		elseif self.items[itemIndex].hovered then
			itemColor = self.itemHoverColor
		end

		Graphics.queueQuad( self.textureIndex, position, self.itemSize, self.depth + GUI_DEPTH_SMALL_INC, itemColor )

		position[1] = position[1] + self.padding
		Graphics.queueText( self.fontIndex, self.items[itemIndex].text, position, self.depth + GUI_DEPTH_SMALL_INC*2, self.textColor )
	end

	-- render gutter
	local position = { self.position[1] + self.gutter.position[1], self.position[2] + self.gutter.position[2] }
	Graphics.queueQuad( self.textureIndex, position, self.gutter.size, self.gutter.depth, self.gutter.color )

	-- render scrollbar
	local position = { self.position[1] + self.scrollbar.position[1], self.position[2] + self.scrollbar.position[2] }
	Graphics.queueQuad( self.textureIndex, position, self.scrollbar.size, self.scrollbar.depth, self.scrollbar.color )
end