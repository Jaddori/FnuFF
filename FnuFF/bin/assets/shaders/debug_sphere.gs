#version 420

layout(points) in;
layout(line_strip, max_vertices = 48) out;

in float geomRadius[];
in vec4 geomColor[];

out vec4 fragColor;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

const float PI = 3.1415926;
const int TOTAL_DEGREE = 360;
const int LINES_PER_CIRCLE = 15;

void xCircle()
{
	const float INCREMENT = TOTAL_DEGREE / LINES_PER_CIRCLE;
	const float SEGMENT = INCREMENT / TOTAL_DEGREE;
	for( int i=0; i<LINES_PER_CIRCLE; i++ )
	{
		float rad = 2*PI * (i*SEGMENT);
		gl_Position = projectionMatrix * viewMatrix * (vec4(cos(rad) * geomRadius[0], 0, sin(rad) * geomRadius[0], 0) + gl_in[0].gl_Position);
		fragColor = geomColor[0];
		EmitVertex();
	}

	gl_Position = projectionMatrix * viewMatrix * (vec4(geomRadius[0], 0, 0, 0) + gl_in[0].gl_Position);
	fragColor = geomColor[0];
	EmitVertex();

	EndPrimitive();
}

void yCircle()
{
	const float INCREMENT = TOTAL_DEGREE / LINES_PER_CIRCLE;
	const float SEGMENT = INCREMENT / TOTAL_DEGREE;
	for( int i=0; i<LINES_PER_CIRCLE; i++ )
	{
		float rad = 2*PI * (i*SEGMENT);
		gl_Position = projectionMatrix * viewMatrix * (vec4(0, cos(rad) * geomRadius[0], sin(rad) * geomRadius[0], 0) + gl_in[0].gl_Position);
		fragColor = geomColor[0];
		EmitVertex();
	}

	gl_Position = projectionMatrix * viewMatrix * (vec4(0, geomRadius[0], 0, 0) + gl_in[0].gl_Position);
	fragColor = geomColor[0];
	EmitVertex();

	EndPrimitive();
}

void zCircle()
{
	const float INCREMENT = TOTAL_DEGREE / LINES_PER_CIRCLE;
	const float SEGMENT = INCREMENT / TOTAL_DEGREE;
	for( int i=0; i<LINES_PER_CIRCLE; i++ )
	{
		float rad = 2*PI * (i*SEGMENT);
		gl_Position = projectionMatrix * viewMatrix * ( vec4( cos(rad) * geomRadius[0], sin(rad) * geomRadius[0], 0, 0 ) + gl_in[0].gl_Position );
		fragColor = geomColor[0];
		EmitVertex();
	}

	gl_Position = projectionMatrix * viewMatrix * ( vec4( geomRadius[0], 0, 0, 0 ) + gl_in[0].gl_Position );
	fragColor = geomColor[0];
	EmitVertex();

	EndPrimitive();
}

void main()
{
	xCircle();
	yCircle();
	zCircle();
}