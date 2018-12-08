#version 440

layout(location=0) in vec2 vertPosition;
layout(location=1) in vec2 vertUV;

out vec2 fragUV;

void main()
{
	gl_Position = vec4( vertPosition, 0.0, 1.0 );
	fragUV = vertUV * vec2( 1.0, -1.0 );
}