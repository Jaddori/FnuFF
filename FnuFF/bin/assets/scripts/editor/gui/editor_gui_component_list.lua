GUI_COMPONENT_LIST_WIDTH = 128
GUI_COMPONENT_LIST_DEPTH = 0.7

local list =
{
	textureIndex = -1,
	position = Vec2.create({0,0}),
	size = Vec2.create({0,0}),
	depth = GUI_COMPONENT_LIST_DEPTH,
	color = Vec2.create({0.35, 0.35, 0.35, 1.0}),
	visible = false,

	items = {},

	onClick = nil,
}

function list:load()
	self.textureIndex = Assets.loadTexture( GUI_DEFAULT_BACKGROUND_TEXTURE )

	self.position = Vec2.create({WINDOW_WIDTH - GUI_PANEL_WIDTH - GUI_COMPONENT_LIST_WIDTH, GUI_MENU_HEIGHT})
	self.size = Vec2.create({GUI_COMPONENT_LIST_WIDTH, WINDOW_HEIGHT-GUI_MENU_HEIGHT})
end

function list:addItem( text, tag )
	local count = #self.items

	local padding = 4

	local button = EditorButton.create
	(
		Vec2.create({self.position[1]+padding, self.position[2]+padding+count*(GUI_BUTTON_HEIGHT+padding)}),
		Vec2.create({self.size[1]-padding*2, GUI_BUTTON_HEIGHT}),
		text
	)
	button:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	button:setDepth( self.depth + GUI_DEPTH_SMALL_INC )
	button.tag = tag
	button.index = count + 1
	button.onClick = function( button )
		if self.onClick then
			self.onClick( button )
		end

		self.visible = false
	end

	self.items[count+1] = button
	
	return button
end

function list:checkCapture( capture, mousePosition )
	for _,v in pairs(self.items) do
		v:checkCapture( capture, mousePosition )
	end

	if capture.depth < self.depth then
		if insideRect( self.position, self.size, mousePosition ) then
			capture.depth = self.depth
			capture.item = self
		end
	end
end

function list:update( deltaTime, mousePosition )
	for _,v in pairs(self.items) do
		v:update( deltaTime, mousePosition )
	end
end

function list:render()
	-- render background
	Graphics.queueQuad( self.textureIndex, self.position, self.size, self.depth, self.color )

	-- render items
	for _,v in pairs(self.items) do
		v:render()
	end
end

return list