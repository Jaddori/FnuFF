SERVER_BOOL = 1
SERVER_INT = 2
SERVER_FLOAT = 3
SERVER_STRING = 4

SERVER_RESEND_TIME_MS = 1000
SERVER_MAX_RETRIES = 3

GameServer =
{
	notifies = {},
	objects = {},
	acks = {},

	debug_droprate = 0,
	packets = {},
	reliablePackets = {},

	handshakes = {},
	networkIDCounter = 1,
}

function GameServer:register( object, id )
	assert( isnumber( id ), "ID must be a number." )
	assert( self.objects[id] == nil, "ID is already taken: " .. tostring( id ) )

	self.objects[id] = object
end

--function GameServer:unregister( id )
--	assert( isnumber( id ), "ID must be a number." )
--	assert( self.objects[id], "No object found for id " .. tostring( id ) )
--
--	self.objects[id] = nil
--end
--
--function GameServer:registerNotify( object )
--	local id = #self.notifies + 1
--	self.notifies[id] = object
--
--	return id
--end
--
--function GameServer:unregisterNotify( id )
--	self.notifies[id] = nil
--end

function GameServer:serverWrite()
	for ackKey,ackValue in pairs(self.acks) do
		if self.handshakes[ackValue.hash] then
			self:writeHandshake( ackValue.hash )
		else
			Server.queueUint( ackValue.hash, ackValue.localAck )
			Server.queueUint( ackValue.hash, ackValue.remoteAck )
			Server.queueUint( ackValue.hash, ackValue.history )

			local writingObjects = 0
			for objectKey,objectValue in pairs(self.objects) do
				if objectValue:serverWrite( ackValue.hash ) then
					writingObjects = writingObjects + 1
				else
					if #self.reliablePackets[ackValue.hash][objectKey] > 0 then
						for packetKey,packetValue in pairs(self.reliablePackets[ackValue.hash][objectKey]) do
							if packetValue.localAck == ackValue.localAck then
								writingObjects = writingObjects + 1
								break
							end
						end
					end
				end
			end

			Server.queueInt( ackValue.hash, writingObjects )
			for objectKey,objectValue in pairs(self.objects) do
				if #self.packets[ackValue.hash][objectKey] > 0 or #self.reliablePackets[ackValue.hash][objectKey] > 0 then
					Server.queueInt( ackValue.hash, objectKey )

					for packetKey, packetValue in pairs(self.packets[ackValue.hash][objectKey]) do
						local type = packetValue.type

						if type == SERVER_INT then Server.queueInt( ackValue.hash, packetValue.value )
						elseif type == SERVER_UINT then Server.queueUint( ackValue.hash, packetValue.value )
						elseif type == SERVER_FLOAT then Server.queueFloat( ackValue.hash, packetValue.value )
						elseif type == SERVER_STRING then Server.queueString( ackValue.hash, packetValue.value )
						end
					end

					for packetKey, packetValue in pairs(self.reliablePackets[ackValue.hash][objectKey]) do
						if packetValue.localAck == ackValue.localAck then
							local type = packetValue.type

							if type == SERVER_INT then Server.queueInt( ackValue.hash, packetValue.value )
							elseif type == SERVER_UINT then Server.queueUint( ackValue.hash, packetValue.value )
							elseif type == SERVER_FLOAT then Server.queueFloat( ackValue.hash, packetValue.value )
							elseif type == SERVER_STRING then Server.queueString( ackValue.hash, packetValue.value )
							end
						end
					end
				end

				self.packets[ackValue.hash][objectKey] = {}
			end

			ackValue.localAck = ackValue.localAck + 1
		end
	end
end

function GameServer:serverRead()
	local messages = Server.getMessages()

	for i=1, #messages do
		local message = messages[i]

		local hash = message:getHash()
		
		-- setup packet table for hash, if non-existant
		if not self.packets[hash] then
			self.packets[hash] = {}
			self.reliablePackets[hash] = {}
			
			self.acks[hash] =
			{
				hash = hash,
				localAck = 0,
				remoteAck = 0,
				history = 0
			}

			for k,v in pairs(self.objects) do
				self.packets[hash][k] = {}
				self.reliablePackets[hash][k] = {}

				if v.serverOnNewHash then
					v:serverOnNewHash( hash )
				end
			end

			self.handshakes[hash] =
			{
				phase = 1,
				retries = 0,
				lastSend = Core.getTicks(),
				salt = 0,
				networkID = 0,
			}
		end

		local r = math.random(100)
		if r >= self.debug_droprate then
			if self.handshakes[hash] then
				self:readHandshake( message, hash )
			else
				message.localAck = message:readInt()
				message.remoteAck = message:readInt()
				message.history = message:readInt()

				if message.localAck > self.acks[hash].remoteAck then
					-- let all registered objects read from the message
					local idCount = message:readInt()
					for j=1, idCount do
						local id = message:readInt()

						if self.objects[id] then
							self.objects[id]:serverRead( message )
						else
							Log.error( "Server: bad object id. Discarding packet." )
							break
						end
					end

					-- update acks
					local prevRemoteAck = self.acks[hash].remoteAck
					self.acks[hash].remoteAck = message.localAck

					self.acks[hash].history = bit32.lshift( self.acks[hash].history, self.acks[hash].remoteAck - prevRemoteAck )
					self.acks[hash].history = bit32.bor( self.acks[hash].history, 1 )

					-- check if any reliable packets were dropped
					for i=1, #self.reliablePackets[hash] do
						local notReceived = {}

						for j=1, #self.reliablePackets[hash][i] do
							local packet = self.reliablePackets[hash][i][j]

							if packet.localAck <= message.remoteAck then
								local offset = message.remoteAck - packet.localAck
								local wasReceived = false
								if offset < 32 then
									wasReceived = ( bit32.extract( message.history, offset ) > 0 )
								else
									Log.debug( "OUT OF RANGE" )
								end

								if not wasReceived then
									packet.localAck = self.acks[hash].localAck
									notReceived[#notReceived+1] = packet

									Log.debug( "Server: Resending dropped reliable packet." )
								end
							else
								notReceived[#notReceived+1] = packet
							end
						end

						self.reliablePackets[hash][i] = notReceived
					end
				end
			end
		end
	end
end

function GameServer:writeHandshake( hash )
	if self.handshakes[hash].phase == 2 then
		local shouldSend = false
		local curTime = Core.getTicks()
		local elapsedTime = curTime - self.handshakes[hash].lastSend

		if elapsedTime > SERVER_RESEND_TIME_MS then
			self.handshakes[hash].retries = self.handshakes[hash].retries + 1
			if self.handshakes[hash].retries < SERVER_MAX_RETRIES then
				shouldSend = true
				self.handshakes[hash].lastSend = curTime
			else
				Log.debug( "Maximum number of retries reached. Dropping client." )
			end
		end

		if shouldSend then
			Log.debug( "GameServer: writing handshake for phase #" .. tostring( self.handshakes[hash].phase ) )

			local fullSalt = bit32.bxor( self.handshakes[hash].salt, hash )
			self.handshakes[hash].networkID = self.networkIDCounter
			self.networkIDCounter = self.networkIDCounter + 1

			Server.queueUint( hash, self.handshakes[hash].phase )
			Server.queueUint( hash, fullSalt )
			Server.queueUint( hash, self.handshakes[hash].networkID )
		end
	end
end

function GameServer:readHandshake( message, hash )
	local phase = message:readUint()

	if phase == self.handshakes[hash].phase then
		if self.handshakes[hash].phase == 1 then
			Log.debug( "GameServer: reading handshake for phase #" .. tostring( self.handshakes[hash].phase ) )

			self.handshakes[hash].salt = message:readUint()
			self.handshakes[hash].playerName = message:readString()
			self.handshakes[hash].phase = 2
			self.handshakes[hash].retries = 0
			self.handshakes[hash].lastSend = 0
		elseif self.handshakes[hash].phase == 2 then
			Log.debug( "GameServer: reading handshake for phase #" .. tostring( self.handshakes[hash].phase ) )

			local networkID = self.handshakes[hash].networkID
			local playerName = self.handshakes[hash].playerName

			for _,v in pairs(self.objects) do
				if v.serverOnHandshakeCompleted then
					v:serverOnHandshakeCompleted( hash, networkID, playerName )
				end
			end

			for _,v in pairs(self.notifies) do
				if v.serverOnHandshakeCompleted then
					v:serverOnHandshakeCompleted( hash, networkID, playerName )
				end
			end

			self.handshakes[hash] = nil
			Log.debug( "GameServer: handshake completed." )
		end
	else
		Log.warning( "GameServer: handshake phase mismatch." )
	end
end

function GameServer:queue( hash, id, type, value, reliable )
	assert( type == SERVER_INT or type == SERVER_UINT or type == SERVER_FLOAT or type == SERVER_STRING, "Server: bad type = " .. tostring( type ) )
	assert( value, "Value was nil." )
	assert( hash, "Hash was nil." )

	if reliable then
		local count = #self.reliablePackets[hash][id]
		self.reliablePackets[hash][id][count+1] = { type = type, value = value, localAck = self.acks[hash].localAck }
	else
		local count = #self.packets[hash][id]
		self.packets[hash][id][count+1] = { type = type, value = value }
	end
end