GUI_PANEL_WIDTH = 256
GUI_PANEL_DEPTH = 0.5

GUI_TAB_INFO = 1
GUI_TAB_ENTITIES = 2
GUI_TAB_PREFABS = 3

local panel =
{
	textureIndex = -1,
	position = Vec2.create({0,0}),
	size = Vec2.create({0,0}),
	depth = GUI_PANEL_DEPTH,
	color = Vec4.create({0.5, 0.5, 0.5, 1.0}),

	contentPosition = Vec2.create({0,0}),
	contentSize = Vec2.create({0,0}),

	tabBar = {},
	tabs = {},
}

function panel:load()
	self.textureIndex = Assets.loadTexture( GUI_DEFAULT_BACKGROUND_TEXTURE )

	self.position = Vec2.create({ WINDOW_WIDTH - GUI_PANEL_WIDTH, GUI_MENU_HEIGHT })
	self.size = Vec2.create({ GUI_PANEL_WIDTH, WINDOW_HEIGHT - GUI_MENU_HEIGHT })

	self.contentPosition = Vec2.create({ self.position[1], self.position[2] + GUI_BUTTON_HEIGHT })
	self.contentSize = Vec2.create({ self.size[1], self.size[2] - GUI_BUTTON_HEIGHT*2 })

	-- load tab bar
	self.tabBar = doscript( "editor/gui/editor_gui_tabbar.lua" )
	self.tabBar:load( self.position, self.depth )

	-- load tabs
	self.tabs[GUI_TAB_INFO] = doscript( "editor/gui/editor_gui_tab_info.lua" )
	self.tabs[GUI_TAB_INFO]:load( self.contentPosition, self.contentSize, self.depth )

	self.tabs[GUI_TAB_ENTITIES] = doscript( "editor/gui/editor_gui_tab_entities.lua" )
	self.tabs[GUI_TAB_ENTITIES]:load( self.contentPosition, self.contentSize, self.depth )

	self.tabs[GUI_TAB_PREFABS] = doscript( "editor/gui/editor_gui_tab_prefabs.lua" )
	self.tabs[GUI_TAB_PREFABS]:load( self.contentPosition, self.contentSize, self.depth )
end

function panel:checkCapture( capture, mousePosition )
	self.tabBar:checkCapture( capture, mousePosition )
	self.tabs[self.tabBar.currentTab]:checkCapture( capture, mousePosition )
end

function panel:update( deltaTime, mousePosition )
	self.tabBar:update( deltaTime, mousePosition )
	self.tabs[self.tabBar.currentTab]:update( deltaTime, mousePosition )
end

function panel:render()
	-- render background
	Graphics.queueQuad( self.textureIndex, self.position, self.size, self.depth, self.color )

	-- render tab bar
	self.tabBar:render()

	-- render tabs
	self.tabs[self.tabBar.currentTab]:render()
end

return panel