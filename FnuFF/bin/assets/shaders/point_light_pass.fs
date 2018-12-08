#version 440

struct PointLight
{
	vec3 position;
	vec3 color;
	float intensity;
	float linear;
	float constant;
	float exponent;
};

in vec2 fragUV;

out vec4 finalColor;

uniform PointLight pointLight;
uniform vec3 cameraPosition;
uniform vec2 screenSize;
uniform float specularPower;
uniform sampler2D diffuseTarget;
uniform sampler2D positionTarget;
uniform sampler2D normalTarget;

vec4 calculatePointLight( vec3 normal, vec3 position )
{
	vec3 lightDirection = ( position - pointLight.position );
	float dist = length( lightDirection );
	
	lightDirection = normalize( lightDirection );

	vec4 diffuseColor = vec4( 0.0, 0.0, 0.0, 0.0 );
	vec4 specularColor = vec4( 0.0, 0.0, 0.0, 0.0 );
	
	float diffuseFactor = dot( normal, -lightDirection );
	if( diffuseFactor > 0.0 )
	{
		diffuseColor.rgb = pointLight.color * pointLight.intensity * diffuseFactor;
		
		vec3 directionToEye = normalize( cameraPosition - position );
		vec3 halfDirection = normalize( directionToEye - lightDirection );
		
		float specularFactor = dot( halfDirection, normal );
		specularFactor = pow( specularFactor, specularPower );
		if( specularFactor > 0.0 )
		{
			specularColor.rgb = pointLight.color * 2.0 * specularFactor;
		}
	}
	
	float attenuation = pointLight.constant +
						pointLight.linear * dist +
						pointLight.exponent * dist * dist +
						0.0001; // prevent division by 0
	
	return ( ( diffuseColor + specularColor ) / attenuation );
}

void main()
{
	vec2 uv = gl_FragCoord.xy / screenSize;
	vec3 diffuse = texture( diffuseTarget, uv ).rgb;
	vec3 position = texture( positionTarget, uv ).rgb;
	vec3 normal = normalize( texture( normalTarget, uv ).rgb );
	
	finalColor = vec4( diffuse, 1.0 ) * calculatePointLight( normal, position );
	//finalColor = vec4( diffuse, 1.0 );
	//finalColor = vec4( 1.0, 0.0, 0.0, 1.0 );
}