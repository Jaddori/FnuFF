#version 420

in vec2 fragUV;
in vec2 fragLM;

out vec4 finalColor;

uniform sampler2D fragDiffuse;
uniform sampler2D fragLightmap;

void main()
{
	//float brightness = texture( fragLightmap, fragLM ).r;
	//finalColor = texture( fragDiffuse, fragUV ) * brightness;
	
	//finalColor = texture( fragDiffuse, fragUV );
	finalColor = texture( fragLightmap, fragLM );
}