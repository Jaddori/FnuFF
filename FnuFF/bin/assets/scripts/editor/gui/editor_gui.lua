GUI_DEFAULT_BACKGROUND_TEXTURE = "./assets/textures/white.dds"
GUI_DEFAULT_FONT_INFO = "./assets/fonts/verdana12.bin"
GUI_DEFAULT_FONT_TEXTURE = "./assets/fonts/verdana12.dds"
GUI_DEPTH_INC = 0.01
GUI_DEPTH_SMALL_INC = 0.001

GUI_BUTTON_HEIGHT = 0
GUI_WINDOW_BASE_DEPTH = 0.0

local gui =
{
	fontIndex = -1,
	fontHeight = 0,
	
	menu = {},	
	panel =	{},
	componentList = {},
	contextMenu = {},

	openWindows = {},
}

-- windows
function gui:focusWindow( window )
	local openWindows = {}
	if window and window.visible then
		openWindows[1] = window
	end

	for i=1, #self.openWindows do
		if self.openWindows[i] ~= window and self.openWindows[i].visible then
			openWindows[#openWindows+1] = self.openWindows[i]
		end
	end

	for i=1, #openWindows do
		openWindows[i]:setDepth( GUI_WINDOW_BASE_DEPTH - GUI_DEPTH_INC*i )
	end

	self.openWindows = openWindows
end

function gui:load()
	-- load controls
	local controlFiles = Filesystem.getFiles( "./assets/scripts/editor/controls/*" )
	for _,v in pairs(controlFiles) do
		doscript( "editor/controls/" .. v )
	end
	
	self.fontIndex = Assets.loadFont( "./assets/fonts/verdana12.bin", "./assets/fonts/verdana12.dds" )
	local font = Assets.getFont( self.fontIndex )
	self.fontHeight = font:getHeight()
	
	GUI_BUTTON_HEIGHT = self.fontHeight + 4 -- 2 pixels of padding at top and bottom
	GUI_MENU_HEIGHT = GUI_BUTTON_HEIGHT
	
	-- create menu
	self.menu = doscript( "editor/gui/editor_gui_menu.lua" )
	self.menu:load()
	
	-- create panel
	self.panel = doscript( "editor/gui/editor_gui_panel.lua" )
	self.panel:load()
	self.panel.tabs[GUI_TAB_INFO].onAddComponent = function()
		self.componentList.visible = not self.componentList.visible
	end

	-- create component list
	self.componentList = doscript( "editor/gui/editor_gui_component_list.lua" )
	self.componentList:load()

	-- create context menu
	self.contextMenu = doscript( "editor/gui/editor_gui_context_menu.lua" )
	self.contextMenu:load()
end

function gui:checkCapture( capture, mousePosition )
	-- menu
	self.menu:checkCapture( capture, mousePosition )

	-- panel
	self.panel:checkCapture( capture, mousePosition )

	-- component list
	if self.componentList.visible then
		self.componentList:checkCapture( capture, mousePosition )
	end

	-- context menu
	self.contextMenu:checkCapture( capture, mousePosition )

	-- component info windows
	for _,v in pairs(Entity.windowList) do
		v.window:checkCapture( capture, mousePosition )
	end

	if capture.depth < self.panel.depth then
		if insideRect( self.panel.position, self.panel.size, mousePosition ) then
			capture.depth = self.panel.depth
		end
	end
end

function gui:update( deltaTime, mousePosition )
	-- menu
	self.menu:update( deltaTime, mousePosition )

	-- panel
	self.panel:update( deltaTime, mousePosition )

	-- component list
	if self.componentList.visible then
		self.componentList:update( deltaTime, mousePosition )
	end

	-- context menu
	if self.contextMenu.visible then
		self.contextMenu:update( deltaTime, mousePosition )
	end

	-- component info windows
	for _,v in pairs(Entity.windowList) do
		v:update( deltaTime, mousePosition )
	end
end

function gui:render()
	-- menu
	self.menu:render()
	
	self.panel:render()
	
	-- component list
	if self.componentList.visible then
		self.componentList:render()
	end

	-- component info windows
	for _,v in pairs(Entity.windowList) do
		v:render()
	end

	-- context menu
	if self.contextMenu.visible then
		self.contextMenu:render()
	end
end

return gui