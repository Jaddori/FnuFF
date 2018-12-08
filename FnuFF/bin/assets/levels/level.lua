local entities = {}

-- NewEntity
local NewEntity = Entity.create( "NewEntity", {12.63,0,27.73}, {0,0,0,1}, {1,1,1} )
local NewEntity_component = ComponentMesh.create( NewEntity )
NewEntity_component:loadMesh( "./assets/models/arrow.mesh" )
NewEntity:addComponent( NewEntity_component )
local NewEntity_component = nil
entities[#entities+1] = NewEntity
-- NewEntity

-- NewEntity
local NewEntity = Entity.create( "NewEntity", {17,0,11.27}, {0,0,0,1}, {1,1,1} )
local NewEntity_component = ComponentMesh.create( NewEntity )
NewEntity_component:loadMesh( "./assets/models/pillar.mesh" )
NewEntity:addComponent( NewEntity_component )
local NewEntity_component = nil
entities[#entities+1] = NewEntity
-- NewEntity

-- NewEntity
local NewEntity = Entity.create( "NewEntity", {36.78,0,10.02}, {0,0,0,1}, {1,1,1} )
local NewEntity_component = ComponentWalkable.create( NewEntity )
NewEntity_component.size = {20,20}
NewEntity_component.interval = 1
NewEntity:addComponent( NewEntity_component )
local NewEntity_component = nil
entities[#entities+1] = NewEntity
-- NewEntity

-- NewEntity
local NewEntity = Entity.create( "NewEntity", {28.77,0,30.43}, {0,0,0,1}, {1,1,1} )
local NewEntity_component = ComponentParticleEmitter.create( NewEntity )
NewEntity_component.maxParticles = 5
NewEntity_component.spherical = true
NewEntity_component.minFrequency = 0.1
NewEntity_component.maxFrequency = 0.5
NewEntity_component.minLifetime = 1.5
NewEntity_component.maxLifetime = 2.5
NewEntity_component.minDirection = {0,1,0}
NewEntity_component.maxDirection = {0,1,0}
NewEntity_component.startSpeed = 5
NewEntity_component.endSpeed = 3
NewEntity_component.startSize = 5
NewEntity_component.endSize = 4
NewEntity:addComponent( NewEntity_component )
local NewEntity_component = nil
entities[#entities+1] = NewEntity
-- NewEntity

return entities
