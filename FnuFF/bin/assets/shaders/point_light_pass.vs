#version 440

layout(location=0) in vec3 vertPosition;
layout(location=1) in vec2 vertUV;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 worldMatrix;

void main()
{
	gl_Position = projectionMatrix * viewMatrix * worldMatrix * vec4( vertPosition, 1.0 );
}