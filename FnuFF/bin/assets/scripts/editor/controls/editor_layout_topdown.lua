EditorLayoutTopdown =
{
	position = Vec2.create({0,0}),
	size = Vec2.create({0,0}),
	padding = 4,
	items = {},
	displayItems = {},
}

function EditorLayoutTopdown.create( position, width )
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

	setmetatable( result, { __index = EditorLayoutTopdown } )

	return result
end

function EditorLayoutTopdown:setDepth( depth )
	for _,v in pairs(self.displayItems) do
		v:setDepth( depth )
	end
end

function EditorLayoutTopdown:addItem( item )
	self.items[#self.items+1] = item

	if #item == 0 then
		item.relativePosition = tableVal( item.position )
	else
		for _,v in pairs(item) do
			v.relativePosition = tableVal( v.position )
		end
	end

	--NOTE: Room for optimization here
	self:layout()
end

function EditorLayoutTopdown:removeItem( item )
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

function EditorLayoutTopdown:removeAt( index )
	self.items[index] = nil

	self:layout()
end

function EditorLayoutTopdown:clear()
	local count = #self.items
	for i=1, count do
		self.items[i] = nil
	end

	count = #self.displayItems
	for i=1, count do
		self.displayItems[i] = nil
	end
end

function EditorLayoutTopdown:layout()
	local count = #self.displayItems
	for i=1, count do
		self.displayItems[i] = nil
	end

	local xoffset = self.position[1] + self.padding
	local yoffset = self.position[2] + self.padding
	local itemWidth = self.size[1] - self.padding*2

	for i=1, #self.items do
		if #self.items[i] == 0 then
			local position = Vec2.create({ xoffset, yoffset })
			self.items[i]:setPosition( position )

			local size = Vec2.create({ itemWidth, self.items[i].size[2] })
			if self.items[i].size[1] <= 0 then
				self.items[i]:setSize( size )
			end

			yoffset = yoffset + size[2] + self.padding

			self.displayItems[#self.displayItems+1] = self.items[i]
		else
			local fixedWidth = 0
			for j=1, #self.items[i] do
				if self.items[i][j].size[1] > 0 then
					local itemPadding = self.padding
					if j > 1 then
						itemPadding = itemPadding * 2
					end

					fixedWidth = fixedWidth + self.items[i][j].size[1] + itemPadding
				end
			end

			local xposition = xoffset
			local maxHeight = 0
			for j=1, #self.items[i] do
				-- set position
				local position = Vec2.create({ xposition, yoffset })
				if self.items[i][j].position[2] > 0 then
					position[2] = position[2] + self.items[i][j].relativePosition[2]
				end
				self.items[i][j]:setPosition( position )
				
				-- set size
				local size = Vec2.create({ itemWidth - fixedWidth, self.items[i][j].size[2] })
				if self.items[i][j].size[1] <= 0 then
					self.items[i][j]:setSize( size )
				end

				xposition = xposition + size[1] + self.padding
				if self.items[i][j].size[2] > maxHeight then
					maxHeight = self.items[i][j].size[2]
				end

				self.displayItems[#self.displayItems+1] = self.items[i][j]
			end

			yoffset = yoffset + maxHeight + self.padding
		end
	end

	self.size[2] = yoffset - self.position[2]
end

function EditorLayoutTopdown:setPosition( position )
	--self.position = tableVal( position )
	self.position = position and position:copy() or Vec2.create({0,0})
	self:layout()
end

function EditorLayoutTopdown:setSize( size )
	--self.size = tableVal( size )
	self.size = size and size:copy() or Vec2.create({0,0})
	self:layout()
end

function EditorLayoutTopdown:setPadding( padding )
	self.padding = padding
	self:layout()
end

function EditorLayoutTopdown:checkCapture( capture, mousePosition )
	for _,v in pairs(self.displayItems) do
		v:checkCapture( capture, mousePosition )
	end
end

function EditorLayoutTopdown:call( func, ... )
	for _,v in pairs(self.displayItems) do
		if v[func] then
			v[func]( v, ... )
		end
	end
end

function EditorLayoutTopdown:update( deltaTime, mousePosition )
	for _,v in pairs(self.displayItems) do
		v:update( deltaTime, mousePosition )
	end
end

function EditorLayoutTopdown:render()
	for _,v in pairs(self.displayItems) do
		v:render()
	end
end