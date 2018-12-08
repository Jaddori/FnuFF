#version 440

layout(points) in;
layout(triangle_strip, max_vertices = 4) out;

in vec2 geomHalfSize[];

out vec2 fragUV;
out vec4 fragPosition;
out mat3 fragTBN;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main()
{
	vec3 right = normalize( vec3( viewMatrix[0][0], viewMatrix[1][0], viewMatrix[2][0] ) );
	vec3 up = normalize( vec3( viewMatrix[0][1], viewMatrix[1][1], viewMatrix[2][1] ) );
	
	vec3 tangent = -up;
	vec3 bitangent = right;
	vec3 normal = normalize( cross( tangent, bitangent ) );
	fragTBN = mat3( tangent, bitangent, normal );
	
	fragPosition = vec4( gl_in[0].gl_Position.xyz - right * geomHalfSize[0].x + up * geomHalfSize[0].y, 1.0 );
	gl_Position = projectionMatrix * viewMatrix * fragPosition;
	fragUV = vec2( 0.0, 0.0 );
	EmitVertex();
	
	fragPosition = vec4( gl_in[0].gl_Position.xyz - right * geomHalfSize[0].x - up * geomHalfSize[0].y, 1.0 );
	gl_Position = projectionMatrix * viewMatrix * fragPosition;
	fragUV = vec2( 0.0, 1.0 );
	EmitVertex();
	
	fragPosition = vec4( gl_in[0].gl_Position.xyz + right * geomHalfSize[0].x + up * geomHalfSize[0].y, 1.0 );
	gl_Position = projectionMatrix * viewMatrix * fragPosition;
	fragUV = vec2( 1.0, 0.0 );
	EmitVertex();
	
	fragPosition = vec4( gl_in[0].gl_Position.xyz + right * geomHalfSize[0].x - up * geomHalfSize[0].y, 1.0 );
	gl_Position = projectionMatrix * viewMatrix * fragPosition;
	fragUV = vec2( 1.0, 1.0 );
	EmitVertex();
	
	EndPrimitive();
}