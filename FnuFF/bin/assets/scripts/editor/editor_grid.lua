local grid = 
{
	showGrid = true,
	size = 100,
	interval = 2,
	color = Vec4.create({ 0.25, 0.25, 0.25, 0.5 }),
	yoffset = 0,
	
	showOrigo = true,
	origo = Vec3.create({0,0,0}),
	x = Vec3.create({10,0,0}),
	y = Vec3.create({0,10,0}),
	z = Vec3.create({0,0,10}),
	
	red = Vec4.create({1,0,0,1}),
	green = Vec4.create({0,1,0,1}),
	blue = Vec4.create({0,0,1,1}),
}

function grid:render()
	-- render grid
	if self.showGrid then
		local len = self.size*self.interval
		for i=-len, len, self.interval do
			local z = { { i, self.yoffset, -len }, { i, self.yoffset, len } }
			local x = { { -len, self.yoffset, i }, { len, self.yoffset, i } }
			
			DebugShapes.addLine( z[1], z[2], self.color )
			DebugShapes.addLine( x[1], x[2], self.color )
		end
	end
	
	-- render origo
	if self.showOrigo then
		DebugShapes.addLine( self.origo, self.x, self.red, true )
		DebugShapes.addLine( self.origo, self.y, self.green, true )
		DebugShapes.addLine( self.origo, self.z, self.blue, true )
	end
end

return grid