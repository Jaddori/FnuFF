local scripts = {}

function run( func, ... )
	for k,v in pairs(scripts) do
		if v[func] then
			v[func]( v, ... )
		end
	end
end

function mainLoad()
	doscript( "utils.lua" )
	doscript( "debug/info.lua" )
	doscript( "game.lua" )
	
	run( "load" )
end

function mainUnload()
	run( "unload" )
end

function mainUpdate( deltaTime )
	run( "update", deltaTime )
end

function mainFixedUpdate( timestep )
	run( "fixedUpdate" )
end

function mainRender()
	run( "render" )
end

function mainClientWrite()
	run( "clientWrite" )
end

function mainClientRead()
	run( "clientRead" )
end

function mainServerWrite()
	run( "serverWrite" )
end

function mainServerRead()
	run( "serverRead" )
end

function addScript( script )
	scripts[script.name] = script
end

function doscript( name )
	return dofile( "./assets/scripts/" .. name )
end