#version 440

layout(location=0) in vec3 vertPosition;
layout(location=1) in vec2 vertSize;
layout(location=2) in vec4 vertUV;
layout(location=3) in vec4 vertColor;

out vec2 geomSize;
out vec4 geomUV;
out vec4 geomColor;

void main()
{
	geomSize = vertSize;
	geomUV = vertUV;
	geomColor = vertColor;
	
	gl_Position = vec4( vertPosition, 1.0 );
}