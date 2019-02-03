#version 420

in vec2 fragUV;
in vec2 fragLM;

out vec4 finalColor;

uniform sampler2D fragDiffuse;
uniform sampler2D fragLightmap;

void main()
{
	//vec4 brightness = texture( fragLightmap, fragLM );
	//finalColor = texture( fragDiffuse, fragUV ) * brightness;
	//finalColor.a = 1.0;
	
	//finalColor = texture( fragDiffuse, fragUV );
	finalColor = texture( fragLightmap, fragLM );
}