local entities = {}

-- NewEntity
local NewEntity = Entity.create( "NewEntity", {13.21,0,15.01}, {0,0,0,1}, {1,1,1} )
NewEntity.visible = true
local NewEntity_component = ComponentParticleEmitter.create( NewEntity )
NewEntity_component:setMaxParticles( 1 )
NewEntity_component.spherical = true
NewEntity_component.minFrequency = 0.1
NewEntity_component.maxFrequency = 0.5
NewEntity_component.minLifetime = 1.5
NewEntity_component.maxLifetime = 2.5
NewEntity_component.minDirection = {-1,-1,-1}
NewEntity_component.maxDirection = {1,1,1}
NewEntity_component.startSpeed = 1
NewEntity_component.endSpeed = 0.1
NewEntity_component.startSize = 5
NewEntity_component.endSize = 4
NewEntity:addComponent( NewEntity_component )
local NewEntity_component = nil
entities[#entities+1] = NewEntity
-- NewEntity

-- NewEntity
local NewEntity = Entity.create( "NewEntity", {19.53,0,20.77}, {0,0,0,1}, {1,1,1} )
NewEntity.visible = true
local NewEntity_component = nil
entities[#entities+1] = NewEntity
-- NewEntity

return entities
