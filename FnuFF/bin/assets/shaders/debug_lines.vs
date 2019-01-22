#version 420

layout(location=0) in vec3 vertStart;
layout(location=1) in vec3 vertEnd;
layout(location=2) in vec4 vertColor;

out vec3 geomEnd;
out vec3 geomColor;

void main()
{
	gl_Position = vec4( vertStart, 1.0 );
	geomEnd = vertEnd;
	geomColor = vertColor;
}