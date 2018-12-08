#version 440

layout(location=0) in vec3 vertPosition;

//layout(std140, binding = 0) uniform WorldMatrices
//{
//	mat4 worldMatrices[512];
//};

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 worldMatrix;

void main()
{
	//gl_Position = projectionMatrix * viewMatrix * worldMatrices[gl_InstanceID] * vec4( vertPosition, 1.0 );
	gl_Position = projectionMatrix * viewMatrix * worldMatrix * vec4( vertPosition, 1.0 );
}