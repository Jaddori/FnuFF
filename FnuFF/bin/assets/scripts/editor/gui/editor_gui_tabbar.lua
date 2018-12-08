GUI_TAB_BUTTON_WIDTH = 64

local bar = 
{
	items = {},
	currentTab = -1,
	selectionColor = Vec4.create({0.75, 0.75, 0.0, 1.0}),
	depth = 0,

	onTabChanged = nil,
}

function bar:onClick( index )
	if index ~= self.currentTab then
		-- reset background color
		if self.currentTab > 0 then
			self.items[self.currentTab].color = nil
		end

		self.currentTab = index
		self.items[self.currentTab].color = self.selectionColor

		if self.onTabChanged then
			self.onTabChanged( index )
		end
	end
end

function bar:load( position, depth )
	local xoffset = position[1]
	local yoffset = position[2]

	self.depth = depth + GUI_DEPTH_INC

	-- info
	local infoButton = EditorButton.create( Vec2.create({xoffset, yoffset}), Vec2.create({ GUI_TAB_BUTTON_WIDTH, GUI_BUTTON_HEIGHT }), "Info" )
	infoButton.depth = self.depth
	infoButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	infoButton.onClick = function( button )
		self:onClick( 1 )
	end
	self.items[#self.items+1] = infoButton
	xoffset = xoffset + GUI_TAB_BUTTON_WIDTH

	-- entities
	local entitiesButton = EditorButton.create( Vec2.create({xoffset, yoffset}), Vec2.create({GUI_TAB_BUTTON_WIDTH, GUI_BUTTON_HEIGHT}), "Entities" )
	entitiesButton.depth = self.depth
	entitiesButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	entitiesButton.onClick = function( button )
		self:onClick( 2 )
	end
	self.items[#self.items+1] = entitiesButton
	xoffset = xoffset + GUI_TAB_BUTTON_WIDTH

	-- prefabs
	local prefabsButton = EditorButton.create( Vec2.create({xoffset, yoffset}), Vec2.create({GUI_TAB_BUTTON_WIDTH, GUI_BUTTON_HEIGHT}), "Prefabs" )
	prefabsButton.depth = self.depth
	prefabsButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	prefabsButton.onClick = function( button )
		self:onClick( 3 )
	end
	self.items[#self.items+1] = prefabsButton
	xoffset = xoffset + GUI_TAB_BUTTON_WIDTH

	self:onClick( 1 )
end

function bar:checkCapture( capture, mousePosition )
	for _,v in pairs(self.items) do
		v:checkCapture( capture, mousePosition )
	end
end

function bar:update( deltaTime, mousePosition )
	for _,v in pairs(self.items) do
		v:update( deltaTime, mousePosition )
	end
end

function bar:render()
	for _,v in pairs(self.items) do
		v:render()
	end
end

return bar