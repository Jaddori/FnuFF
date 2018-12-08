-- camera
Editor.camera.camera:setPosition( Vec3.create({24.32,38.12,19.14}) )
Editor.camera.camera:setDirection( Vec3.create({-0.68,-0.64,-0.36}) )

-- prefabs
Prefabs["Wall"] =
{
	name = "Wall",
	instances = {},
	components = {},
}
setmetatable( Prefabs["Wall"], { __index = Prefab } )

Prefabs["Wall"].components["Mesh"] = ComponentMesh.create()
Prefabs["Wall"].components["Mesh"]:loadMesh( "wall.mesh" )

--entities
local entities = {}

-- Floor
local Floor = Entity.create( "Floor", Vec3.create({-10,0,0}), Vec3.create({0,0,0}), Vec3.create({4,1,2}) )
Floor.visible = true
local Floor_component = ComponentMesh.create( Floor )
Floor_component:loadMesh( "floor.mesh" )
Floor:addComponent( Floor_component )
local Floor_component = nil
entities[#entities+1] = Floor
-- Floor

-- Wall_BottomRight
local Wall_BottomRight = Entity.create( "Wall_BottomRight", Vec3.create({15,0,0}), Vec3.create({0,0,0}), Vec3.create({1,1,2}) )
Wall_BottomRight.visible = true
Wall_BottomRight.prefab = Prefabs["Wall"]
Prefabs["Wall"].instances[#Prefabs["Wall"].instances+1] = Wall_BottomRight
local Wall_BottomRight_component = ComponentBoundingBox.create( Wall_BottomRight )
Wall_BottomRight_component.type = 3
Wall_BottomRight_component.offset = Vec3.create({0,0,0})
Wall_BottomRight_component.ray.start = Vec3.create({15,0,0})
Wall_BottomRight_component.ray.length = 5
Wall_BottomRight_component.ray.direction = Vec3.create({0.58,0.58,0.58})
Wall_BottomRight_component.sphere.center = Vec3.create({15,0,0})
Wall_BottomRight_component.sphere.radius = 2
Wall_BottomRight_component.aabb.minPosition = Vec3.create({9.5,0,-10})
Wall_BottomRight_component.aabb.maxPosition = Vec3.create({10,10,10})
Wall_BottomRight_component.aabb.minOffset = Vec3.create({-5.5,0,-10})
Wall_BottomRight_component.aabb.maxOffset = Vec3.create({-5,10,10})
Wall_BottomRight:addComponent( Wall_BottomRight_component )
local Wall_BottomRight_component = ComponentMesh.create( Wall_BottomRight )
Wall_BottomRight_component:loadMesh( "wall.mesh" )
Wall_BottomRight:addComponent( Wall_BottomRight_component )
local Wall_BottomRight_component = nil
entities[#entities+1] = Wall_BottomRight
-- Wall_BottomRight

-- Wall_TopRight
local Wall_TopRight = Entity.create( "Wall_TopRight", Vec3.create({-10,0,-15}), Vec3.create({0,90,0}), Vec3.create({1,1,4}) )
Wall_TopRight.visible = true
Wall_TopRight.prefab = Prefabs["Wall"]
Prefabs["Wall"].instances[#Prefabs["Wall"].instances+1] = Wall_TopRight
local Wall_TopRight_component = ComponentBoundingBox.create( Wall_TopRight )
Wall_TopRight_component.type = 3
Wall_TopRight_component.offset = Vec3.create({0,0,0})
Wall_TopRight_component.ray.start = Vec3.create({-10,0,-15})
Wall_TopRight_component.ray.length = 5
Wall_TopRight_component.ray.direction = Vec3.create({0.58,0.58,0.58})
Wall_TopRight_component.sphere.center = Vec3.create({-10,0,-15})
Wall_TopRight_component.sphere.radius = 2
Wall_TopRight_component.aabb.minPosition = Vec3.create({-30,0,-9.5})
Wall_TopRight_component.aabb.maxPosition = Vec3.create({10,10,-10})
Wall_TopRight_component.aabb.minOffset = Vec3.create({-20,0,5.5})
Wall_TopRight_component.aabb.maxOffset = Vec3.create({20,10,5})
Wall_TopRight:addComponent( Wall_TopRight_component )
local Wall_TopRight_component = ComponentMesh.create( Wall_TopRight )
Wall_TopRight_component:loadMesh( "wall.mesh" )
Wall_TopRight:addComponent( Wall_TopRight_component )
local Wall_TopRight_component = nil
entities[#entities+1] = Wall_TopRight
-- Wall_TopRight

-- Wall_TopLeft
local Wall_TopLeft = Entity.create( "Wall_TopLeft", Vec3.create({-35,0,0}), Vec3.create({0,180,0}), Vec3.create({1,1,2}) )
Wall_TopLeft.visible = true
Wall_TopLeft.prefab = Prefabs["Wall"]
Prefabs["Wall"].instances[#Prefabs["Wall"].instances+1] = Wall_TopLeft
local Wall_TopLeft_component = ComponentBoundingBox.create( Wall_TopLeft )
Wall_TopLeft_component.type = 3
Wall_TopLeft_component.offset = Vec3.create({0,0,0})
Wall_TopLeft_component.ray.start = Vec3.create({-35,0,0})
Wall_TopLeft_component.ray.length = 5
Wall_TopLeft_component.ray.direction = Vec3.create({0.58,0.58,0.58})
Wall_TopLeft_component.sphere.center = Vec3.create({-35,0,0})
Wall_TopLeft_component.sphere.radius = 2
Wall_TopLeft_component.aabb.minPosition = Vec3.create({-29.5,0,10})
Wall_TopLeft_component.aabb.maxPosition = Vec3.create({-30,10,-10})
Wall_TopLeft_component.aabb.minOffset = Vec3.create({5.5,0,10})
Wall_TopLeft_component.aabb.maxOffset = Vec3.create({5,10,-10})
Wall_TopLeft:addComponent( Wall_TopLeft_component )
local Wall_TopLeft_component = ComponentMesh.create( Wall_TopLeft )
Wall_TopLeft_component:loadMesh( "wall.mesh" )
Wall_TopLeft:addComponent( Wall_TopLeft_component )
local Wall_TopLeft_component = nil
entities[#entities+1] = Wall_TopLeft
-- Wall_TopLeft

-- Wall_BottomLeft
local Wall_BottomLeft = Entity.create( "Wall_BottomLeft", Vec3.create({-10,0,15}), Vec3.create({0,270,0}), Vec3.create({1,1,4}) )
Wall_BottomLeft.visible = true
Wall_BottomLeft.prefab = Prefabs["Wall"]
Prefabs["Wall"].instances[#Prefabs["Wall"].instances+1] = Wall_BottomLeft
local Wall_BottomLeft_component = ComponentBoundingBox.create( Wall_BottomLeft )
Wall_BottomLeft_component.type = 3
Wall_BottomLeft_component.offset = Vec3.create({0,0,0})
Wall_BottomLeft_component.ray.start = Vec3.create({-10,0,15})
Wall_BottomLeft_component.ray.length = 5
Wall_BottomLeft_component.ray.direction = Vec3.create({0.58,0.58,0.58})
Wall_BottomLeft_component.sphere.center = Vec3.create({-10,0,15})
Wall_BottomLeft_component.sphere.radius = 2
Wall_BottomLeft_component.aabb.minPosition = Vec3.create({10,0,9.5})
Wall_BottomLeft_component.aabb.maxPosition = Vec3.create({-30,10,10})
Wall_BottomLeft_component.aabb.minOffset = Vec3.create({20,0,-5.5})
Wall_BottomLeft_component.aabb.maxOffset = Vec3.create({-20,10,-5})
Wall_BottomLeft:addComponent( Wall_BottomLeft_component )
local Wall_BottomLeft_component = ComponentMesh.create( Wall_BottomLeft )
Wall_BottomLeft_component:loadMesh( "wall.mesh" )
Wall_BottomLeft:addComponent( Wall_BottomLeft_component )
local Wall_BottomLeft_component = nil
entities[#entities+1] = Wall_BottomLeft
-- Wall_BottomLeft

-- Light
local Light = Entity.create( "Light", Vec3.create({4.21,4.83,-6.95}), Vec3.create({0,0,0}), Vec3.create({1,1,1}) )
Light.visible = true
local Light_component = ComponentPointLight.create( Light )
Light_component.position = Vec3.create({4.21,4.83,-6.95})
Light_component.offset = Vec3.create({0,0,0})
Light_component.color = Vec3.create({0.6,1,1})
Light_component.intensity = 4
Light_component.constant = 1
Light_component.size = 1
Light:addComponent( Light_component )
local Light_component = nil
entities[#entities+1] = Light
-- Light

-- SunLight
local SunLight = Entity.create( "SunLight", Vec3.create({-6.78,16.13,15.48}), Vec3.create({0,0,0}), Vec3.create({1,1,1}) )
SunLight.visible = true
local SunLight_component = ComponentDirectionalLight.create( SunLight )
SunLight_component.direction = Vec3.create({-1,-1,-1})
SunLight_component.color = Vec3.create({1,0.7,0.7})
SunLight_component.intensity = 0.1
SunLight:addComponent( SunLight_component )
local SunLight_component = nil
entities[#entities+1] = SunLight
-- SunLight

-- Light2
local Light2 = Entity.create( "Light2", Vec3.create({-15.12,5.9,4.42}), Vec3.create({0,0,0}), Vec3.create({1,1,1}) )
Light2.visible = true
local Light2_component = ComponentPointLight.create( Light2 )
Light2_component.position = Vec3.create({-15.12,5.9,4.42})
Light2_component.offset = Vec3.create({0,0,0})
Light2_component.color = Vec3.create({0.6,1,1})
Light2_component.intensity = 4
Light2_component.constant = 1
Light2_component.size = 1
Light2:addComponent( Light2_component )
local Light2_component = nil
entities[#entities+1] = Light2
-- Light2

-- Pillar
local Pillar = Entity.create( "Pillar", Vec3.create({-15.46,0,4.35}), Vec3.create({0,0,0}), Vec3.create({1,1,1}) )
Pillar.visible = true
local Pillar_component = ComponentBoundingBox.create( Pillar )
Pillar_component.type = 3
Pillar_component.offset = Vec3.create({0,0,0})
Pillar_component.ray.start = Vec3.create({-15.46,0,4.35})
Pillar_component.ray.length = 5
Pillar_component.ray.direction = Vec3.create({0.58,0.58,0.58})
Pillar_component.sphere.center = Vec3.create({-15.46,0,4.35})
Pillar_component.sphere.radius = 2
Pillar_component.aabb.minPosition = Vec3.create({-16.46,0,3.35})
Pillar_component.aabb.maxPosition = Vec3.create({-14.46,4,5.35})
Pillar_component.aabb.minOffset = Vec3.create({-1,0,-1})
Pillar_component.aabb.maxOffset = Vec3.create({1,4,1})
Pillar:addComponent( Pillar_component )
local Pillar_component = ComponentMesh.create( Pillar )
Pillar_component:loadMesh( "pillar05.mesh" )
Pillar:addComponent( Pillar_component )
local Pillar_component = nil
entities[#entities+1] = Pillar
-- Pillar

-- WalkableSurface
local WalkableSurface = Entity.create( "WalkableSurface", Vec3.create({-30,0,-10}), Vec3.create({0,0,0}), Vec3.create({1,1,1}) )
WalkableSurface.visible = true
local WalkableSurface_component = ComponentWalkable.create( WalkableSurface )
WalkableSurface_component.size = Vec2.create({40,20})
WalkableSurface_component.interval = 1
WalkableSurface:addComponent( WalkableSurface_component )
local WalkableSurface_component = nil
entities[#entities+1] = WalkableSurface
-- WalkableSurface

return entities
