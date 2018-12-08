BoundingBoxes =
{
	aabbs = {},
	spheres = {},
	rays = {},
	debug = false,
	ignoreDepth = false,
	aabbDebugColor = {1,0,1,1},
	sphereDebugColor = {1,0,1,1},
	rayDebugColor = {1,0,1,1},
}

function BoundingBoxes:addAABB( aabb )
	self.aabbs[#self.aabbs+1] = aabb
end

function BoundingBoxes:addSphere( sphere )
	self.spheres[#self.spheres+1] = sphere
end

function BoundingBoxes:addRay( ray )
	self.rays[#self.rays+1] = ray
end

function BoundingBoxes:render()
	if self.debug then
		for _,v in pairs( self.aabbs ) do
			DebugShapes.addAABB( v.minPosition, v.maxPosition, self.aabbDebugColor, self.ignoreDepth )
		end

		for _,v in pairs( self.spheres ) do
			DebugShapes.addSphere( v.center, v.radius, self.sphereDebugColor, self.ignoreDepth )
		end

		for _,v in pairs( self.rays ) do
			DebugShapes.addRay( v.first, v.last, self.rayDebugColor, self.ignoreDepth )
		end
	end
end