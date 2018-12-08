local entities = {}

-- NewEntity
local NewEntity = Entity.create( "NewEntity", {16.84,0,34.68}, {0,0,0,1}, {1,1,1} )
NewEntity.visible = true
local NewEntity_component = ComponentMesh.create( NewEntity )
NewEntity_component:loadMesh( "./assets/models/box.mesh" )
NewEntity:addComponent( NewEntity_component )
local NewEntity_component = ComponentParticleEmitter.create( NewEntity )
NewEntity_component:setMaxParticles( 25 )
NewEntity_component.spherical = true
NewEntity_component.minFrequency = 0.01
NewEntity_component.maxFrequency = 0.05
NewEntity_component.minLifetime = 1.5
NewEntity_component.maxLifetime = 2.5
NewEntity_component.minDirection = {-0.25,1,-0.25}
NewEntity_component.maxDirection = {0.25,1,0.25}
NewEntity_component.startSpeed = 5
NewEntity_component.endSpeed = 3
NewEntity_component.startSize = 5
NewEntity_component.endSize = 3
NewEntity:addComponent( NewEntity_component )
local NewEntity_component = nil
entities[#entities+1] = NewEntity
-- NewEntity

return entities
