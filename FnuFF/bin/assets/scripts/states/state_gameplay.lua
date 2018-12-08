StateGameplay =
{
	name = "Gameplay",
}

function StateGameplay:load()
	if IS_CLIENT then
		Log.debug( "LOADING GAMEPLAY" )
	end

	doscript( "game/player_handler.lua" )
	--doscript( "game/chat.lua" )
	
	doscript( "game/bounding_boxes.lua" )

	doscript( "game/lights.lua" )
	doscript( "game/props.lua" )
	doscript( "game/particles.lua" )

	if IS_SERVER then
		--doscript( "game/server.lua" )
	else
		--doscript( "game/client.lua" )
		Graphics.setLightingEnabled( true )
	end

	PlayerHandler:load()
	--Chat:load()

	self:loadLevel( "walkable_level02.lua" )

	if IS_CLIENT then
		Core.sleep( 2000 ) -- sleep two seconds to test the loading screen
	end

	Core.resetUpdateAccumulator()
end

function StateGameplay:loadLevel( level )
	dofile( "./assets/levels/" .. level )
end

function StateGameplay:unload()
	PlayerHandler:unload()
	--Chat:unload()
end

function StateGameplay:update( deltaTime )
	PlayerHandler:update( deltaTime )
	Particles:update( deltaTime )
end

function StateGameplay:fixedUpdate()
	PlayerHandler:fixedUpdate()
	--Chat:fixedUpdate()

	if Input.keyReleased( Keys.T ) then
		Graphics.setLightingEnabled( not Graphics.getLightingEnabled() )
	end

	if Input.keyReleased( Keys.B ) then
		BoundingBoxes.ignoreDepth = true
		BoundingBoxes.debug = not BoundingBoxes.debug
	end
end

function StateGameplay:render()
	PlayerHandler:render()
	--Chat:render()

	BoundingBoxes:render()
	Lights:render()
	Props:render()
	Particles:render()
end

function StateGameplay:clientWrite()
	GameClient:clientWrite()
end

function StateGameplay:clientRead()
	GameClient:clientRead()
end

function StateGameplay:serverWrite()
	GameServer:serverWrite()
end

function StateGameplay:serverRead()
	GameServer:serverRead()
end

Game:addState( StateGameplay )