require( "./assets/scripts/states/state_menu" )

GUI_DEFAULT_BACKGROUND_TEXTURE = "./assets/textures/white.dds"
GUI_DEFAULT_FONT_INFO = "./assets/fonts/verdana12.bin"
GUI_DEFAULT_FONT_TEXTURE = "./assets/fonts/verdana12.dds"
GUI_DEPTH_INC = 0.01
GUI_DEPTH_SMALL_INC = 0.001

GUI_BUTTON_HEIGHT = 24
GUI_WINDOW_BASE_DEPTH = 0.0

StateMainMenu = StateMenu.create( "MainMenu" )

function StateMainMenu:load()
	self.titleLabel = EditorLabel.create( Vec2.create(), Vec2.create({WINDOW_WIDTH, 128}), "Triadic" )
	self.titleLabel:loadFont( "./assets/fonts/verdana18.bin", "./assets/fonts/verdana18.dds" )
	self.titleLabel:setTextAlignment( ALIGN_MIDDLE, ALIGN_MIDDLE )

	self.playButton = EditorButton.create( Vec2.create({32,128}), Vec2.create({128,24}), "Play" )
	self.playButton.onClick = function( button )
		Game:pushState( "PlayMenu" )
	end

	self.editorButton = EditorButton.create( Vec2.create({32,128+48}), Vec2.create({128,24}), "Editor" )

	self.quitButton = EditorButton.create( Vec2.create({32,256+32}), Vec2.create({128,24}), "Quit" )
	self.quitButton.onClick = function( button )
		Core.exit()
	end

	self:addControl( self.titleLabel )
	self:addControl( self.playButton )
	self:addControl( self.editorButton )
	self:addControl( self.quitButton )
end

Game:addState( StateMainMenu )
Game:setState( "MainMenu" )