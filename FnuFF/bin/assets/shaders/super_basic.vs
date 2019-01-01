#version 420

layout(location=0) in vec3 vertPosition;
layout(location=1) in vec2 vertUV;

out vec2 fragUV;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main()
{
	fragUV = vertUV;
	gl_Position = projectionMatrix * viewMatrix * vec4( vertPosition, 1.0 );
}