PlayerHandler =
{
	playerCount = 0,
}

function PlayerHandler:load()
	doscript( "player.lua" )

	if IS_CLIENT then
		self.localPlayer = Player.create( true )
		self.remotePlayers = {}

		self.localPlayer:load()

		GameClient:register( self, 1 )
	else -- IS_SERVER
		self.players = {}

		GameServer:register( self, 1 )
	end
end

function PlayerHandler:unload()
	if IS_CLIENT then
		self.localPlayer:unload()

		for i=1, #self.remotePlayers do
			self.remotePlayers[i]:unload()
		end
	end
end

function PlayerHandler:update( deltaTime )
	if IS_CLIENT then
		self.localPlayer:update( deltaTime )

		for i=1, #self.remotePlayers do
			self.remotePlayers[i]:update( deltaTime )
		end
	end
end

function PlayerHandler:fixedUpdate()
	if IS_CLIENT and GameClient.handshakeCompleted then
		self.localPlayer:fixedUpdate()
	end
end

function PlayerHandler:render()
	if IS_CLIENT then
		self.localPlayer:render()

		for i=1, #self.remotePlayers do
			self.remotePlayers[i]:render()
		end
	end
end

function PlayerHandler:clientWrite()
	self.localPlayer:clientWrite()

	return true
end

function PlayerHandler:clientRead( message )
	local playerCount = message:readInt()
	local remoteIndex = 1

	if #self.remotePlayers < playerCount-1 then
		local dif = playerCount - #self.remotePlayers - 1

		for i=1, dif do
			self.remotePlayers[i] = Player.create( false )
			self.remotePlayers[i]:load()
			Log.debug( "PlayerHandler: Adding remote player." )
		end
	end

	for i=1, playerCount do
		local networkID = message:readInt()
		--if networkID == Client.getNetworkID() then
		if networkID == GameClient.networkID then
			self.localPlayer:clientRead( message )
		else
			self.remotePlayers[remoteIndex]:clientRead( message )
			remoteIndex = remoteIndex + 1
		end
	end
end

function PlayerHandler:serverWrite( hash )
	GameServer:queue( hash, 1, SERVER_INT, #self.players )
	for i=1, #self.players do
		GameServer:queue( hash, 1, SERVER_INT, self.players[i].networkID )
		self.players[i]:serverWrite( hash )
	end

	return true
end

function PlayerHandler:serverRead( message )
	local hash = message:getHash()
	
	for i=1, #self.players do
		if self.players[i].hash == hash then
			self.players[i]:serverRead( message )
		end
	end
end

function PlayerHandler:serverOnNewHash( hash )
	--local newPlayer = Player.create( false )
	--newPlayer.hash = hash
	----newPlayer.networkID = Server.getNetworkID( hash )
	--
	--self.players[#self.players+1] = newPlayer
	--Log.debug( "PlayerHandler: Adding player #" .. tostring( #self.players ) .. " with hash " .. tostring( hash ) )
end

function PlayerHandler:serverOnHandshakeCompleted( hash, networkID, playerName )
	local newPlayer = Player.create( false )
	newPlayer.hash = hash
	newPlayer.networkID = networkID
	newPlayer.name = playerName

	self.players[#self.players+1] = newPlayer
	Log.debug( "PlayerHandler: Adding player " .. playerName .. " (#" .. tostring( #self.players ) .. ") with hash " .. tostring( hash ) )
end