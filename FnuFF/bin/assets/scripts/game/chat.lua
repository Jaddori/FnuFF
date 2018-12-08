Chat =
{
	name = "Chat",
	sendMessages = {},
	messages = {},

	fontIndex = -1,
	counter = 1,
}

function Chat:load()
	if IS_CLIENT then
		self.fontIndex = Assets.loadFont( "./assets/fonts/verdana12.bin", "./assets/fonts/verdana12.dds" )

		GameClient:register( self, 2 )
	else
		GameServer:register( self, 2 )
	end
end

function Chat:unload()
end

function Chat:fixedUpdate()
	if IS_CLIENT then
		if Input.keyReleased( Keys.M ) then
			self:queueMessage( "Hello, World!: " .. tostring( self.counter ) )
			self.counter = self.counter + 1
		end
	end
end

function Chat:render()
	for i=1, #self.messages do
		Graphics.queueText( self.fontIndex, self.messages[i], {32,32+16*i}, 0, {1,1,1,1} )
	end
end

function Chat:queueMessage( message )
	self.sendMessages[#self.sendMessages+1] = message
	self.messages[#self.messages+1] = "Local: " .. message
end

function Chat:clientRead( message )
	local text = message:readString()

	self.messages[#self.messages+1] = "Server: " .. text
end

function Chat:clientWrite()
	local result = false

	if #self.sendMessages > 0 then
		GameClient:queue( 2, CLIENT_STRING, self.sendMessages[1], true )
		self.sendMessages[1] = nil

		result = true
	end

	return result
end

function Chat:serverRead( message )
	local text = message:readString()

	for _,v in pairs(self.sendMessages) do
		v[#v+1] = text
	end
end

function Chat:serverWrite( hash )
	local result = false

	if #self.sendMessages[hash] > 0 then
		GameServer:queue( hash, 2, SERVER_STRING, self.sendMessages[hash][1], true )
		self.sendMessages[hash][1] = nil

		result = true
	end

	return result
end

function Chat:serverOnNewHash( hash )
	self.sendMessages[hash] = {}
end