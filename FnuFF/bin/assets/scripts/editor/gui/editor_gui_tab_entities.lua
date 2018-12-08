local ent = 
{
	items = {},
	position = Vec2.create({0,0}),
	size = Vec2.create({0,0}),
	depth = 0,

	onSelect = nil,
}

function ent:load( position, size, depth )
	self.position = position:copy()
	self.size = size:copy()
	self.depth = depth + GUI_DEPTH_INC
end

function ent:onShow()
	Log.debug( "ON SHOW" )
	for _,v in pairs(self.items) do
		v.text = v.tag.name
	end
end

function ent:addEntity( entity )
	local padding = 4
	local yoffset = #self.items * (GUI_BUTTON_HEIGHT + padding)

	local button = EditorButton.create
	(
		Vec2.create({self.position[1] + padding, self.position[2] + padding + yoffset}),
		Vec2.create({self.size[1] - padding*2, GUI_BUTTON_HEIGHT}),
		entity.name
	)
	button:setTextAlignment( ALIGN_NEAR, ALIGN_NEAR )
	button.depth = self.depth + GUI_DEPTH_SMALL_INC
	button.tag = entity
	button.onClick = self.onSelect
	
	self.items[#self.items+1] = button
end

function ent:removeEntity( entity )
	local index = 0
	for i=1, #self.items do
		if self.items[i].tag == entity then
			index = i
			break
		end
	end

	if index > 0 then
		self.items[index] = nil
	end
end

function ent:clear()
	local count = #self.items
	for i=1, count do
		self.items[i] = nil
	end
end

function ent:checkCapture( capture, mousePosition )
	for _,v in pairs(self.items) do
		v:checkCapture( capture, mousePosition )
	end

	if capture.depth < self.depth then
		if insideRect( self.position, self.size, mousePosition ) then
			capture.depth = self.depth
			capture.item = self
		end
	end
end

function ent:update( deltaTime, mousePosition )
	for _,v in pairs(self.items) do
		v:update( deltaTime, mousePosition )
	end
end

function ent:render()
	for _,v in pairs(self.items) do
		v:render()
	end
end

return ent