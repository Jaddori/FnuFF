ComponentScript =
{
	name = "Script",
	scriptFile = "",
}

ComponentScriptWindow = 
{
	window = {},
	component = {},
	prevText = "",
}

function ComponentScript.create( parent )
	local result = 
	{
		parent = parent,
		scriptFile = "",
	}

	setmetatable( result, { __index = ComponentScript } )

	return result
end

function ComponentScript:write( file, level, prefabName )
	local location = ""

	if self.parent then -- entity
		location = self.parent.name .. "_component"
		writeIndent( file, level, "local " .. location .. " = ComponentScript.create( " .. self.parent.name .. " )\n" )
	else -- prefab
		location = "Prefabs[\"" .. prefabName .. "\"].components[\"" .. self.name .. "\"]"
		writeIndent( file, level, location .. " = ComponentScript.create()\n" )
	end

	writeIndent( file, level, location .. ".scriptFile = \"" .. self.scriptFile .. "\"\n" )
end

function ComponentScript:compile( file, level )
	--[[local name = self.parent.name .. "_component"

	writeIndent( file, level, "local " .. name .. " = ComponentScript.create( " .. self.parent.name .. " )\n" )
	writeIndent( file, level, name .. ".scriptFile = \"" .. self.scriptFile .. "\"\n" )--]]
end

function ComponentScript:copy( parent )
	local result = self.create( parent )

	result.scriptFile = self.scriptFile
	result.scriptText = self.scriptText

	return result
end

function ComponentScript:select( ray )
	return -1
end

function ComponentScript:update( deltaTime )
end

function ComponentScript:render()
	return false
end

function ComponentScript:showInfoWindow()
	if ComponentScriptWindow.window.visible then
		ComponentScriptWindow:hide()
	else
		ComponentScriptWindow:show( self )
	end
end

-- WINDOW
function ComponentScriptWindow:show( component )
	self.component = component
	self.window.visible = true
	self.window.focused = true

	if self.window.onFocus then self.window:onFocus() end

	-- update items
	self.scriptFileInput.textbox:setText( self.component.scriptFile )
	self.updateButton.disabled = true
	self.revertButton.disabled = true
end

function ComponentScriptWindow:hide()
	self.window.visible = false
end

function ComponentScriptWindow:refresh( entity )
	if self.window.visible then
		if entity.components[ComponentScript.name] then
			self:show( entity.components[ComponentScript.name] )
		else
			self.window.visible = false
		end
	end
end

function ComponentScriptWindow:load()
	-- window
	self.window = EditorWindow.create( "Script Component" )
	self.window.position[1] = WINDOW_WIDTH - GUI_PANEL_WIDTH - self.window.size[1] - 8
	self.window.position[2] = GUI_MENU_HEIGHT + 8
	self.window.visible = false

	-- layout
	local layout = EditorLayoutTopdown.create( Vec2.create({0,0}), self.window.size[1] )

	-- script file input
	local scriptFileInput = EditorInputbox.createWithText( "Script file:" )
	scriptFileInput.textbox.onTextChanged = function( textbox )
		self.updateButton.disabled = false
		self.revertButton.disabled = false
	end
	layout:addItem( scriptFileInput )

	-- horizontal layout
	local horizontalLayout = EditorLayoutHorizontal.create( Vec2.create({0,0}), 0 )

	-- update button
	local updateButton = EditorButton.createWithText( "Update" )
	updateButton.disabled = true
	updateButton.disabledColor = Vec4.create({0.4, 0.4, 0.4, 1.0})
	updateButton.onClick = function( button )
		self.component.scriptFile = self.scriptFileInput.textbox.text
		self.prevText = self.scriptFileInput.textbox.text

		self.updateButton.disabled = true
		self.revertButton.disabled = true
	end
	horizontalLayout:addItem( updateButton )

	-- revert button
	local revertButton = EditorButton.createWithText( "Revert" )
	revertButton.disabled = true
	revertButton.disabledColor = Vec4.create({0.4, 0.4, 0.4, 1.0})
	revertButton.onClick = function( button )
		self.scriptFileInput.textbox:setText( self.prevText )

		self.updateButton.disabled = true
		self.revertButton.disabled = true
	end
	horizontalLayout:addItem( revertButton )
	
	layout:addItem( horizontalLayout )

	self.window:addItem( layout )

	-- set table references for easy access
	self.scriptFileInput = scriptFileInput
	self.updateButton = updateButton
	self.revertButton = revertButton
end

function ComponentScriptWindow:update( deltaTime, mousePosition )
	self.window:update( deltaTime, mousePosition )
end

function ComponentScriptWindow:render()
	self.window:render()
end

ComponentScriptWindow:load()

return ComponentScript, ComponentScriptWindow