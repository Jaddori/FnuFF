#version 440

in vec2 fragUV;

out vec4 finalColor;

uniform sampler2D diffuseMap;
uniform vec2 uvOffset;

void main()
{
	finalColor = texture( diffuseMap, fragUV + uvOffset );
}