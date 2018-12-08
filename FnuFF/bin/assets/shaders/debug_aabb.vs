#version 330

layout(location=0) in vec3 vertMin;
layout(location=1) in vec3 vertMax;
layout(location=2) in vec4 vertColor;

out vec3 geomMax;
out vec4 geomColor;

void main()
{
	gl_Position = vec4( vertMin, 1.0 );
	geomMax = vertMax;
	geomColor = vertColor;
}