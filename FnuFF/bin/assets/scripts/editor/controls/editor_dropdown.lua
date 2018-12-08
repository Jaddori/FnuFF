local DEFAULT_ARROW_DOWN_TEXTURE = "./assets/textures/editor/dropdown_arrow_down.dds"
local DEFAULT_ARROW_UP_TEXTURE = "./assets/textures/editor/dropdown_arrow_up.dds"
local DEFAULT_ARROW_SIZE = 16

EditorDropdown = 
{
	fontIndex = -1,
	fontHeight = -1,
	textureIndex = -1,
	position = Vec2.create({0,0}),
	size = Vec2.create({0,0}),
	depth = 0,
	color = Vec4.create({0.5, 0.5, 0.5, 1.0}),
	hoverColor = Vec4.create({0.75, 0.75, 0.75, 1.0}),
	pressColor = Vec4.create({0.35, 0.35, 0.35, 1.0}),
	textColor = Vec4.create({1,1,1,1}),
	expanded = false,
	hovered = false,
	pressed = false,
	selectedIndex = 0,
	capturedIndex = -1,

	-- arrow
	arrowDownTextureIndex = -1,
	arrowUpTextureIndex = -1,
	arrowPosition = Vec2.create({0,0}),
	arrowSize = Vec2.create({DEFAULT_ARROW_SIZE, DEFAULT_ARROW_SIZE}),
	arrowColor = Vec4.create({1,1,1,1}),

	-- menu
	menu =
	{
		itemSize = 0,
		items = {},
	},

	onItemSelected = nil,
}

function EditorDropdown.create( position, size )
	if EditorDropdown.textureIndex < 0 then
		EditorDropdown.textureIndex = Assets.loadTexture( GUI_DEFAULT_BACKGROUND_TEXTURE )
	end

	if EditorDropdown.arrowDownTextureIndex < 0 then
		EditorDropdown.arrowDownTextureIndex = Assets.loadTexture( DEFAULT_ARROW_DOWN_TEXTURE )
		EditorDropdown.arrowUpTextureIndex = Assets.loadTexture( DEFAULT_ARROW_UP_TEXTURE )
	end

	if EditorDropdown.fontIndex < 0 then
		EditorDropdown.fontIndex = Assets.loadFont( GUI_DEFAULT_FONT_INFO, GUI_DEFAULT_FONT_TEXTURE )
		EditorDropdown.fontHeight = Assets.getFont( EditorDropdown.fontIndex ):getHeight()
	end

	local result = 
	{
		--position = tableVal( position ),
		position = position and position:copy() or Vec2.create({0,0}),
		--size = tableVal( size ),
		size = size and size:copy() or Vec2.create({0,0}),
		depth = 0,
		hovered = false,
		pressed = false,
		expanded = false,
		selectedIndex = 0,

		arrowPosition = Vec2.create({0,0}),

		menu = 
		{
			itemSize = 0,
			items = {},
		},
	}

	setmetatable( result, { __index = EditorDropdown } )

	return result
end

function EditorDropdown.createDefault()
	return EditorDropdown.create( nil, Vec2.create({ 0, GUI_BUTTON_HEIGHT }) )
end

function EditorDropdown:expand( expanded )
	self.expanded = expanded or not self.expanded
end

function EditorDropdown:addItem( text, tag )
	local index = #self.menu.items+1
	self.menu.items[index] = 
	{
		text = text,
		tag = tag,
		hovered = false,
		pressed = false,
		index = index,
	}

	local font = Assets.getFont( self.fontIndex )
	local textWidth = font:measureText( text )[1]

	if textWidth > self.menu.itemSize then
		self.menu.itemSize = textWidth
	end

	self.selectedIndex = 1
end

function EditorDropdown:updateArrowPosition()
	local arrowPadding = (self.size[2] - self.arrowSize[2]) * 0.5
	self.arrowPosition[1] = self.position[1] + self.size[1] - self.arrowSize[1] - arrowPadding
	self.arrowPosition[2] = self.position[2] + arrowPadding
end

function EditorDropdown:setPosition( position )
	self.position = position:copy()

	self:updateArrowPosition()
end

function EditorDropdown:setSize( size )
	self.size = size:copy()

	self:updateArrowPosition()
end

function EditorDropdown:setDepth( depth )
	self.depth = depth
end

function EditorDropdown:checkCapture( capture, mousePosition )
	if capture.depth < self.depth + GUI_DEPTH_SMALL_INC*4 then
		if self.expanded then
			local position = Vec2.create({self.position[1], self.position[2] + self.size[2]})
			local size = self.size:copy()
			if self.menu.itemSize > size[1] then
				size[1] = self.menu.itemSize
			end

			for _,v in pairs(self.menu.items) do
				if insideRect( position, size, mousePosition ) then
					capture.depth = self.depth + GUI_DEPTH_SMALL_INC*4
					capture.item = self
					--self.capturedIndex = v.index
					break
				end

				position[2] = position[2] + size[2]
			end
		end

		if capture.depth < self.depth then
			if insideRect( self.position, self.size, mousePosition ) then
				capture.depth = self.depth
				capture.item = self
			end
		end
	end
end

function EditorDropdown:updateMouseInput( deltaTime, mousePosition )
	if self.capturedIndex < 0 then
		self.pressed = insideRect( self.position, self.size, mousePosition )
	else
		local position = Vec2.create({self.position[1], self.position[2] + self.size[2]*self.capturedIndex})
		local size = Vec2.create({ math.max(self.size[1], self.menu.itemSize), self.size[2] })

		self.menu.items[self.capturedIndex].pressed = insideRect( position, size, mousePosition )
	end
end

function EditorDropdown:press( mousePosition )
	if self.expanded then
		local position = Vec2.create({self.position[1], self.position[2] + self.size[2]})
		local size = Vec2.create({math.max(self.size[1], self.menu.itemSize), self.size[2]})

		for _,v in pairs(self.menu.items) do
			if insideRect( position, size, mousePosition ) then
				self.capturedIndex = v.index
				v.hovered = true
			end

			position[2] = position[2] + size[2]
		end
	end
end

function EditorDropdown:release( mousePosition )
	-- check items
	if self.expanded then
		local position = Vec2.create({self.position[1], self.position[2] + self.size[2]*self.capturedIndex})
		local size = Vec2.create({ math.max(self.size[1], self.menu.itemSize), self.size[2] })

		if insideRect( position, size, mousePosition ) then
			self.selectedIndex = self.capturedIndex

			if self.onItemSelected then
				self:onItemSelected( self.menu.items[self.capturedIndex] )
			end

			self.expanded = false
		end
	end

	-- check main box
	if insideRect( self.position, self.size, mousePosition ) then
		self.expanded = not self.expanded
	end

	self.hovered = false
	self.pressed = false
	self.capturedIndex = -1
end

function EditorDropdown:update( deltaTime, mousePosition )
	if self.expanded then
		self.hovered = false

		local position = Vec2.create({self.position[1], self.position[2] + self.size[2]})
		local size = Vec2.create({math.max(self.size[1], self.menu.itemSize), self.size[2]})

		for _,v in pairs(self.menu.items) do
			v.hovered = insideRect( position, size, mousePosition )

			position[2] = position[2] + size[2]
		end
	else
		if insideRect( self.position, self.size, mousePosition ) then
			self.hovered = true
		else
			self.hovered = false
		end
	end
end

function EditorDropdown:render()
	local color = self.color
	if self.pressed then
		color = self.pressColor
	elseif self.hovered or self.expanded then
		color = self.hoverColor
	end

	local textPadding = 8

	-- draw background
	Graphics.queueQuad( self.textureIndex, self.position, self.size, self.depth, color )

	-- draw selected item text
	if self.selectedIndex > 0 then
		local textPosition = Vec2.create({ self.position[1] + textPadding, self.position[2] })
		Graphics.queueText( self.fontIndex, self.menu.items[self.selectedIndex].text, textPosition, self.depth + GUI_DEPTH_SMALL_INC, self.textColor )
	end

	-- draw arrow
	local arrowTextureIndex = self.arrowDownTextureIndex
	if self.expanded then
		arrowTextureIndex = self.arrowUpTextureIndex
	end
	Graphics.queueQuad( arrowTextureIndex, self.arrowPosition, self.arrowSize, self.depth + GUI_DEPTH_SMALL_INC, self.arrowColor )

	-- draw menu
	if self.expanded then
		local position = Vec2.create({ self.position[1], self.position[2] + self.size[2] })
		local size = Vec2.create({ math.max(self.size[1], self.menu.itemSize), self.size[2] })
		
		for _,v in pairs(self.menu.items) do
			local color = self.color
			if v.pressed then
				color = self.pressColor
			elseif v.hovered then
				color = self.hoverColor
			end

			-- draw background
			Graphics.queueQuad( self.textureIndex, position, size, self.depth + GUI_DEPTH_SMALL_INC*3, color )

			-- draw text
			Graphics.queueText( self.fontIndex, v.text, {position[1]+textPadding, position[2]}, self.depth + GUI_DEPTH_SMALL_INC*4, self.textColor )

			position[2] = position[2] + size[2]
		end
	end
end