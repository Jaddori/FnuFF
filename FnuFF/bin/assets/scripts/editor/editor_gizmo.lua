local gizmo = 
{
	position = Vec3.create(),
	orientation = Vec4.create({0,0,1,0}),
	scale = Vec3.create({1,1,1}),
	visible = false,
	x = Vec3.create({10,0,0}),
	y = Vec3.create({0,10,0}),
	z = Vec3.create({0,0,10}),
	
	xbounds = nil,
	ybounds = nil,
	zbounds = nil,
	
	red =   Vec4.create({1,0,0,1}),
	green = Vec4.create({0,1,0,1}),
	blue =  Vec4.create({0,0,1,1}),
	cyan =  Vec4.create({0,1,1,1}),

	selectedAxis = -1,
}

function gizmo:load()
end

function gizmo:unload()
end

function gizmo:update( deltaTime )
end

function gizmo:render()
	if self.visible then
		self.red[4] = 1
		self.green[4] = 1
		self.blue[4] = 1
		
		if self.selectedAxis == 1 then
			self.green[4] = 0.1
			self.blue[4] = 0.1
		elseif self.selectedAxis == 2 then
			self.red[4] = 0.1
			self.blue[4] = 0.1
		elseif self.selectedAxis == 3 then
			self.red[4] = 0.1
			self.green[4] = 0.1
		end
	
		DebugShapes.addLine( self.position, addVec( self.position, self.x ), self.red, true )
		DebugShapes.addLine( self.position, addVec( self.position, self.y ), self.green, true )
		DebugShapes.addLine( self.position, addVec( self.position, self.z ), self.blue, true )
	end
end

function gizmo:setPosition( position )
	self.position = Vec3.copy( position )
	
	local x = position[1]
	local y = position[2]
	local z = position[3]
	
	self.xbounds = Physics.createAABB( {x+1,y-1,z-1}, {x+10,y+1,z+1} )
	self.ybounds = Physics.createAABB( {x-1,y+1,z-1}, {x+1,y+10,z+1} )
	self.zbounds = Physics.createAABB( {x-1,y-1,z+1}, {x+1,y+1,z+10} )
end

function gizmo:setOrientation( orientation )
	--copyVec( orientation, self.orientation )
	self.orientation = orientation:copy()
end

function gizmo:setScale( scale )
	--copyVec( scale, self.scale )
	self.scale = scale:copy()
end

function gizmo:setMode( mode )
	self.mode = mode
end

return gizmo