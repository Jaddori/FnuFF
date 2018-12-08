#version 450

in vec2 fragUV;
in vec3 fragScroll;

layout(location=0) out vec4 finalColor;
layout(location=1) out vec4 finalAlpha;

uniform vec2 screenSize;
uniform sampler2D diffuseMap;
uniform sampler2D normalMap;
uniform sampler2D specularMap;
uniform sampler2D maskMap;
uniform sampler2D depthTarget;
uniform float deltaTime;

void main()
{
	// determine if fragment should be discarded
	vec2 depthUV = gl_FragCoord.xy / screenSize;
	float depthStored = texture( depthTarget, depthUV ).r;
	float fragDepth = gl_FragCoord.z;
	
	if( fragDepth > depthStored )
		discard;
		
	// color fragment
	//finalColor = texture( diffuseMap, fragUV );
	//finalAlpha = vec4( finalColor.a, finalColor.a, finalColor.a, 1.0 );
	
	float a1 = texture( diffuseMap, fragUV + fragScroll.xy + deltaTime * fragScroll.z ).a;
	float a2 = texture( diffuseMap, (fragUV*0.5) + fragScroll.xy + deltaTime * fragScroll.z*0.5 ).a;
	float a3 = texture( diffuseMap, (fragUV*2.0) + fragScroll.xy + deltaTime * fragScroll.z*1.5 ).a;
	float mask = texture( maskMap, fragUV ).r;
	
	finalColor = texture( diffuseMap, fragUV );
	finalColor.a = ((a1*a2*2)*a3*2)*mask;
	
	finalAlpha = vec4( finalColor.a, finalColor.a, finalColor.a, 1.0 );
}