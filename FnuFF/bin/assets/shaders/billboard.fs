#version 450

in vec2 fragUV;
in vec3 fragScroll;

layout(location=0) out vec4 finalColor;

uniform sampler2D diffuseMap;
uniform sampler2D maskMap;
uniform float deltaTime;

void main()
{
	float a1 = texture( diffuseMap, fragUV + fragScroll.xy + deltaTime * fragScroll.z ).a;
	float a2 = texture( diffuseMap, (fragUV*0.5) + fragScroll.xy + deltaTime * fragScroll.z*0.5 ).a;
	float a3 = texture( diffuseMap, (fragUV*2.0) + fragScroll.xy + deltaTime * fragScroll.z*1.5 ).a;
	float mask = texture( maskMap, fragUV ).r;
	
	finalColor = texture( diffuseMap, fragUV );
	finalColor.a = ((a1*a2*2)*a3*2)*mask;
}