#version 440

layout(location=0) in vec3 vertPosition;
layout(location=1) in vec2 vertUV;

out vec2 fragUV;

uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

void main()
{
	fragUV = vec2( vertUV.x, -vertUV.y );
	
	gl_Position = projectionMatrix * viewMatrix * vec4( vertPosition, 1.0 );
}