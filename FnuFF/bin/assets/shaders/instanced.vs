#version 440

layout(location=0) in vec3 vertPosition;
layout(location=1) in vec2 vertUV;
layout(location=2) in vec3 vertNormal;
layout(location=3) in vec3 vertTangent;
layout(location=4) in vec3 vertBitangent;

out vec2 fragUV;

layout(std140, binding = 0) uniform WorldMatrices
{
	mat4 worldMatrices[512];
};

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main()
{
	fragUV = vec2( vertUV.x, -vertUV.y );
	
	gl_Position = projectionMatrix * viewMatrix * worldMatrices[gl_InstanceID] * vec4( vertPosition, 1.0 );
}