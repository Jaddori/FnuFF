local entities = {}

-- NewEntity
local NewEntity = Entity.create( "NewEntity", {0,0,0}, {0,0,0,1}, {10,1,10} )
NewEntity.visible = true
local NewEntity_component = ComponentMesh.create( NewEntity )
NewEntity_component:loadMesh( "plane.mesh" )
NewEntity:addComponent( NewEntity_component )
local NewEntity_component = nil
entities[#entities+1] = NewEntity
-- NewEntity

-- NewEntity
local NewEntity = Entity.create( "NewEntity", {-4.73,0,-2.48}, {0,0,0,1}, {1,1,1} )
NewEntity.visible = true
local NewEntity_component = ComponentMesh.create( NewEntity )
NewEntity_component:loadMesh( "pillar.mesh" )
NewEntity:addComponent( NewEntity_component )
local NewEntity_component = nil
entities[#entities+1] = NewEntity
-- NewEntity

-- NewEntity
local NewEntity = Entity.create( "NewEntity", {4.6,0,6.67}, {0,0,0,1}, {1,1,1} )
NewEntity.visible = true
local NewEntity_component = ComponentMesh.create( NewEntity )
NewEntity_component:loadMesh( "pillar.mesh" )
NewEntity:addComponent( NewEntity_component )
local NewEntity_component = nil
entities[#entities+1] = NewEntity
-- NewEntity

-- NewEntity
local NewEntity = Entity.create( "NewEntity", {-0.62,2.24,3.08}, {0,0,0,1}, {1,1,1} )
NewEntity.visible = true
local NewEntity_component = ComponentPointLight.create( NewEntity )
NewEntity_component.position = {-0.62,2.24,3.08}
NewEntity_component.offset = {0,0,0}
NewEntity_component.color = {1,0.2,0.2}
NewEntity_component.intensity = 2
NewEntity_component.size = 1
NewEntity:addComponent( NewEntity_component )
local NewEntity_component = nil
entities[#entities+1] = NewEntity
-- NewEntity

-- NewEntity
local NewEntity = Entity.create( "NewEntity", {12.93,9.29,17.53}, {0,0,0,1}, {1,1,1} )
NewEntity.visible = true
local NewEntity_component = ComponentDirectionalLight.create( NewEntity )
NewEntity_component.direction = {-1,-1,-1}
NewEntity_component.color = {0.25,1,0.25}
NewEntity_component.intensity = 0.1
NewEntity:addComponent( NewEntity_component )
local NewEntity_component = nil
entities[#entities+1] = NewEntity
-- NewEntity

return entities
