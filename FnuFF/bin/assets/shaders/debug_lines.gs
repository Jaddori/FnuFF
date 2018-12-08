#version 330

layout(points) in;
layout(line_strip, max_vertices = 2) out;

in vec3 geomEnd[];
in vec4 geomColor[];

out vec4 fragColor;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main()
{
	gl_Position = projectionMatrix * viewMatrix * gl_in[0].gl_Position;
	fragColor = geomColor[0];
	EmitVertex();
	
	gl_Position = projectionMatrix * viewMatrix * vec4( geomEnd[0], 1.0 );
	fragColor = geomColor[0];
	EmitVertex();
	
	EndPrimitive();
}