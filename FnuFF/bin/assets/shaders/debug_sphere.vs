#version 420

layout (location=0) in vec3 vertPosition;
layout (location=1) in float vertRadius;
layout (location=2) in vec4 vertColor;

out float geomRadius;
out vec4 geomColor;

void main()
{
	gl_Position = vec4( vertPosition, 1.0 );
	geomRadius = vertRadius;
	geomColor = vertColor;
}