local camera = 
{
	scrollSpeed = 5.0,
}

function camera:load()
	self.camera = Graphics.getPerspectiveCamera()
end

function camera:update( deltaTime )
	local capture = { mouseCaptured = false, keyboardCaptured = false }
	
	local mouseDelta = Input.getMouseDelta()
	
	if Input.buttonDown( Buttons.Middle ) then
		self.camera:updateDirection( mouseDelta )
		
		local movement = Vec3.create({0,0,0})
		if Input.keyDown( Keys.A ) then
			movement[1] = movement[1] - 1
		end

		if Input.keyDown( Keys.D ) then
			movement[1] = movement[1] + 1
		end

		if Input.keyDown( Keys.S ) then
			movement[3] = movement[3] - 1
		end

		if Input.keyDown( Keys.W ) then
			movement[3] = movement[3] + 1
		end

		self.camera:relativeMovement( movement )
		
		capture.mouseCaptured = true
		capture.keyboardCaptured = true
	else
		local mouseWheel = Input.getMouseWheel()
		if mouseWheel ~= 0 then
			local movement = Vec3.create({ 0, 0, mouseWheel * self.scrollSpeed })
			self.camera:relativeMovement( movement )

			capture.mouseCaptured = true
		end
	end
	
	return capture
end

return camera