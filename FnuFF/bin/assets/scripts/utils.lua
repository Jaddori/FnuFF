function insideRect( position, size, point )
	assert( position, "Position was nil." )
	assert( size, "Size was nil." )
	assert( point, "Point was nil." )

	assert( istable( position ), "Position must be a table." )
	assert( istable( size ), "Size must be a table." )
	assert( istable( point ), "Point must be a table." )

	assert( #position >= 2, "Position must contain at least 2 elements." )
	assert( #size >= 2, "Size must contain at least 2 elements." )
	assert( #point >= 2, "Point must contain at least 2 elements." )

	return ( point[1] >= position[1] and
				point[2] >= position[2] and
				point[1] <= position[1] + size[1] and
				point[2] <= position[2] + size[2] );
end

function stringVec( vec )
	assert( istable( vec ), "Input must be a table." )

	local result = tostring( roundTo( vec[1], 2 ) )
	
	for i=2, #vec do
		result = result .. "," .. tostring( roundTo( vec[i], 2 ) )
	end
	
	return result
end

function vecString( str )
	assert( isstring( str ), "Input must be a string." )

	local result = {}
	local components = split( str, "," )

	for i=1, #components do
		result[i] = tonumber( components[i] )
	end

	return result
end

function split( str, delimiter )
	assert( isstring( str ), "Input must be a string." )

	local result = {}
	
	local req = string.format("([^%s]+)", delimiter)
	for word in str:gmatch(req) do
		result[#result+1] = word
	end
	
	return result
end

function roundTo(num, numDecimalPlaces)
	local mult = 10^(numDecimalPlaces or 0)
	return math.floor(num * mult + 0.5) / mult
end

function writeIndent( file, level, text )
	file:write( string.rep( "\t", level ) .. text )
end

function lerp( a, b, t )
	return a + (b-a)*t
end

function easeInCubic( t, b, c, d )
	t = t / d
	return c * math.pow(t, 3) + b
end
  
function easeOutCubic( t, b, c, d )
	t = t / d - 1
	return c * (math.pow(t, 3) + 1) + b
end

function tableVal( t, size, default )
	local result = {}

	if t then
		for i=1, #t do
			result[i] = t[i]
		end
	else
		size = size or 2
		default = default or 0
		for i=1, size do
			result[i] = default
		end
	end

	return result
end

function stringBits( num )
	local result = ""

	for i=0, 4 do
		for j=0, 4 do
			result = result .. tostring( bit32.extract( num, 31 - (i*4+j) ) )
		end

		result = result .. " "
	end

	return result
end

function vec( x, y, z, w )
	if w then
		return Vec4.create({x,y,z,w})
	elseif z then
		return Vec3.create({x,y,z})
	elseif y then
		return Vec2.create({x,y})
	end

	return Vec2.create({x,y})
end

function isnumber( a )
	return type(a) == "number"
end

function isstring( a )
	return type(a) == "string"
end

function istable( a )
	return type(a) == "table"
end

function isuserdata( a )
	return type(a) == "userdata"
end

function isfunction( a )
	return type(a) == "function"
end

function isboolean( a )
	return type(a) == "boolean"
end
