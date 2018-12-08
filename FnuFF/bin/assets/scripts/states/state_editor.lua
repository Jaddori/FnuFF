StateEditor =
{
	name = "StateEditor",
}

function StateEditor:load()
	Input.setUpdateBound( false )

	dofile( "./assets/scripts/editor/editor.lua" )
	
	Editor:load()
end

function StateEditor:unload()
	Editor:unload()
end

function StateEditor:update( deltaTime )
	Editor:update( deltaTime )
end

function StateEditor:fixedUpdate()
end

function StateEditor:render()
	Editor:render()
end

--Game:addState( StateEditor )