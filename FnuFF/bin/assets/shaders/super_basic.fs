#version 420

in vec2 fragUV;

out vec4 finalColor;

uniform sampler2D fragDiffuse;

void main()
{
	//finalColor = vec4( fragColor, 1.0 );
	finalColor = texture( fragDiffuse, fragUV );
	//finalColor = vec4( fragUV, 1.0, 1.0 );
}