Lights =
{
	directionalLights = {},
	pointLights = {},
}

function Lights:addDirectionalLight( light )
	self.directionalLights[#self.directionalLights+1] = light
end

function Lights:addPointLight( light )
	self.pointLights[#self.pointLights+1] = light
end

function Lights:render()
	for _,v in pairs(self.directionalLights) do
		Graphics.queueDirectionalLight( v.direction, v.color, v.intensity )
	end

	for _,v in pairs(self.pointLights) do
		Graphics.queuePointLight( v.position, v.color, v.intensity, v.linear, v.constant, v.exponent )
	end
end