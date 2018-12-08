#version 440

in vec4 fragPosition;
in vec2 fragUV;
in vec3 fragNormal;
in mat3 fragTBN;
in vec3 fragTangent;
in vec3 fragBitangent;

layout(location=0) out vec4 finalDiffuse;
layout(location=1) out vec4 finalPosition;
layout(location=2) out vec4 finalNormal;
layout(location=3) out vec4 finalDepth;

uniform sampler2D diffuseMap;
uniform sampler2D normalMap;

void main()
{
	vec2 uv = vec2( fragUV.s, 1 - fragUV.t );
	
	finalDiffuse = texture( diffuseMap, uv );
	
	vec3 normal = texture( normalMap, uv ).rgb;
	normal = normalize(normal * 2.0 - 1.0);
	normal = normalize( fragTBN * normal );
	finalNormal = vec4( normal, 1.0 );
	//finalNormal = vec4( fragTangent, 1.0 );
	
	finalPosition = fragPosition;
	//finalPosition = vec4( fragBitangent, 1.0 );
	
	float depth = gl_FragCoord.z;
	finalDepth = vec4( depth, depth, depth, 1.0 );
}