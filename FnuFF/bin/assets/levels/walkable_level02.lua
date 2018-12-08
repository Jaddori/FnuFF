-- Floor
Props:add( Vec3.create({-10,0,0}), {0,0,0,1}, Vec3.create({4,1,2}), "floor.mesh" )

-- Wall_BottomRight
local boundingBox =
{
	minPosition = Vec3.create({9.5,0,-10}),
	maxPosition = Vec3.create({10,10,10}),
	center = Vec3.create({9.75,5,0}),
	extents = Vec3.create({0.25,5,10}),
}
BoundingBoxes:addAABB( boundingBox )

Props:add( Vec3.create({15,0,0}), {0,0,0,1}, Vec3.create({1,1,2}), "wall.mesh" )

-- Wall_TopRight
local boundingBox =
{
	minPosition = Vec3.create({-30,0,-10}),
	maxPosition = Vec3.create({10,10,-9.5}),
	center = Vec3.create({-10,5,-9.75}),
	extents = Vec3.create({20,5,0.25}),
}
BoundingBoxes:addAABB( boundingBox )

Props:add( Vec3.create({-10,0,-15}), {0,0.71,0,0.71}, Vec3.create({1,1,4}), "wall.mesh" )

-- Wall_TopLeft
local boundingBox =
{
	minPosition = Vec3.create({-30,0,-10}),
	maxPosition = Vec3.create({-29.5,10,10}),
	center = Vec3.create({-29.75,5,0}),
	extents = Vec3.create({0.25,5,10}),
}
BoundingBoxes:addAABB( boundingBox )

Props:add( Vec3.create({-35,0,0}), {0,1,0,0}, Vec3.create({1,1,2}), "wall.mesh" )

-- Wall_BottomLeft
local boundingBox =
{
	minPosition = Vec3.create({-30,0,9.5}),
	maxPosition = Vec3.create({10,10,10}),
	center = Vec3.create({-10,5,9.75}),
	extents = Vec3.create({20,5,0.25}),
}
BoundingBoxes:addAABB( boundingBox )

Props:add( Vec3.create({-10,0,15}), {0,0.71,0,-0.71}, Vec3.create({1,1,4}), "wall.mesh" )

-- Light
local light =
{
	position = Vec3.create({4.21,4.83,-6.95}),
	offset = Vec3.create({0,0,0}),
	color = Vec3.create({0.6,1,1}),
	intensity = 4,
	linear = 1,
	constant = 1,
	exponent = 1,
	size = 1
}
Lights:addPointLight( light )

-- SunLight
local light =
{
	direction = Vec3.create({-1,-1,-1}),
	color = Vec3.create({1,0.7,0.7}),
	intensity = 0.1
}
Lights:addDirectionalLight( light )

-- Light2
local light =
{
	position = Vec3.create({-15.12,5.9,4.42}),
	offset = Vec3.create({0,0,0}),
	color = Vec3.create({0.6,1,1}),
	intensity = 4,
	linear = 1,
	constant = 1,
	exponent = 1,
	size = 1
}
Lights:addPointLight( light )

-- Pillar
local boundingBox =
{
	minPosition = Vec3.create({-16.46,0,3.35}),
	maxPosition = Vec3.create({-14.46,4,5.35}),
	center = Vec3.create({-15.46,2,4.35}),
	extents = Vec3.create({1,2,1}),
}
BoundingBoxes:addAABB( boundingBox )

Props:add( Vec3.create({-15.46,0,4.35}), {0,0,0,1}, Vec3.create({1,1,1}), "pillar05.mesh" )

-- WalkableSurface

-- NewEntity
local emitter =
{
	spherical = true,
	minFrequency = 0.1,
	maxFrequency = 0.3,
	minLifetime = 2,
	maxLifetime = 3,
	minDirection = Vec3.create({-0.25,1,-0.25}),
	maxDirection = Vec3.create({0.25,1,0.25}),
	startSpeed = 4,
	endSpeed = 3,
	startSize = 3,
	endSize = 5,
	
	maxParticles = 15,
	position = Vec3.create({-10,0,2}),
}
Particles:addEmitter( emitter )

