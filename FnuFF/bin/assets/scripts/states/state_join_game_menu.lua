require( "./assets/scripts/states/state_menu" )

StateJoinGameMenu = StateMenu.create( "JoinGameMenu" )

function StateJoinGameMenu:load()
	self.titleLabel = EditorLabel.create( Vec2.create(), Vec2.create({WINDOW_WIDTH, 128}), "Triadic" )
	self.titleLabel:loadFont( "./assets/fonts/verdana18.bin", "./assets/fonts/verdana18.dds" )
	self.titleLabel:setTextAlignment( ALIGN_MIDDLE, ALIGN_MIDDLE )

	local yoffset = 128
	self.nameInput = EditorInputbox.create( vec(32,yoffset), 256, "Name:" )
	self.nameInput.textbox:setText( "Bluebear" )
	yoffset = yoffset + self.nameInput.size[2] + 8

	self.ipInput = EditorInputbox.create( vec(32,yoffset), 256, "IP:" )
	self.ipInput.textbox:setText( "127.0.0.1" )
	yoffset = yoffset + self.ipInput.size[2] + 8

	self.connectButton = EditorButton.create( vec(32+256-128, yoffset), vec(128,24), "Connect" )
	self.connectButton.onClick = function( button )
		self.notifyID = GameClient:registerNotify( self )
		GameClient:connect( self.ipInput.textbox.text, 12345, self.nameInput.textbox.text )
	end

	self.backButton = EditorButton.create( vec(32,256+32), vec(128,24), "Back" )
	self.backButton.onClick = function( button )
		Game:popState()
	end

	self:addControl( self.titleLabel )
	self:addControl( self.nameInput )
	self:addControl( self.ipInput )
	self:addControl( self.connectButton )
	self:addControl( self.backButton )
end

function StateJoinGameMenu:clientOnHandshakeCompleted()
	GameClient:unregisterNotify( self.notifyID )
	self.notifyID = nil

	doscript( "states/state_gameplay.lua" )
	StateLoading:show( StateGameplay )
end

Game:addState( StateJoinGameMenu )