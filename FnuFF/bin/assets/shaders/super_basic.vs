#version 420

layout(location=0) in vec3 vertPosition;
layout(location=1) in vec3 vertNormal;
layout(location=2) in vec3 vertColor;

out vec3 fragColor;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main()
{
	fragColor = vertColor;
	gl_Position = projectionMatrix * viewMatrix * vec4( vertPosition, 1.0 );
}