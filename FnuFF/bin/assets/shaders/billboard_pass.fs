#version 440

in vec2 fragUV;
in vec4 fragPosition;
in mat3 fragTBN;

//out vec4 finalColor;
/*layout(location=0) out vec4 finalDiffuse;
layout(location=1) out vec4 finalPosition;
layout(location=2) out vec4 finalNormal;
layout(location=3) out vec4 finalAlpha;*/
layout(location=0) out vec4 finalColor;
layout(location=1) out vec4 finalAlpha;

uniform vec2 screenSize;
uniform sampler2D diffuseMap;
uniform sampler2D normalMap;
uniform sampler2D specularMap;
uniform sampler2D depthTarget;

void main()
{
	//finalColor = texture( diffuseMap, fragUV );
	
	vec2 depthUV = gl_FragCoord.xy / screenSize;
	float depthStored = texture( depthTarget, depthUV ).r;
	float fragDepth = gl_FragCoord.z;
	
	if( fragDepth > depthStored )
		discard;
	
	/*finalDiffuse = texture( diffuseMap, fragUV );
	finalPosition = fragPosition;
	
	vec3 normal = texture( normalMap, fragUV ).xyz;
	normal = normalize( normal * 2.0 - 1.0 );
	normal = normalize( fragTBN * normal );
	finalNormal = vec4( normal, 1.0 );
	
	finalAlpha = vec4( finalDiffuse.a, finalDiffuse.a, finalDiffuse.a, 1.0 );*/
	
	finalColor = texture( diffuseMap, fragUV );
	finalAlpha = vec4( finalColor.a, finalColor.a, finalColor.a, 1.0 );
}