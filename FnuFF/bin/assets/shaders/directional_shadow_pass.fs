#version 440

out vec4 finalColor;

void main()
{
	float z = gl_FragCoord.z;
	finalColor = vec4( z, z, z, 1.0 );
}