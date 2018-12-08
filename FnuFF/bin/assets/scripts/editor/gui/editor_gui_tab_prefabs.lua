local padding = 4
GUI_PREFABS_EDIT_PANEL_WIDTH = 128

local prefabs = 
{
	textureIndex = -1,

	position = Vec2.create({0,0}),
	size = Vec2.create({0,0}),
	depth = 0,
	selectedIndex = 0,

	items = {},

	listPosition = Vec2.create({0,0}),
	listSize = Vec2.create({0,0}),
	listColor = Vec4.create({0.35, 0.35, 0.35, 1.0}),
	prefabItems = {},

	editPanelPosition = Vec2.create({0,0}),
	editPanelSize = Vec2.create({0,0}),
	editPanelColor = Vec4.create({0.4, 0.4, 0.4, 1.0}),
	editPanelVisible = false,
	editPanelComponentsLabel = {},
	editPanelItems = {},

	onSelect = nil,
}

function prefabs:load( position, size, depth )
	self.textureIndex = Assets.loadTexture( GUI_DEFAULT_BACKGROUND_TEXTURE )

	--copyVec( position, self.position )
	--copyVec( size, self.size )
	self.position = position:copy()
	self.size = size:copy()
	self.depth = depth + GUI_DEPTH_INC

	local yoffset = self.position[2] + padding

	-- name
	local nameInputbox = EditorInputbox.create( Vec2.create({self.position[1] + padding, yoffset}), self.size[1]-padding*2, "Name:" )
	nameInputbox:setDepth( self.depth + GUI_DEPTH_SMALL_INC )
	nameInputbox.textbox.readOnly = true
	self.items[#self.items+1] = nameInputbox
	yoffset = yoffset + nameInputbox.size[2] + padding

	-- edit
	local editButton = EditorButton.create( Vec2.create({self.position[1] + padding, yoffset}), Vec2.create({self.size[1] - padding*2, GUI_BUTTON_HEIGHT}), "Edit" )
	editButton:setDepth( self.depth + GUI_DEPTH_SMALL_INC )
	editButton.disabled = true
	editButton.onClick = function( button )
		self:showEditPanel()
	end
	self.items[#self.items+1] = editButton
	yoffset = yoffset + editButton.size[2] + padding

	-- prefabs
	local componentsLabel = EditorLabel.create( Vec2.create({self.position[1] + padding, yoffset}), Vec2.create({self.size[1] - padding*2, GUI_BUTTON_HEIGHT}), "Prefabs:" )
	componentsLabel:setDepth( self.depth + GUI_DEPTH_SMALL_INC )
	self.items[#self.items+1] = componentsLabel
	yoffset = yoffset + componentsLabel.size[2] + padding

	-- list
	self.listPosition = Vec2.create({self.position[1]+padding, yoffset})
	self.listSize = Vec2.create({self.size[1]-padding*2, self.size[2] - yoffset - padding})

	-- edit panel
	self.editPanelPosition = Vec2.create({WINDOW_WIDTH - GUI_PANEL_WIDTH - GUI_PREFABS_EDIT_PANEL_WIDTH, GUI_MENU_HEIGHT})
	self.editPanelSize = Vec2.create({ GUI_PREFABS_EDIT_PANEL_WIDTH, WINDOW_HEIGHT - GUI_MENU_HEIGHT })

	self.editPanelComponentsLabel = EditorLabel.create
	(
		Vec2.create({self.editPanelPosition[1] + padding, self.editPanelPosition[2] + padding }),
		Vec2.create({ GUI_PREFABS_EDIT_PANEL_WIDTH, GUI_BUTTON_HEIGHT}),
		"Components:"
	)
	self.editPanelComponentsLabel:setDepth( self.depth + GUI_DEPTH_SMALL_INC )
end

function prefabs:showEditPanel()
	-- clear edit panel items
	local count = #self.editPanelItems
	for i=1, count do
		self.editPanelItems[i] = nil
	end

	-- add current items
	local yoffset = self.editPanelComponentsLabel.position[2] + self.editPanelComponentsLabel.size[2] + padding
	local button = self.prefabItems[self.selectedIndex]
	for _,v in pairs(button.tag.components) do
		local componentButton = EditorButton.create
		(
			Vec2.create({self.editPanelPosition[1] + padding, yoffset}),
			Vec2.create({self.editPanelSize[1]-padding*2, GUI_BUTTON_HEIGHT}),
			v.name
		)
		componentButton:setDepth( self.depth + GUI_DEPTH_SMALL_INC )
		componentButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
		self.editPanelItems[#self.editPanelItems+1] = componentButton
		yoffset = yoffset + componentButton.size[2] + padding
	end

	-- show edit panel
	self.editPanelVisible = true
end

function prefabs:onClick( button )
	self.selectedIndex = button.index
	self.items[1].textbox:setText( button.tag.name )
	self.items[2].disabled = false
	self.editPanelVisible = false

	if self.onSelect then
		self.onSelect( button )
	end
end

function prefabs:addPrefab( prefab )
	local yoffset = #self.prefabItems * (GUI_BUTTON_HEIGHT + padding)

	local button = EditorButton.create
	(
		Vec2.create({self.listPosition[1] + padding, self.listPosition[2] + padding + yoffset}),
		Vec2.create({self.listSize[1] - padding*2, GUI_BUTTON_HEIGHT}),
		prefab.name
	)
	button:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	button:setDepth( self.depth + GUI_DEPTH_SMALL_INC )
	button.tag = prefab
	button.onClick = function( button )
		self:onClick( button )
	end

	local index = #self.prefabItems+1
	button.index = index

	self.prefabItems[index] = button
end

function prefabs:removePrefab( prefab )
	local index = 0
	for i=1, #self.prefabItems do
		if self.prefabItems[i].tag == entity then
			index = i
			break
		end
	end

	if index > 0 then
		self.prefabItems[index] = nil
	end
end

function prefabs:clear()
	local count = #self.prefabItems
	for i=1, count do
		self.prefabItems[i] = nil
	end
end

function prefabs:checkCapture( capture, mousePosition )
	-- check items
	for _,v in pairs(self.items) do
		v:checkCapture( capture, mousePosition )
	end

	-- check prefab items
	for _,v in pairs(self.prefabItems) do
		v:checkCapture( capture, mousePosition )
	end

	-- check edit panel
	if self.editPanelVisible then
		for _,v in pairs(self.editPanelItems) do
			v:checkCapture( capture, mousePosition )
		end
	end

	if capture.depth < self.depth then
		if insideRect( self.position, self.size, mousePosition ) then
			capture.depth = self.depth
			capture.item = self
		end
	end
end

function prefabs:update( deltaTime, mousePosition )
	-- update items
	for _,v in pairs(self.items) do
		v:update( deltaTime, mousePosition )
	end

	-- update prefab items
	for _,v in pairs(self.prefabItems) do
		v:update( deltaTime, mousePosition )
	end

	-- update edit panel
	if self.editPanelVisible then
		for _,v in pairs(self.editPanelItems) do
			v:update( deltaTime, mousePosition )
		end
	end
end

function prefabs:render()
	-- render items
	for _,v in pairs(self.items) do
		v:render()
	end

	-- render prefab list background
	Graphics.queueQuad( self.textureIndex, self.listPosition, self.listSize, self.depth, self.listColor )

	-- render prefab items
	for _,v in pairs(self.prefabItems) do
		v:render()
	end

	-- render edit panel
	if self.editPanelVisible then
		-- render background
		Graphics.queueQuad( self.textureIndex, self.editPanelPosition, self.editPanelSize, self.depth, self.editPanelColor )

		-- render components label
		self.editPanelComponentsLabel:render()

		-- render items
		for _,v in pairs(self.editPanelItems) do
			v:render()
		end
	end
end

return prefabs