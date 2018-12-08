local list = 
{
	fontIndex = -1,
	fontHeight = 0,
	meshes = {},
	position = Vec2.create({ WINDOW_WIDTH - 128, 0 }),
	textColor = Vec4.create({1,1,1,1}),
}

function list:load()
	self.fontIndex = Assets.loadFont( "./assets/fonts/verdana12.bin", "./assets/fonts/verdana12.dds" )
	if self.fontIndex >= 0 then
		local font = Assets.getFont( self.fontIndex )
		self.fontHeight = font:getHeight()
	else
		Log.error( "Failed to load font verdana12 for editor_mesh_list.lua" )
	end
	
	self.meshes = Filesystem.getFiles( "./assets/models/*" )
end

function list:render()
	if self.fontIndex < 0 then return end 

	local position = Vec2.create({ self.position[1], self.position[2] })
	
	for _,v in pairs(self.meshes) do
		Graphics.queueText( self.fontIndex, v, position, self.textColor )
		position[2] = position[2] + self.fontHeight
	end
end

return list