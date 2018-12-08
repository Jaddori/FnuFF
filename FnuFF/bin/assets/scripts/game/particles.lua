Particles =
{
	emitters = {}
}

function Particles:addEmitter( emitter )
	emitter.elapsedTime = 0
	emitter.curFrequency = lerp( emitter.minFrequency, emitter.maxFrequency, math.random() )
	emitter.update = Particles.updateEmitter

	emitter.textureIndex = Assets.loadTexture( "./assets/textures/cloud.dds" )
	emitter.normalIndex = Assets.loadTexture( "./assets/textures/normal.dds" )
	emitter.specularIndex = Assets.loadTexture( "./assets/textures/specular.dds" )
	emitter.maskIndex = Assets.loadTexture( "./assets/textures/mask.dds" )

	emitter.particles = {}
	for i=1, emitter.maxParticles do
		emitter.particles[i] =
		{
			alive = false,
			position = Vec3.create(),
			lifetime = 0,
			elapsed = 0,
			size = 0,
		}
	end

	self.emitters[#self.emitters+1] = emitter
end

function Particles:update( deltaTime )
	for _,emitter in pairs(self.emitters) do
		emitter:update( deltaTime )
	end
end

function Particles.updateEmitter( self, deltaTime )
	self.elapsedTime = self.elapsedTime + deltaTime

	-- spawn new particles
	if self.elapsedTime >= self.curFrequency then
		self.elapsedTime = self.elapsedTime - self.curFrequency
		self.curFrequency = lerp( self.minFrequency, self.maxFrequency, math.random() )

		for _,v in pairs(self.particles) do
			if not v.alive then
				v.alive = true

				v.position = self.position:copy()
				v.direction = Vec3.create
				({
					lerp( self.minDirection[1], self.maxDirection[1], math.random() ),
					lerp( self.minDirection[2], self.maxDirection[2], math.random() ),
					lerp( self.minDirection[3], self.maxDirection[3], math.random() )
				})
				v.lifetime = lerp( self.minLifetime, self.maxLifetime, math.random() )
				v.elapsed = 0
				v.scroll = Vec3.create
				({
					math.random(),
					math.random(),
					lerp( 0.25, 0.5, math.random() )
				})

				if math.random() < 0.5 then
					v.scroll[3] = -v.scroll[3]
				end

				break
			end
		end
	end

	-- update particles
	for _,v in pairs(self.particles) do
		if v.alive then
			v.elapsed = v.elapsed + deltaTime

			if v.elapsed >= v.lifetime then
				v.alive = false
			else
				local speed = easeOutCubic( v.elapsed, self.startSpeed, self.endSpeed - self.startSpeed, v.lifetime )

				v.position = v.position + ( v.direction * speed * deltaTime )
				v.size = easeOutCubic( v.elapsed, self.startSize, self.endSize - self.startSize, v.lifetime )
			end
		end
	end
end

function Particles:render()
	for _,emitter in pairs(self.emitters) do
		for _,particle in pairs(emitter.particles) do
			if particle.alive then
				Graphics.queueBillboard( emitter.textureIndex, emitter.normalIndex, emitter.specularIndex, emitter.maskIndex, particle.position, {particle.size, particle.size}, {0,0,1,1}, emitter.spherical, particle.scroll )
			end
		end
	end
end