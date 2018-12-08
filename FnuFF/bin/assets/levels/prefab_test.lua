-- camera
Editor.camera.camera:setPosition( {36.96,23.41,33.1} )
Editor.camera.camera:setDirection( {-0.55,-0.56,-0.62} )

-- prefabs
Prefabs["Big"] =
{
	name = "Big",
	instances = {},
	components = {},
}
setmetatable( Prefabs["Big"], { __index = Prefab } )

Prefabs["Big"].components["Directional Light"] = ComponentDirectionalLight.create()
Prefabs["Big"].components["Directional Light"].direction = {1,0,0}
Prefabs["Big"].components["Directional Light"].color = {1,1,1}
Prefabs["Big"].components["Directional Light"].intensity = 2
Prefabs["Big"].components["Walkable"] = ComponentWalkable.create()
Prefabs["Big"].components["Walkable"].size = {20,20}
Prefabs["Big"].components["Walkable"].interval = 1
Prefabs["Big"].components["Bounding Box"] = ComponentBoundingBox.create()
Prefabs["Big"].components["Bounding Box"].type = 3
Prefabs["Big"].components["Bounding Box"].offset = {0,0,0}
Prefabs["Big"].components["Bounding Box"].ray.start = {0,0,0}
Prefabs["Big"].components["Bounding Box"].ray.length = 5
Prefabs["Big"].components["Bounding Box"].ray.direction = {0.58,0.58,0.58}
Prefabs["Big"].components["Bounding Box"].sphere.center = {0,0,0}
Prefabs["Big"].components["Bounding Box"].sphere.radius = 2
Prefabs["Big"].components["Bounding Box"].aabb.minPosition = {-1.23,-2,19.12}
Prefabs["Big"].components["Bounding Box"].aabb.maxPosition = {2.77,2,23.12}
Prefabs["Big"].components["Bounding Box"].aabb.minOffset = {-2,-2,-2}
Prefabs["Big"].components["Bounding Box"].aabb.maxOffset = {2,2,2}
Prefabs["Big"].components["Mesh"] = ComponentMesh.create()
Prefabs["Big"].components["Mesh"]:loadMesh( "cube.mesh" )
Prefabs["Big"].components["Particle Emitter"] = ComponentParticleEmitter.create()
Prefabs["Big"].components["Particle Emitter"]:setMaxParticles( 1 )
Prefabs["Big"].components["Particle Emitter"].spherical = true
Prefabs["Big"].components["Particle Emitter"].minFrequency = 0.1
Prefabs["Big"].components["Particle Emitter"].maxFrequency = 0.5
Prefabs["Big"].components["Particle Emitter"].minLifetime = 1.5
Prefabs["Big"].components["Particle Emitter"].maxLifetime = 2.5
Prefabs["Big"].components["Particle Emitter"].minDirection = {-1,-1,-1}
Prefabs["Big"].components["Particle Emitter"].maxDirection = {1,1,1}
Prefabs["Big"].components["Particle Emitter"].startSpeed = 1
Prefabs["Big"].components["Particle Emitter"].endSpeed = 0.1
Prefabs["Big"].components["Particle Emitter"].startSize = 5
Prefabs["Big"].components["Particle Emitter"].endSize = 4
Prefabs["Big"].components["Point Light"] = ComponentPointLight.create()
Prefabs["Big"].components["Point Light"].position = {0,0,0}
Prefabs["Big"].components["Point Light"].offset = {0,0,0}
Prefabs["Big"].components["Point Light"].color = {1,1,1}
Prefabs["Big"].components["Point Light"].intensity = 2
Prefabs["Big"].components["Point Light"].size = 1

--entities
local entities = {}

-- First
local First = Entity.create( "First", {0.77,0,21.12}, {0,0,0,1}, {1,1,1} )
First.visible = true
First.prefab = Prefabs["Big"]
Prefabs["Big"].instances[#Prefabs["Big"].instances+1] = First
local First_component = ComponentDirectionalLight.create( First )
First_component.direction = {1,0,0}
First_component.color = {1,1,1}
First_component.intensity = 2
First:addComponent( First_component )
local First_component = ComponentWalkable.create( First )
First_component.size = {20,20}
First_component.interval = 1
First:addComponent( First_component )
local First_component = ComponentBoundingBox.create( First )
First_component.type = 3
First_component.offset = {0,0,0}
First_component.ray.start = {0.77,0,21.12}
First_component.ray.length = 5
First_component.ray.direction = {0.58,0.58,0.58}
First_component.sphere.center = {0.77,0,21.12}
First_component.sphere.radius = 2
First_component.aabb.minPosition = {-1.23,-2,19.12}
First_component.aabb.maxPosition = {2.77,2,23.12}
First_component.aabb.minOffset = {-2,-2,-2}
First_component.aabb.maxOffset = {2,2,2}
First:addComponent( First_component )
local First_component = ComponentMesh.create( First )
First_component:loadMesh( "cube.mesh" )
First:addComponent( First_component )
local First_component = ComponentParticleEmitter.create( First )
First_component:setMaxParticles( 1 )
First_component.spherical = true
First_component.minFrequency = 0.1
First_component.maxFrequency = 0.5
First_component.minLifetime = 1.5
First_component.maxLifetime = 2.5
First_component.minDirection = {-1,-1,-1}
First_component.maxDirection = {1,1,1}
First_component.startSpeed = 1
First_component.endSpeed = 0.1
First_component.startSize = 5
First_component.endSize = 4
First:addComponent( First_component )
local First_component = ComponentPointLight.create( First )
First_component.position = {0.77,0,21.12}
First_component.offset = {0,0,0}
First_component.color = {1,1,1}
First_component.intensity = 2
First_component.size = 1
First:addComponent( First_component )
local First_component = nil
entities[#entities+1] = First
-- First

-- Last
local Last = Entity.create( "Last", {22.52,0,-0.75}, {0,0,0,1}, {1,1,1} )
Last.visible = true
Last.prefab = Prefabs["Big"]
Prefabs["Big"].instances[#Prefabs["Big"].instances+1] = Last
local Last_component = ComponentDirectionalLight.create( Last )
Last_component.direction = {1,0,0}
Last_component.color = {1,1,1}
Last_component.intensity = 2
Last:addComponent( Last_component )
local Last_component = ComponentWalkable.create( Last )
Last_component.size = {20,20}
Last_component.interval = 1
Last:addComponent( Last_component )
local Last_component = ComponentBoundingBox.create( Last )
Last_component.type = 3
Last_component.offset = {0,0,0}
Last_component.ray.start = {22.52,0,-0.75}
Last_component.ray.length = 5
Last_component.ray.direction = {0.58,0.58,0.58}
Last_component.sphere.center = {22.52,0,-0.75}
Last_component.sphere.radius = 2
Last_component.aabb.minPosition = {20.52,-2,-2.75}
Last_component.aabb.maxPosition = {24.52,2,1.25}
Last_component.aabb.minOffset = {-2,-2,-2}
Last_component.aabb.maxOffset = {2,2,2}
Last:addComponent( Last_component )
local Last_component = ComponentMesh.create( Last )
Last_component:loadMesh( "cube.mesh" )
Last:addComponent( Last_component )
local Last_component = ComponentParticleEmitter.create( Last )
Last_component:setMaxParticles( 1 )
Last_component.spherical = true
Last_component.minFrequency = 0.1
Last_component.maxFrequency = 0.5
Last_component.minLifetime = 1.5
Last_component.maxLifetime = 2.5
Last_component.minDirection = {-1,-1,-1}
Last_component.maxDirection = {1,1,1}
Last_component.startSpeed = 1
Last_component.endSpeed = 0.1
Last_component.startSize = 5
Last_component.endSize = 4
Last:addComponent( Last_component )
local Last_component = ComponentPointLight.create( Last )
Last_component.position = {22.52,0,-0.75}
Last_component.offset = {0,0,0}
Last_component.color = {1,1,1}
Last_component.intensity = 2
Last_component.size = 1
Last:addComponent( Last_component )
local Last_component = nil
entities[#entities+1] = Last
-- Last

return entities
