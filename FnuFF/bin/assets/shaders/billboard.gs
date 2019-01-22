#version 420

layout(points) in;
layout(triangle_strip, max_vertices=4) out;

in vec4 geomUV[];
in vec2 geomSize[];
in float geomSpherical[];
in vec3 geomScroll[];

out vec2 fragUV;
out vec3 fragScroll;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main()
{	
	mat4 vp = projectionMatrix * viewMatrix;
	vec3 right = vec3( viewMatrix[0][0], viewMatrix[1][0], viewMatrix[2][0] );
	vec3 up = vec3( viewMatrix[0][1], viewMatrix[1][1], viewMatrix[2][1] ) * geomSpherical[0];
	up.y += 1.0 - geomSpherical[0];
	
	vec3 p = gl_in[0].gl_Position.xyz;
	vec2 minUV = geomUV[0].xy;
	vec2 maxUV = geomUV[0].zw;
	vec2 size = geomSize[0];
	
	// top left
	gl_Position = vp * vec4( p - right * 0.5 * size.x + up * 0.5 * size.y, 1.0 );
	fragUV = minUV;
	fragScroll = geomScroll[0];
	EmitVertex();
	
	// bottom left
	gl_Position = vp * vec4( p - right * 0.5 * size.x - up * 0.5 * size.y, 1.0 );
	fragUV = vec2( minUV.x, maxUV.y );
	fragScroll = geomScroll[0];
	EmitVertex();
	
	// top right
	gl_Position = vp * vec4( p + right * 0.5 * size.x + up * 0.5 * size.y, 1.0 );
	fragUV = vec2( maxUV.x, minUV.y );
	fragScroll = geomScroll[0];
	EmitVertex();
	
	// bottom right
	gl_Position = vp * vec4( p + right * 0.5 * size.x - up * 0.5 * size.y, 1.0 );
	fragUV = maxUV;
	fragScroll = geomScroll[0];
	EmitVertex();
	
	EndPrimitive();
}