#version 450

layout(location=0) in vec3 vertPosition;
layout(location=1) in vec4 vertUV;
layout(location=2) in vec2 vertSize;
layout(location=3) in float vertSpherical;
layout(location=4) in vec3 vertScroll;

out vec4 geomUV;
out vec2 geomSize;
out float geomSpherical;
out vec3 geomScroll;

void main()
{
	gl_Position = vec4( vertPosition, 1.0 );
	geomUV = vertUV;
	geomSize = vertSize;
	geomSpherical = vertSpherical;
	geomScroll = vertScroll;
}