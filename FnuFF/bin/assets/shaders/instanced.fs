#version 440

in vec2 fragUV;

out vec4 finalColor;

uniform sampler2D diffuseMap;

void main()
{
	finalColor = texture( diffuseMap, fragUV );
}