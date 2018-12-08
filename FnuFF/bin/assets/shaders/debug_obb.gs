#version 330

layout(points) in;
layout(line_strip, max_vertices = 16) out;

in vec3 geomX[];
in vec3 geomY[];
in vec3 geomZ[];
in vec3 geomExtents[];
in vec4 geomColor[];

out vec4 fragColor;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main()
{
	vec3 halfLengths = geomExtents[0];
	mat4 viewProjection = projectionMatrix * viewMatrix;

	vec4 posX = vec4(geomX[0], 1.0)*halfLengths.x;
	vec4 negX = vec4(geomX[0], 1.0)*-halfLengths.x;
	vec4 posY = vec4(geomY[0], 1.0)*halfLengths.y;
	vec4 negY = vec4(geomY[0], 1.0)*-halfLengths.y;
	vec4 posZ = vec4(geomZ[0], 1.0)*halfLengths.z;
	vec4 negZ = vec4(geomZ[0], 1.0)*-halfLengths.z;
	
	vec4 nearBotRightPos = (gl_in[0].gl_Position + posX + negY +  negZ);
	vec4 nearBotLeftPos = (gl_in[0].gl_Position + negX + negY+  negZ);
	vec4 nearTopRightPos = (gl_in[0].gl_Position + posX + posY +  negZ);
	vec4 nearTopLeftPos = (gl_in[0].gl_Position + negX +posY +  negZ);

	vec4 farBotRightPos = (gl_in[0].gl_Position + posX + negY+  posZ);
	vec4 farBotLeftPos = (gl_in[0].gl_Position + negX + negY +  posZ);
	vec4 farTopRightPos = (gl_in[0].gl_Position +posX + posY +  posZ);
	vec4 farTopLeftPos = (gl_in[0].gl_Position + negX + posY +  posZ);

	nearBotRightPos.w = 1;
	nearBotLeftPos.w = 1;
	nearTopRightPos.w = 1;
	nearTopLeftPos.w = 1;

	farBotRightPos.w = 1;
	farBotLeftPos.w = 1;
	farTopRightPos.w = 1;
	farTopLeftPos.w = 1;

	vec4 nearBotRight = viewProjection * nearBotRightPos;
	vec4 nearBotLeft = viewProjection * nearBotLeftPos;
	vec4 nearTopRight= viewProjection * nearTopRightPos;
	vec4 nearTopLeft= viewProjection * nearTopLeftPos;

	vec4 farBotRight= viewProjection * farBotRightPos;
	vec4 farBotLeft= viewProjection * farBotLeftPos;
	vec4 farTopRight= viewProjection * farTopRightPos;
	vec4 farTopLeft= viewProjection * farTopLeftPos;

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