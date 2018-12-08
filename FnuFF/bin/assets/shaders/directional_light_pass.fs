#version 440

struct DirectionalLight
{
	vec3 direction;
	vec3 color;
	float intensity;
};

in vec2 fragUV;

out vec4 finalColor;

uniform DirectionalLight directionalLight;
uniform vec3 cameraPosition;
uniform float specularPower;
uniform mat4 lightTransformation;
uniform sampler2D diffuseTarget;
uniform sampler2D positionTarget;
uniform sampler2D normalTarget;
uniform sampler2D shadowTarget;

float calculateShadowFactor( vec3 position )
{
	vec4 shadowClip = lightTransformation * vec4( position, 1.0 );
	vec3 shadowNDC = ( ( shadowClip.xyz / shadowClip.w ) + 1.0 ) / 2.0;
	vec2 shadowUV = shadowNDC.xy;
	float shadowDepth = shadowNDC.z;
	float lightDepth = texture( shadowTarget, shadowUV ).r;
	
	float bias = 0.005;
	if( lightDepth < shadowDepth-bias )
	{
		return 0.0;
	}
	else
	{
		return 1.0;
	}
}

vec4 calculateDirectionalLight( vec3 normal, vec3 position )
{
	float diffuseFactor = dot( normal, -directionalLight.direction );
	vec4 diffuseColor = vec4( 0.0, 0.0, 0.0, 1.0 );
	vec4 specularColor = vec4( 0.0, 0.0, 0.0, 0.0 );
	
	if( diffuseFactor > 0.0 )
	{
		diffuseColor.rgb = directionalLight.color * directionalLight.intensity * diffuseFactor;
		
		vec3 directionToEye = normalize( cameraPosition - position );
		vec3 halfDirection = normalize( directionToEye - directionalLight.direction );
		
		float specularFactor = dot( halfDirection, normal );
		specularFactor = pow( specularFactor, specularPower );
		
		if( specularFactor > 0.0 )
		{
			specularColor.rgb = directionalLight.color * 2.0 * specularFactor;
		}
	}
	
	return diffuseColor + specularColor;
}

void main()
{
	vec3 diffuse = texture( diffuseTarget, fragUV ).rgb;
	vec3 position = texture( positionTarget, fragUV ).rgb;
	vec3 normal = texture( normalTarget, fragUV ).rgb;

	float shadowFactor = calculateShadowFactor( position );
	
	finalColor = vec4( diffuse, 1.0 ) * calculateDirectionalLight( normal, position ) * shadowFactor;
	//finalColor = vec4( diffuse, 1.0 ) * 0.01;
}