EditorLayoutHorizontal =
{
	position = Vec2.create({0,0}),
	size = Vec2.create({0,0}),
	padding = 4,
	items = {},
	displayItems = {},
}

function EditorLayoutHorizontal.create( position, width )
	assert( position, "Position was nil." )
	assert( width, "Width was nil." )

	assert( istable( position ), "Position must be a table." )
	assert( isnumber( width ), "Width must be a number." )

	local result =
	{
		--position = tableVal( position ),
		position = position and position:copy() or Vec2.create({0,0}),
		size = Vec2.create({ width or 0, 0 }),
		items = {},
		displayItems = {},
	}

	setmetatable( result, { __index = EditorLayoutHorizontal } )

	return result
end

function EditorLayoutHorizontal:setDepth( depth )
	for _,v in pairs(self.displayItems) do
		v:setDepth( depth )
	end
end

function EditorLayoutHorizontal:addItem( item )
	self.items[#self.items+1] = item

	item.relativeSize = tableVal( item.size )

	-- NOTE: Room for optimization
	self:layout()
end

function EditorLayoutHorizontal:removeItem( item )
	local index = 0
	for i=1, #self.items do
		if self.items[i] == item then
			index = i
			break
		end
	end

	if index > 0 then
		self.items[index] = nil
	end

	self:layout()
end

function EditorLayoutHorizontal:removeAt( index )
	self.items[index] = nil

	self:layout()
end

function EditorLayoutHorizontal:clear()
	local count = #self.items
	for i=1, count do
		self.items[i] = nil
	end

	count = #self.displayItems
	for i=1, count do
		self.displayItems[i] = nil
	end
end

function EditorLayoutHorizontal:layout()
	local count = #self.displayItems
	for i=1, count do
		self.displayItems[i] = nil
	end

	local xoffset = self.position[1]
	local yoffset = self.position[2]

	local itemWidth = self.size[1]
	local relativeItems = #self.items
	for i=1, #self.items do
		if self.items[i].relativeSize[1] > 0 then
			itemWidth = itemWidth - self.items[i].relativeSize[1] - self.padding
			relativeItems = relativeItems - 1
		end
	end

	itemWidth = (itemWidth / relativeItems) - (self.padding * (relativeItems-1))/relativeItems

	local maxHeight = 0
	for i=1, #self.items do
		local position = Vec2.create({xoffset, yoffset})
		self.items[i]:setPosition( position )

		if self.items[i].relativeSize[1] <= 0 then
			local size = Vec2.create({ itemWidth, self.items[i].size[2] })
			self.items[i]:setSize( size )
		end

		xoffset = xoffset + self.items[i].size[1] + self.padding

		if self.items[i].size[2] > maxHeight then
			maxHeight = self.items[i].size[2]
		end

		self.displayItems[#self.displayItems+1] = self.items[i]
	end

	self.size[2] = maxHeight
end

function EditorLayoutHorizontal:setPosition( position )
	--self.position = tableVal( position )
	self.position = position and position:copy() or Vec2.create({0,0})
	self:layout()
end

function EditorLayoutHorizontal:setSize( size )
	--self.size = tableVal( size )
	self.size = size and size:copy() or Vec2.create({0,0})
	self:layout()
end

function EditorLayoutHorizontal:setPadding( padding )
	self.padding = padding
	self:layout()
end

function EditorLayoutHorizontal:checkCapture( capture, mousePosition )
	for _,v in pairs(self.displayItems) do
		v:checkCapture( capture, mousePosition )
	end
end

function EditorLayoutHorizontal:call( func, ... )
	for _,v in pairs(self.displayItems) do
		if v[func] then
			v[func]( v, ... )
		end
	end
end

function EditorLayoutHorizontal:update( deltaTime, mousePosition )
	for _,v in pairs(self.displayItems) do
		v:update( deltaTime, mousePosition )
	end
end

function EditorLayoutHorizontal:render()
	for _,v in pairs(self.displayItems) do
		v:render()
	end
end