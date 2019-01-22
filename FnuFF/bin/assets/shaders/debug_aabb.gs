#version 420

layout(points) in;
layout(line_strip, max_vertices = 16) out;

in vec3 geomMax[];
in vec4 geomColor[];

out vec4 fragColor;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main()
{
	mat4 viewProjection = projectionMatrix * viewMatrix;

	vec4 nearBotRight = viewProjection * vec4(geomMax[0].x, gl_in[0].gl_Position.y, gl_in[0].gl_Position.z, 1);
	vec4 nearBotLeft = viewProjection * gl_in[0].gl_Position;
	vec4 nearTopRight = viewProjection * vec4(geomMax[0].x, geomMax[0].y, gl_in[0].gl_Position.z,1);
	vec4 nearTopLeft = viewProjection * vec4(gl_in[0].gl_Position.x, geomMax[0].y, gl_in[0].gl_Position.z,1);

	vec4 farBotRight = viewProjection * vec4(geomMax[0].x, gl_in[0].gl_Position.y, geomMax[0].z,1);
	vec4 farBotLeft = viewProjection * vec4(gl_in[0].gl_Position.x, gl_in[0].gl_Position.y, geomMax[0].z,1);
	vec4 farTopRight = viewProjection * vec4(geomMax[0], 1.0);
	vec4 farTopLeft = viewProjection * vec4(gl_in[0].gl_Position.x, geomMax[0].y, geomMax[0].z,1);

	gl_Position = farTopLeft;
	fragColor = geomColor[0];
	EmitVertex();
	gl_Position = farBotLeft;
	fragColor = geomColor[0];
	EmitVertex();
	gl_Position = nearBotLeft;
	fragColor = geomColor[0];
	EmitVertex();
	gl_Position = nearBotRight;
	fragColor = geomColor[0];
	EmitVertex();
	gl_Position = farBotRight;
	fragColor = geomColor[0];
	EmitVertex();
	gl_Position = farBotLeft;
	fragColor = geomColor[0];
	EmitVertex();

	EndPrimitive();

	gl_Position = nearBotLeft;
	fragColor = geomColor[0];
	EmitVertex();
	gl_Position = nearTopLeft;
	fragColor = geomColor[0];
	EmitVertex();
	gl_Position = farTopLeft;
	fragColor = geomColor[0];
	EmitVertex();
	gl_Position = farTopRight;
	fragColor = geomColor[0];
	EmitVertex();
	gl_Position = nearTopRight;
	fragColor = geomColor[0];
	EmitVertex();
	gl_Position = nearBotRight;
	fragColor = geomColor[0];
	EmitVertex();

	EndPrimitive();

	gl_Position = nearTopLeft;
	fragColor = geomColor[0];
	EmitVertex();
	gl_Position = nearTopRight;
	fragColor = geomColor[0];
	EmitVertex();
	gl_Position = farTopRight;
	fragColor = geomColor[0];
	EmitVertex();
	gl_Position = farBotRight;
	fragColor = geomColor[0];
	EmitVertex();

	EndPrimitive();
}