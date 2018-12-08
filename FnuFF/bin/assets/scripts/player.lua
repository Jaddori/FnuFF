Player =
{
	name = "Player",
	transform = nil,
	meshIndex = -1,
	camera = nil,
	grid = nil,
	prevPosition = Vec3.create(),
	nextPosition = Vec3.create(),
	elapsedTime = 0,

	ghostTransform = nil,
	ghostPosition = Vec3.create(),

	isLocal = false,
	commands = {},

	collisionMargin = 0.1,

	-- DEBUG
	fontIndex = -1,
}

function Player.create( isLocal )
	local player = 
	{
		transform = Transform.create(),
		prevPosition = Vec3.create(),
		nextPosition = Vec3.create(),
		elapsedTime = 0,
	
		ghostTransform = nil,
		ghostPosition = Vec3.create(),
	
		isLocal = isLocal,
		commands = {},

		movementRays = {},
	}

	if IS_CLIENT then
		player.ghostTransform = Transform.create()

		if player.isLocal then
			player.camera = Graphics.getPerspectiveCamera()
			player.camera:setDirection( {0,-1,-1} )

			player.grid = doscript( "editor/editor_grid.lua" )

			player.fontIndex = Assets.loadFont( "./assets/fonts/verdana12.bin", "./assets/fonts/verdana12.dds" )
		end
	end

	setmetatable( player, { __index = Player } )

	return player
end

function Player:load()
	if IS_CLIENT then
		self.meshIndex = Assets.loadMesh( "./assets/models/cube.mesh" )
	end
end

function Player:unload()
end

function Player:update( deltaTime )
	if IS_CLIENT then
		if self.isLocal then
			-- update local players position
			self.elapsedTime = self.elapsedTime + deltaTime
			local timestep = TIMESTEP_MS * 0.001
			local t = self.elapsedTime / timestep

			if t > 1.0 then
				t = 1.0
			end

			local position = Vec3.lerp( self.prevPosition, self.nextPosition, t )

			self.transform:setPosition( position )

			-- update camera position
			self.camera:setPosition( position )
			self.camera:relativeMovement( {0,0,-25} )
		else -- not local
			self.elapsedTime = self.elapsedTime + deltaTime
			local timestep = (GameClient.averageReceiveTime*2) * 0.001

			local t = 0.5
			if timestep > 0 then
				t = self.elapsedTime / timestep
			end
			
			if t > 1.0 then
				t = 1.0
			end

			local position = Vec3.lerp( self.prevPosition, self.nextPosition, t )

			self.transform:setPosition( position )
		end
	end
end

function Player:fixedUpdate()
	if IS_CLIENT and self.isLocal then
		local command = { horizontal = 0, vertical = 0, localAck = GameClient.localAck }

		if Input.keyDown( Keys.Left ) then
			command.horizontal = command.horizontal - 1
		end

		if Input.keyDown( Keys.Right ) then
			command.horizontal = command.horizontal + 1
		end

		if Input.keyDown( Keys.Up ) then
			command.vertical = command.vertical - 1
		end

		if Input.keyDown( Keys.Down ) then
			command.vertical = command.vertical + 1
		end

		self:processCommand( command )
		self.commands[#self.commands+1] = command
	end
end

function Player:processCommand( command )
	if IS_CLIENT and self.isLocal then
		local movement = Vec3.create( { command.horizontal * 0.5, 0, command.vertical * 0.5 } )
		local curPosition = self.nextPosition:copy()
		local newPosition = curPosition + movement

		local ray = Physics.createRayFromPoints( curPosition, newPosition )

		for _,v in pairs(BoundingBoxes.aabbs) do
			local hit = {}
			if Physics.rayExpandedAABB( ray, v, 1, hit ) then
				if hit.length < ray.length then
					if math.abs( hit.normal[1] ) > 0 then
						movement[1] = movement[1] + ( hit.normal[1] * ( ray.length - hit.length + self.collisionMargin ) )
					else
						movement[3] = movement[3] + ( hit.normal[3] * ( ray.length - hit.length + self.collisionMargin ) )
					end
				end
			end
		end

		newPosition = curPosition + movement

		self.prevPosition = curPosition:copy()
		self.nextPosition = newPosition:copy()
	else -- IS_SERVER
		local movement = Vec3.create( { command.horizontal * 0.5, 0, command.vertical * 0.5 } )
		local curPosition = self.transform:getPosition()
		local newPosition = curPosition + movement

		local ray = Physics.createRayFromPoints( curPosition, newPosition )

		for _,v in pairs(BoundingBoxes.aabbs) do
			local hit = {}
			if Physics.rayExpandedAABB( ray, v, 1, hit ) then
				if hit.length < ray.length then
					if math.abs( hit.normal[1] ) > 0 then
						movement[1] = movement[1] + ( hit.normal[1] * ( ray.length - hit.length + self.collisionMargin ) )
					else
						movement[3] = movement[3] + ( hit.normal[3] * ( ray.length - hit.length + self.collisionMargin ) )
					end
				end
			end
		end

		newPosition = curPosition + movement

		self.transform:setPosition( newPosition )
	end
end

function Player:addMovementRay( ray )
	for i=#self.movementRays, 2, -1 do
		self.movementRays[i] = self.movementRays[i-1]
	end
	
	self.movementRays[1] = ray
end

function Player:render()
	Graphics.queueMesh( self.meshIndex, self.transform )
	--Graphics.queueMesh( self.meshIndex, self.ghostTransform )

	local position = self.ghostTransform:getPosition()
	local ghostMin = { position[1]-1, position[2]-1, position[3]-1 }
	local ghostMax = { position[1]+1, position[2]+1, position[3]+1 }

	DebugShapes.addAABB( ghostMin, ghostMax, {0,1,0,1}, true )
	
	-- render debug information
	if self.isLocal then
		self.grid:render()

		local rttText = "Average RTT: " .. tostring( math.floor( GameClient.averageRTT ) )
		Graphics.queueText( self.fontIndex, rttText, {32,32}, 0, {1,1,1,1} )

		local receiveTimeText = "Average receive time: " .. tostring( math.floor( GameClient.averageReceiveTime ) )
		Graphics.queueText( self.fontIndex, receiveTimeText, {32,32+16}, 0, {1,1,1,1} )
	end
end

function Player:clientWrite()
	local commandCount = 0
	for i=1, #self.commands do
		if self.commands[i].localAck == GameClient.localAck then
			commandCount = commandCount + 1
		end
	end

	GameClient:queue( 1, CLIENT_INT, commandCount )

	for i=1, #self.commands do
		if self.commands[i].localAck == GameClient.localAck then
			GameClient:queue( 1, CLIENT_INT, self.commands[i].horizontal )
			GameClient:queue( 1, CLIENT_INT, self.commands[i].vertical )
		end
	end

	return true
end

function Player:clientRead( message )
	local position = Vec3.create()

	position[1] = message:readFloat()
	position[2] = message:readFloat()
	position[3] = message:readFloat()

	if self.isLocal then
		self.ghostPosition = position:copy()
		self.ghostTransform:setPosition( self.ghostPosition )

		self.prevPosition = position:copy()
		self.nextPosition = position:copy()

		local newCommands = {}
		local count = #self.commands
		for i=1, count do
			if self.commands[i].localAck > message.remoteAck then
				self:processCommand( self.commands[i] )
				newCommands[#newCommands+1] = self.commands[i]
			end
		end

		self.commands = newCommands
	else
		self.ghostPosition = position:copy()
		self.ghostTransform:setPosition( self.ghostPosition )

		local currentPosition = self.transform:getPosition()
		self.prevPosition = currentPosition:copy()
		self.nextPosition = position:copy()

		self.elapsedTime = 0
	end
end

function Player:serverRead( message )
	local commandCount = message:readInt()

	for i=1, commandCount do
		local command =
		{
			horizontal = message:readInt(),
			vertical = message:readInt(),
		}

		self:processCommand( command )
	end
end

function Player:serverWrite( hash )
	local position = self.transform:getPosition()

	GameServer:queue( hash, 1, SERVER_FLOAT, position[1] )
	GameServer:queue( hash, 1, SERVER_FLOAT, position[2] )
	GameServer:queue( hash, 1, SERVER_FLOAT, position[3] )

	return true
end