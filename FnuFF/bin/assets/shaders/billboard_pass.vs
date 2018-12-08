#version 440

layout(location=0) in vec3 vertPosition;
layout(location=1) in vec2 vertHalfSize;

out vec2 geomHalfSize;

void main()
{
	gl_Position = vec4( vertPosition, 1.0 );
	geomHalfSize = vertHalfSize;
}