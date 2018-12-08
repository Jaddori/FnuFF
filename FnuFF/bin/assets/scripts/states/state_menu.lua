StateMenu =
{
	name = "StateMenu",

	controls = {},

	capture =
	{
		depth = -1,
		button = -1,
		item = nil,
		focusItem = nil,
	},
}

function StateMenu.create( name )
	local result =
	{
		name = name,

		controls = {},

		capture =
		{
			depth = -1,
			button = -1,
			item = nil,
			focusItem = nil,
		},
	}

	setmetatable( result, { __index = StateMenu } )

	return result
end

function StateMenu:addControl( control )
	self.controls[#self.controls+1] = control
end

function StateMenu:update( deltaTime )
	local mousePosition = Input.getMousePosition()

	if self.capture.button == -1 then -- no button currently pressed
		if Input.buttonPressed( Buttons.Left ) then
			self.capture.button = Buttons.Left
		end

		if self.capture.button > -1 then
			local prevFocusItem = self.capture.focusItem

			-- check capture in all controls
			for _,v in pairs(self.controls) do
				v:checkCapture( self.capture, mousePosition )
			end

			-- focus item must be the same as capture item
			if self.capture.focusItem ~= self.capture.item then
				self.capture.focusItem = nil
			end

			-- unset focus from previous focus item
			if prevFocusItem and prevFocusItem ~= self.capture.focusItem then
				if prevFocusItem.unsetFocus then
					prevFocusItem:unsetFocus()
				end
			end

			-- set focus on new focus item
			if self.capture.focusItem then
				if self.capture.focusItem.setFocus then
					self.capture.focusItem:setFocus()
				end
			end

			-- press capture item
			if self.capture.item then
				if self.capture.item.press then
					self.capture.item:press( mousePosition )
				end
			end
		end
	else -- button currently pressed
		if Input.buttonReleased( self.capture.button ) then
			if self.capture.item then
				if self.capture.item.release then 
					self.capture.item:release( mousePosition )
				end
			end

			self.capture.depth = -1
			self.capture.item = nil
			self.capture.button = -1
		end
	end

	if self.capture.item then
		if self.capture.item.updateMouseInput then
			self.capture.item:updateMouseInput( deltaTime, mousePosition )
		end
	else
		if self.capture.focusItem then
			local stillFocused = self.capture.focusItem:updateKeyboardInput()
			if not stillFocused then
				self.capture.focusItem = nil
			end
		end

		for _,v in pairs(self.controls) do
			v:update( deltaTime, mousePosition )
		end
	end
end

function StateMenu:render()
	for _,v in pairs(self.controls) do
		v:render()
	end
end