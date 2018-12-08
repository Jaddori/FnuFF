#version 440

layout(location=0) in vec3 vertPosition;
layout(location=1) in vec4 vertUV;
layout(location=2) in vec2 vertSize;
layout(location=3) in vec4 vertColor;

out vec4 geomUV;
out vec2 geomSize;
out vec4 geomColor;

void main()
{
	geomUV = vertUV;
	geomSize = vertSize;
	geomColor = vertColor;
	
	gl_Position = vec4( vertPosition, 1.0 );
}