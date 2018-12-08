local entities = {}

-- First
local First = Entity.create( "First", {3.12,0,10.51}, {0,0,0,1}, {1,1,1} )
First.visible = true
local First_component = nil
entities[#entities+1] = First
-- First

-- Second
local Second = Entity.create( "Second", {7.12,0,1.13}, {0,0,0,1}, {1,1,1} )
Second.visible = true
local Second_component = nil
entities[#entities+1] = Second
-- Second

-- Last
local Last = Entity.create( "Last", {3.91,0,-7.7}, {0,0,0,1}, {1,1,1} )
Last.visible = true
local Last_component = nil
entities[#entities+1] = Last
-- Last

return entities
