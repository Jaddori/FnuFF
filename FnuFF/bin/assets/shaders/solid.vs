#version 420

layout(location=0) in vec3 vertPosition;
layout(location=1) in vec2 vertUV;
layout(location=2) in vec2 vertLM;

out vec2 fragUV;
out vec2 fragLM;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main()
{
	fragUV = vertUV;
	fragLM = vertLM;
	gl_Position = projectionMatrix * viewMatrix * vec4( vertPosition, 1.0 );
}