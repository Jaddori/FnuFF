#version 330

layout(location=0) in vec3 vertPosition;
layout(location=1) in vec3 vertX;
layout(location=2) in vec3 vertY;
layout(location=3) in vec3 vertZ;
layout(location=4) in vec3 vertExtents;
layout(location=5) in vec4 vertColor;

out vec3 geomX;
out vec3 geomY;
out vec3 geomZ;
out vec3 geomExtents;
out vec4 geomColor;

void main()
{
	gl_Position = vec4( vertPosition, 1.0 );
	geomX = vertX;
	geomY = vertY;
	geomZ = vertZ;
	geomExtents = vertExtents;
	geomColor = vertColor;
}