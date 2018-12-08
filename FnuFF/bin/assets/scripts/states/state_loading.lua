STATE_LOADING_FONT_INFO = "./assets/fonts/verdana18.bin"
STATE_LOADING_FONT_TEXTURE = "./assets/fonts/verdana18.dds"

StateLoading =
{
	name = "Loading",
	text = "Loading...",
	label = {},

	returnState = {},
	hasRendered = false,
	hasLoaded = false,
}

function StateLoading:load()
	self.label = EditorLabel.create( vec(0,0), vec(WINDOW_WIDTH, WINDOW_HEIGHT), self.text )
	self.label:loadFont( STATE_LOADING_FONT_INFO, STATE_LOADING_FONT_TEXTURE )
	self.label:setTextAlignment( ALIGN_MIDDLE, ALIGN_MIDDLE )
end

function StateLoading:show( returnState )
	self.returnState = returnState
	self.label:setText( self.text )
	self.hasRendered = false

	Game:pushState( "Loading" )
end

function StateLoading:update( deltaTime )
	if self.hasRendered and not self.hasLoaded then
		self.returnState:load()
		Game:setState( self.returnState.name )

		self.hasLoaded = true
	end
end

function StateLoading:render()
	self.label:render()
	self.hasRendered = true
end

Game:addState( StateLoading )