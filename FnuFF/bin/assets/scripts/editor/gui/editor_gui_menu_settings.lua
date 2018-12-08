local settings =
{
	visible = false,
	items = {},
	settingButton = nil,
	showGridButton = nil,
	showOrigoButton = nil,
	enableLightingButton = nil,
	
	onShowGrid = nil,
	onShowOrigo = nil,
	onEnableLighting = nil,
}

function settings:load( xoffset, items, depth )
	local width = 64
	
	self.settingsButton = EditorButton.create( Vec2.create({xoffset, 0}), Vec2.create({width, GUI_MENU_HEIGHT}), "Settings" )
	self.settingsButton.depth = depth + GUI_DEPTH_INC
	self.settingsButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	self.settingsButton.onClick = function( button )
		self.visible = true
		self.settingsButton.color = {0.4, 0.4, 0.4, 1.0}
	end
	items[#items+1] = self.settingsButton

	local pos = {xoffset, 0}
	local yoffset = GUI_MENU_HEIGHT
	
	self.showGridButton = EditorButton.create( Vec2.create({pos[1], pos[2]+yoffset}), Vec2.create({GUI_MENU_SETTINGS_BUTTON_WIDTH, GUI_BUTTON_HEIGHT}), "Show grid" )
	self.showGridButton.depth = self.settingsButton.depth + GUI_DEPTH_INC
	self.showGridButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	self.showGridButton.onClick = function( button )
		self.visible = false
		
		if self.onShowGrid then
			self.onShowGrid()
		end
	end
	yoffset = yoffset + GUI_BUTTON_HEIGHT
	
	self.showOrigoButton = EditorButton.create( Vec2.create({pos[1], pos[2]+yoffset}), Vec2.create({GUI_MENU_SETTINGS_BUTTON_WIDTH, GUI_BUTTON_HEIGHT}), "Show origo" )
	self.showOrigoButton.depth = self.settingsButton.depth + GUI_DEPTH_INC
	self.showOrigoButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	self.showOrigoButton.onClick = function( button )
		self.visible = false
		
		if self.onShowOrigo then
			self.onShowOrigo()
		end
	end
	yoffset = yoffset + GUI_BUTTON_HEIGHT

	self.enableLightingButton = EditorButton.create( Vec2.create({pos[1], pos[2]+yoffset}), Vec2.create({GUI_MENU_SETTINGS_BUTTON_WIDTH, GUI_BUTTON_HEIGHT}), "Toggle lighting" )
	self.enableLightingButton.depth = self.settingsButton.depth + GUI_DEPTH_INC
	self.enableLightingButton:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	self.enableLightingButton.onClick = function( button )
		self.visible = false

		if self.onEnableLighting then
			self.onEnableLighting()
		end
	end
	yoffset = yoffset + GUI_BUTTON_HEIGHT
	
	self.items[#self.items+1] = self.showGridButton
	self.items[#self.items+1] = self.showOrigoButton
	self.items[#self.items+1] = self.enableLightingButton
	
	return width
end

function settings:checkCapture( capture, mousePosition )
	if self.visible then
		local localCapture =
		{
			depth = capture.depth,
			button = capture.button,
			item = nil,
			focusItem = nil
		}

		for _,v in pairs(self.items) do
			v:checkCapture( localCapture, mousePosition )
		end

		if localCapture.item then
			capture.depth = localCapture.depth
			capture.item = localCapture.item
			capture.focusItem = localCapture.focusItem
		else
			self.visible = false
			self.settingsButton.color = nil
		end
	end
end

function settings:update( deltaTime, mousePosition )
	if self.visible then
		for _,v in pairs(self.items) do
			v:update( deltaTime, mousePosition )
		end
	end
end

function settings:render()
	if self.visible then
		for _,v in pairs(self.items) do
			v:render()
		end
	end
end

return settings