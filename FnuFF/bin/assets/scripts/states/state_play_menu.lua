StatePlayMenu = StateMenu.create( "PlayMenu" )

function StatePlayMenu:load()
	self.titleLabel = EditorLabel.create( Vec2.create(), Vec2.create({WINDOW_WIDTH, 128}), "Triadic" )
	self.titleLabel:loadFont( "./assets/fonts/verdana18.bin", "./assets/fonts/verdana18.dds" )
	self.titleLabel:setTextAlignment( ALIGN_MIDDLE, ALIGN_MIDDLE )

	self.hostButton = EditorButton.create( Vec2.create({32,128}), Vec2.create({128,24}), "Host Game" )
	self.hostButton.onClick = function( button )
		Game:pushState( "Lobby" )
	end

	self.joinButton = EditorButton.create( Vec2.create({32,128+48}), Vec2.create({128,24}), "Join Game" )
	self.joinButton.onClick = function( button )
		Game:pushState( "JoinGameMenu" )
	end

	self.backButton = EditorButton.create( Vec2.create({32,256+32}), Vec2.create({128,24}), "Back" )
	self.backButton.onClick = function( button )
		Game:popState()
	end

	self:addControl( self.titleLabel )
	self:addControl( self.hostButton )
	self:addControl( self.joinButton )
	self:addControl( self.backButton )
end

Game:addState( StatePlayMenu )