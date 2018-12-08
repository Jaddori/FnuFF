#include "gbuffer.h"
using namespace Rendering;

Gbuffer::Gbuffer()
{
	LOG( VERBOSITY_INFORMATION, "Constructing." );
}

Gbuffer::~Gbuffer()
{
	LOG( VERBOSITY_INFORMATION, "Destructing." );
}

bool Gbuffer::load( Assets* a, int w, int h )
{
	bool result = true;

	LOG( VERBOSITY_INFORMATION, "Loading shaders." );

	width = w;
	height = h;
	assets = a;

	// GEOMETRY PASS
	if( geometryPass.load( "./assets/shaders/geometry_pass.vs",
							NULL,
							"./assets/shaders/geometry_pass.fs" ) )
	{
		geometryPass.bind();

		geometryProjectionMatrix = geometryPass.getLocation( "projectionMatrix" );
		geometryViewMatrix = geometryPass.getLocation( "viewMatrix" );
		geometryWorldMatrices = geometryPass.getLocation( "worldMatrices" );

		geometryDiffuseMap = geometryPass.getLocation( "diffuseMap" );
		geometryNormalMap = geometryPass.getLocation( "normalMap" );
		geometryPositionMap = geometryPass.getLocation( "positionMap" );
		geometryDepthMap = geometryPass.getLocation( "depthMap" );

		glUseProgram( 0 );
	}
	else
	{
		LOG( VERBOSITY_ERROR, "Failed to load geometry pass shader." );
		result = false;
	}

	// DIRECTIONAL PASS
	if( directionalLightPass.load( "./assets/shaders/directional_light_pass.vs",
									NULL,
									"./assets/shaders/directional_light_pass.fs" ) )
	{
		directionalLightPass.bind();

		directionalLightDirection = directionalLightPass.getLocation( "directionalLight.direction" );
		directionalLightColor = directionalLightPass.getLocation( "directionalLight.color" );
		directionalLightIntensity= directionalLightPass.getLocation( "directionalLight.intensity" );
		directionalLightCameraPosition = directionalLightPass.getLocation( "cameraPosition" );
		directionalLightSpecularPower = directionalLightPass.getLocation( "specularPower" );
		directionalLightTransformation = directionalLightPass.getLocation( "lightTransformation" );

		directionalLightDiffuseTarget = directionalLightPass.getLocation( "diffuseTarget" );
		directionalLightPositionTarget = directionalLightPass.getLocation( "positionTarget" );
		directionalLightNormalTarget = directionalLightPass.getLocation( "normalTarget" );
		directionalLightDepthTarget = directionalLightPass.getLocation( "depthTarget" );
		directionalLightShadowTarget = directionalLightPass.getLocation( "shadowTarget" );

		glUseProgram( 0 );
	}
	else
	{
		LOG( VERBOSITY_ERROR, "Failed to load directional light pass shader." );
		result = false;
	}

	// DIRECTIONAL SHADOW PASS
	if( directionalShadowPass.load( "./assets/shaders/directional_shadow_pass.vs",
										NULL,
										"./assets/shaders/directional_shadow_pass.fs" ) )
	{
		directionalShadowPass.bind();

		directionalShadowProjectionMatrix = directionalShadowPass.getLocation( "projectionMatrix" );
		directionalShadowViewMatrix = directionalShadowPass.getLocation( "viewMatrix" );
		directionalShadowWorldMatrices = directionalShadowPass.getLocation( "worldMatrices" );
		directionalShadowWorldMatrix = directionalShadowPass.getLocation( "worldMatrix" );

		glUseProgram( 0 );
	}
	else
	{
		LOG( VERBOSITY_ERROR, "Failed to load directional shadow pass shader." );
		result = false;
	}

	// POINT LIGHT PASS
	if( pointLightPass.load( "./assets/shaders/point_light_pass.vs",
								NULL,
								"./assets/shaders/point_light_pass.fs" ) )
	{
		pointLightPass.bind();

		pointLightProjectionMatrix = pointLightPass.getLocation( "projectionMatrix" );
		pointLightViewMatrix = pointLightPass.getLocation( "viewMatrix" );
		pointLightWorldMatrix = pointLightPass.getLocation( "worldMatrix" );

		pointLightCameraPosition = pointLightPass.getLocation( "cameraPosition" );
		pointLightScreenSize = pointLightPass.getLocation( "screenSize" );
		pointLightSpecularPower = pointLightPass.getLocation( "specularPower" );

		pointLightPosition = pointLightPass.getLocation( "pointLight.position" );
		pointLightRadius = pointLightPass.getLocation( "pointLight.radius" );
		pointLightColor = pointLightPass.getLocation( "pointLight.color" );
		pointLightIntensity = pointLightPass.getLocation( "pointLight.intensity" );
		pointLightLinear = pointLightPass.getLocation( "pointLight.linear" );
		pointLightConstant = pointLightPass.getLocation( "pointLight.constant" );
		pointLightExponent = pointLightPass.getLocation( "pointLight.exponent" );

		pointLightDiffuseTarget = pointLightPass.getLocation( "diffuseTarget" );
		pointLightPositionTarget = pointLightPass.getLocation( "positionTarget" );
		pointLightNormalTarget = pointLightPass.getLocation( "normalTarget" );

		glUseProgram( 0 );
	}
	else
	{
		LOG( VERBOSITY_ERROR, "Failed to load point light pass shader." );
		result = false;
	}

	sphereMesh = assets->loadMesh( GBUFFER_SPHERE_MESH_PATH );
	if( sphereMesh < 0 )
	{
		LOG( VERBOSITY_ERROR, "Failed to load sphere mesh for point light pass." );
		result = false;
	}

	// BILLBOARD PASS
	if( billboardPass.load( "./assets/shaders/billboard.vs",
								"./assets/shaders/billboard.gs",
								"./assets/shaders/billboard.fs" ) )
	{
		billboardPass.bind();

		LOG_INFO( "Retrieving uniform locations from billboard shader." );
		billboardProjectionMatrix = billboardPass.getLocation( "projectionMatrix" );
		billboardViewMatrix = billboardPass.getLocation( "viewMatrix" );
		billboardScreenSize = billboardPass.getLocation( "screenSize" );
		billboardDeltaTime = billboardPass.getLocation( "deltaTime" );

		billboardDiffuseMap = billboardPass.getLocation( "diffuseMap" );
		billboardNormalMap = billboardPass.getLocation( "normalMap" );
		billboardSpecularMap = billboardPass.getLocation( "specularMap" );
		billboardMaskMap = billboardPass.getLocation( "maskMap" );
		billboardDepthTarget = billboardPass.getLocation( "depthTarget" );

		LOG_INFO( "Generating vertex data for billboard shader." );
		glGenVertexArrays( 1, &billboardVAO );
		glBindVertexArray( billboardVAO );

		glEnableVertexAttribArray( 0 );
		glEnableVertexAttribArray( 1 );
		glEnableVertexAttribArray( 2 );
		glEnableVertexAttribArray( 3 );
		glEnableVertexAttribArray( 4 );

		glGenBuffers( 1, &billboardVBO );
		glBindBuffer( GL_ARRAY_BUFFER, billboardVBO );
		glBufferData( GL_ARRAY_BUFFER, sizeof(Billboard)*GRAPHICS_MAX_BILLBOARDS, nullptr, GL_STREAM_DRAW );

		glVertexAttribPointer( 0, 3, GL_FLOAT, GL_FALSE, sizeof(Billboard), 0 );
		glVertexAttribPointer( 1, 4, GL_FLOAT, GL_FALSE, sizeof(Billboard), (void*)(sizeof(GLfloat)*3) );
		glVertexAttribPointer( 2, 2, GL_FLOAT, GL_FALSE, sizeof(Billboard), (void*)(sizeof(GLfloat)*7 ) );
		glVertexAttribPointer( 3, 1, GL_FLOAT, GL_FALSE, sizeof(Billboard), (void*)( sizeof(GLfloat)*9) );
		glVertexAttribPointer( 4, 3, GL_FLOAT, GL_FALSE, sizeof(Billboard), (void*)( sizeof(GLfloat)*10) );

		glBindVertexArray( 0 );
	}
	else
	{
		LOG( VERBOSITY_ERROR, "Failed to load billboard pass shader." );
		result = false;
	}

	// FINAL PASS
	if( finalPass.load( "./assets/shaders/final_pass.vs",
							NULL,
							"./assets/shaders/final_pass.fs" ) )
	{
		finalPass.bind();

		finalLightTarget = finalPass.getLocation( "lightTarget" );
		finalBillboardTarget = finalPass.getLocation( "billboardTarget" );
		finalBillboardAlphaTarget = finalPass.getLocation( "billboardAlphaTarget" );

		glUseProgram( 0 );
	}
	else
	{
		LOG( VERBOSITY_ERROR, "Failed to load final pass shader." );
		result = false;
	}

	// FBO
	LOG( VERBOSITY_INFORMATION, "Generating FBO." );

	glGenFramebuffers( 1, &fbo );
	glBindFramebuffer( GL_FRAMEBUFFER, fbo );

	glGenTextures( MAX_TARGETS, targets );
	glGenTextures( 1, &depthBuffer );

	// generate color targets
	for( int i=0; i<MAX_TARGETS; i++ )
	{
		glBindTexture( GL_TEXTURE_2D, targets[i] );
		glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR );
		glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR );
		glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_BASE_LEVEL, 0 );
		glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAX_LEVEL, 0 );

		if( i == TARGET_DEPTH || i == TARGET_SHADOW )
		{
			glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE );
			glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE );
		}

		glTexImage2D( GL_TEXTURE_2D, 0, GL_RGB32F, width, height, 0, GL_RGB, GL_FLOAT, NULL );
		glFramebufferTexture2D( GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0+i, GL_TEXTURE_2D, targets[i], 0 );
	}

	// generate depth target
	glBindTexture( GL_TEXTURE_2D, depthBuffer );
	glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR );
	glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR );
	glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE );
	glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE );
	glTexImage2D( GL_TEXTURE_2D, 0, GL_DEPTH24_STENCIL8, width, height, 0, GL_DEPTH_COMPONENT, GL_FLOAT, NULL );
	glFramebufferTexture2D( GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_TEXTURE_2D, depthBuffer, 0 );

	GLenum status = glCheckFramebufferStatus( GL_FRAMEBUFFER );
	if( status != GL_FRAMEBUFFER_COMPLETE )
	{
		LOG( VERBOSITY_ERROR, "Failed to create framebuffer.\nStatus: %d", status );
	}

	glClearColor( 0.1f, 0.1f, 0.1f, 1.0f );
	glEnable( GL_DEPTH_TEST );
	glEnable( GL_CULL_FACE );

	glBindFramebuffer( GL_FRAMEBUFFER, 0 );

	// generate quad
	glGenVertexArrays( 1, &quadVAO );
	glBindVertexArray( quadVAO );

	glEnableVertexAttribArray( 0 );
	glEnableVertexAttribArray( 1 );

	const GLfloat quadVertices[] =
	{
		// position		uv
		-1.0f, 1.0f,	0.0f, 0.0f,
		-1.0f, -1.0f,	0.0f, 1.0f,
		1.0f, 1.0f,		1.0f, 0.0f,
		1.0f, -1.0f,	1.0f, 1.0f
	};
	const int STRIDE = sizeof(GLfloat)*4;

	GLuint quadVBO;
	glGenBuffers( 1, &quadVBO );
	glBindBuffer( GL_ARRAY_BUFFER, quadVBO );
	glBufferData( GL_ARRAY_BUFFER, STRIDE*4, quadVertices, GL_STATIC_DRAW );

	glVertexAttribPointer( 0, 2, GL_FLOAT, GL_FALSE, STRIDE, 0 );
	glVertexAttribPointer( 1, 2, GL_FLOAT, GL_FALSE, STRIDE, (void*)(sizeof(GLfloat)*2) );

	glBindVertexArray( 0 );

	return result;
}

void Gbuffer::begin( float deltaTime )
{
	elapsedTime += deltaTime;

	glBindFramebuffer( GL_FRAMEBUFFER, fbo );

	// clear geometry targets
	GLenum geometryTargets[] =
	{
		GL_COLOR_ATTACHMENT0 + TARGET_DIFFUSE,
		GL_COLOR_ATTACHMENT0 + TARGET_POSITION,
		GL_COLOR_ATTACHMENT0 + TARGET_NORMAL
	};
	glDrawBuffers( 3, geometryTargets );
	glClearColor( 0.0f, 0.0f, 0.0f, 0.0f );
	glClear( GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT );

	// clear final targets
	GLenum finalTargets[] =
	{
		GL_COLOR_ATTACHMENT0 + TARGET_LIGHT,
		GL_COLOR_ATTACHMENT0 + TARGET_BILLBOARD,
		GL_COLOR_ATTACHMENT0 + TARGET_FINAL
	};
	glDrawBuffers( 3, finalTargets );
	glClear( GL_COLOR_BUFFER_BIT );

	// clear depth target to white
	glDrawBuffer( GL_COLOR_ATTACHMENT0 + TARGET_DEPTH );
	glClearColor( 1.0f, 1.0f, 1.0f, 0.0f );
	glClear( GL_COLOR_BUFFER_BIT );
}

void Gbuffer::end()
{
	glBindFramebuffer( GL_DRAW_FRAMEBUFFER, 0 );
	glBindFramebuffer( GL_READ_FRAMEBUFFER, fbo );

	switch( debugMode )
	{
		case DEBUG_GEOMETRY:
		{
			// diffuse target, top left
			glReadBuffer( GL_COLOR_ATTACHMENT0 + TARGET_DIFFUSE );
			glBlitFramebuffer( 0, 0, width, height, 0, height/2, width/2, height, GL_COLOR_BUFFER_BIT, GL_LINEAR );

			// normal target, top right
			glReadBuffer( GL_COLOR_ATTACHMENT0 + TARGET_NORMAL );
			glBlitFramebuffer( 0, 0, width, height, width/2, height/2, width, height, GL_COLOR_BUFFER_BIT, GL_LINEAR );

			// position target, bottom left
			glReadBuffer( GL_COLOR_ATTACHMENT0 + TARGET_POSITION );
			glBlitFramebuffer( 0, 0, width, height, 0, 0, width/2, height/2, GL_COLOR_BUFFER_BIT, GL_LINEAR );

			// depth target, bottom right
			glReadBuffer( GL_COLOR_ATTACHMENT0 + TARGET_DEPTH );
			glBlitFramebuffer( 0, 0, width, height, width/2, 0, width, height/2, GL_COLOR_BUFFER_BIT, GL_LINEAR );
		} break;

		case DEBUG_FINAL:
		{
			// final light
			glReadBuffer( GL_COLOR_ATTACHMENT0+TARGET_LIGHT );
			glBlitFramebuffer( 0, 0, width, height, 0, height/2, width/2, height, GL_COLOR_BUFFER_BIT, GL_LINEAR );

			// final billboard
			glReadBuffer( GL_COLOR_ATTACHMENT0+TARGET_BILLBOARD );
			glBlitFramebuffer( 0, 0, width, height, width/2, height/2, width, height, GL_COLOR_BUFFER_BIT, GL_LINEAR );

			// final
			glReadBuffer( GL_COLOR_ATTACHMENT0+TARGET_FINAL );
			glBlitFramebuffer( 0, 0, width, height, 0, 0, width/2, height/2, GL_COLOR_BUFFER_BIT, GL_LINEAR );

			// alpha
			glReadBuffer( GL_COLOR_ATTACHMENT0+TARGET_ALPHA );
			glBlitFramebuffer( 0, 0, width, height, width/2, 0, width, height/2, GL_COLOR_BUFFER_BIT, GL_LINEAR );
		} break;

		case DEBUG_NONE:
		default:
		{
			glReadBuffer( GL_COLOR_ATTACHMENT0+TARGET_FINAL );
			glBlitFramebuffer( 0, 0, width, height, 0, 0, width, height, GL_COLOR_BUFFER_BIT, GL_LINEAR );
			glBlitFramebuffer( 0, 0, width, height, 0, 0, width, height, GL_DEPTH_BUFFER_BIT, GL_NEAREST );
		} break;
	}

	glBindFramebuffer( GL_FRAMEBUFFER, 0 );
}

void Gbuffer::beginGeometryPass( Camera* camera )
{
	GLenum drawBuffers[] =
	{
		GL_COLOR_ATTACHMENT0 + TARGET_DIFFUSE,
		GL_COLOR_ATTACHMENT0 + TARGET_POSITION,
		GL_COLOR_ATTACHMENT0 + TARGET_NORMAL,
		GL_COLOR_ATTACHMENT0 + TARGET_DEPTH,
	};
	glDrawBuffers( TARGET_DEPTH+1, drawBuffers );

	geometryPass.bind();
	geometryPass.setMat4( geometryProjectionMatrix, camera->getProjectionMatrix() );
	geometryPass.setMat4( geometryViewMatrix, camera->getViewMatrix() );

	geometryPass.setInt( geometryDiffuseMap, 0 );
	geometryPass.setInt( geometryNormalMap, 1 );
}

void Gbuffer::beginGeometry( glm::mat4& proj, glm::mat4& view )
{
	GLenum drawBuffers[] =
	{
		GL_COLOR_ATTACHMENT0 + TARGET_DIFFUSE,
		GL_COLOR_ATTACHMENT0 + TARGET_POSITION,
		GL_COLOR_ATTACHMENT0 + TARGET_NORMAL,
		GL_COLOR_ATTACHMENT0 + TARGET_DEPTH,
	};
	glDrawBuffers( TARGET_DEPTH+1, drawBuffers );

	geometryPass.bind();
	geometryPass.setMat4( geometryProjectionMatrix, proj );
	geometryPass.setMat4( geometryViewMatrix, view );

	geometryPass.setInt( geometryDiffuseMap, 0 );
	geometryPass.setInt( geometryNormalMap, 1 );
}

void Gbuffer::endGeometryPass()
{
}

void Gbuffer::updateGeometryWorldMatrices( const glm::mat4* worldMatrices, int count )
{
	geometryPass.setMat4( geometryWorldMatrices, worldMatrices, count );
}

void Gbuffer::updateGeometryTextures( const Texture* diffuseMap, const Texture* normalMap, const Texture* specularMap )
{
	diffuseMap->bind( GL_TEXTURE0 );
	normalMap->bind( GL_TEXTURE1 );
	specularMap->bind( GL_TEXTURE2 );
}

void Gbuffer::beginDirectionalLightPass( int target, Camera* camera )
{
	glDisable( GL_CULL_FACE );
	glDisable( GL_DEPTH_TEST );
	glDrawBuffer( GL_COLOR_ATTACHMENT0 + target );
	glDepthMask( GL_FALSE );

	glEnable( GL_BLEND );
	glBlendEquation( GL_FUNC_ADD );
	glBlendFunc( GL_ONE, GL_ONE );

	directionalLightPass.bind();
	directionalLightPass.setVec3( directionalLightCameraPosition, camera->getFinalPosition() );
	// TEMP: Magic number
	directionalLightPass.setFloat( directionalLightSpecularPower, 8.0f );

	glActiveTexture( GL_TEXTURE0 );
	glBindTexture( GL_TEXTURE_2D, targets[TARGET_DIFFUSE] );
	glActiveTexture( GL_TEXTURE1 );
	glBindTexture( GL_TEXTURE_2D, targets[TARGET_POSITION] );
	glActiveTexture( GL_TEXTURE2 );
	glBindTexture( GL_TEXTURE_2D, targets[TARGET_NORMAL] );
	glActiveTexture( GL_TEXTURE3 );
	glBindTexture( GL_TEXTURE_2D, targets[TARGET_DEPTH] );
	glActiveTexture( GL_TEXTURE4 );
	glBindTexture( GL_TEXTURE_2D, targets[TARGET_SHADOW] );

	directionalLightPass.setInt( directionalLightDiffuseTarget, 0 );
	directionalLightPass.setInt( directionalLightPositionTarget, 1 );
	directionalLightPass.setInt( directionalLightNormalTarget, 2 );
	directionalLightPass.setInt( directionalLightDepthTarget, 3 );
	directionalLightPass.setInt( directionalLightShadowTarget, 4 );
}

void Gbuffer::endDirectionalLightPass()
{
	glDisable( GL_BLEND );
	glBlendEquation( GL_FUNC_ADD );
	glBlendFunc( GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA );

	glEnable( GL_DEPTH_TEST );
	glEnable( GL_CULL_FACE );
	glDepthMask( GL_TRUE );
}

void Gbuffer::renderDirectionalLight( Camera* camera, const DirectionalLight& light )
{
	directionalLightPass.setVec3( directionalLightDirection, light.direction );
	directionalLightPass.setVec3( directionalLightColor, light.color );
	directionalLightPass.setFloat( directionalLightIntensity, light.intensity );

	// TEMP: Lots of magic numbers
	float halfResolution = GBUFFER_SHADOW_MAP_RESOLUTION * 0.5f;
	glm::mat4 projectionMatrix = glm::ortho( -halfResolution, halfResolution, -halfResolution, halfResolution, 0.01f, 100.0f );
	glm::mat4 viewMatrix = glm::lookAt( -light.direction*10.0f, glm::vec3( 0.0f ), glm::vec3( 0.0f, 1.0f, 0.0f ) );
	glm::mat4 lightTransform = projectionMatrix * viewMatrix;
	directionalLightPass.setMat4( directionalLightTransformation, lightTransform );

	glBindVertexArray( quadVAO );
	glDrawArrays( GL_TRIANGLE_STRIP, 0, 4 );
	glBindVertexArray( 0 );
}

void Gbuffer::beginDirectionalShadowPass( Camera* camera, const DirectionalLight& light )
{
	glDrawBuffer( GL_COLOR_ATTACHMENT0 + TARGET_SHADOW );
	glClearColor( 1.0f, 1.0f, 1.0f, 0.0f );
	glClear( GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT );
	
	directionalShadowPass.bind();

	// TEMP: Lots of magic numbers
	float halfResolution = GBUFFER_SHADOW_MAP_RESOLUTION * 0.5f;
	glm::mat4 projectionMatrix = glm::ortho( -halfResolution, halfResolution, -halfResolution, halfResolution, 0.01f, 100.0f );
	
	directionalShadowPass.setMat4( directionalShadowProjectionMatrix, projectionMatrix );
}

void Gbuffer::endDirectionalShadowPass()
{
}

void Gbuffer::updateDirectionalShadowWorldMatrices( const glm::mat4* worldMatrices, int count )
{
	directionalShadowPass.setMat4( directionalShadowWorldMatrices, worldMatrices, count );
}

void Gbuffer::updateDirectionalShadowWorldMatrix( const glm::mat4& worldMatrix, const glm::vec3& lightDirection )
{
	directionalShadowPass.setMat4( directionalShadowWorldMatrix, worldMatrix );

	glm::vec3 position = glm::vec3( worldMatrix[3] );
	glm::mat4 viewMatrix = glm::lookAt( position - lightDirection*10.0f, glm::vec3( 0.0f ), glm::vec3( 0.0f, 1.0f, 0.0f ) );
	directionalShadowPass.setMat4( directionalShadowViewMatrix, viewMatrix );
}

void Gbuffer::clearShadowTarget()
{
	glDrawBuffer( GL_COLOR_ATTACHMENT0+TARGET_SHADOW );
	glClearColor( 1.0f, 1.0f, 1.0f, 0.0f );
	glClear( GL_COLOR_BUFFER_BIT );
}

void Gbuffer::beginPointLightPass( int target, Camera* camera )
{
	glDrawBuffer( GL_COLOR_ATTACHMENT0+target );
	glDisable( GL_DEPTH_TEST );
	glCullFace( GL_FRONT );

	glDepthMask( GL_FALSE );
	glEnable( GL_BLEND );
	glBlendEquation( GL_FUNC_ADD );
	glBlendFunc( GL_ONE, GL_ONE );

	pointLightPass.bind();

	pointLightPass.setMat4( pointLightProjectionMatrix, camera->getProjectionMatrix() );
	pointLightPass.setMat4( pointLightViewMatrix, camera->getViewMatrix() );
	pointLightPass.setVec3( pointLightCameraPosition, camera->getFinalPosition() );
	pointLightPass.setVec2( pointLightScreenSize, glm::vec2( WINDOW_WIDTH, WINDOW_HEIGHT ) );
	// TEMP: Magic numbers
	pointLightPass.setFloat( pointLightSpecularPower, 8.0f );

	glActiveTexture( GL_TEXTURE0 );
	glBindTexture( GL_TEXTURE_2D, targets[TARGET_DIFFUSE] );
	glActiveTexture( GL_TEXTURE1 );
	glBindTexture( GL_TEXTURE_2D, targets[TARGET_POSITION] );
	glActiveTexture( GL_TEXTURE2 );
	glBindTexture( GL_TEXTURE_2D, targets[TARGET_NORMAL] );

	pointLightPass.setInt( pointLightDiffuseTarget, 0 );
	pointLightPass.setInt( pointLightPositionTarget, 1 );
	pointLightPass.setInt( pointLightNormalTarget, 2 );
}

void Gbuffer::endPointLightPass()
{
	glDepthMask( GL_TRUE );
	glEnable( GL_DEPTH_TEST );
	glDisable( GL_BLEND );
	glCullFace( GL_BACK );
}

void Gbuffer::renderPointLight( const PointLight& light )
{
	pointLightPass.setVec3( pointLightPosition, light.position );
	pointLightPass.setVec3( pointLightColor, light.color );
	pointLightPass.setFloat( pointLightIntensity, light.intensity );

	pointLightPass.setFloat( pointLightLinear, light.linear );
	pointLightPass.setFloat( pointLightConstant, light.constant );
	pointLightPass.setFloat( pointLightExponent, light.exponent );

	// Distance-from-attenuation formula:
	// http://ogldev.atspace.co.uk/www/tutorial36/tutorial36.html
	float C = fmax( fmax( light.color.r, light.color.g ), light.color.b );
	float radius = ( -light.linear + sqrt( powf( light.linear, 2.0f ) - 4*light.exponent * ( light.constant - 256*C*light.intensity ) ) ) / (2*light.exponent);

	glm::mat4 worldMatrix = glm::scale( glm::translate( glm::mat4(), light.position ), glm::vec3( radius ) );
	pointLightPass.setMat4( pointLightWorldMatrix, worldMatrix );

	//Mesh* sphere = assets->getMesh( sphereMesh );
	const Mesh* sphere = assets->getMesh( sphereMesh );
	if( sphere->getUploaded() )
	{
		sphere->bind();
		sphere->render();
	}
}

void Gbuffer::beginBillboardPass( Camera* camera )
{
	glDrawBuffer( GL_COLOR_ATTACHMENT0 + TARGET_ALPHA );
	glClearColor( 0.0f, 0.0f, 0.0f, 0.0f );
	glClear( GL_COLOR_BUFFER_BIT );

	const int BUFFER_COUNT = 2;
	GLenum drawBuffers[BUFFER_COUNT] =
	{
		GL_COLOR_ATTACHMENT0 + TARGET_BILLBOARD,
		GL_COLOR_ATTACHMENT0 + TARGET_ALPHA
	};
	glDrawBuffers( BUFFER_COUNT, drawBuffers );

	glClear( GL_DEPTH_BUFFER_BIT );
	glEnable( GL_BLEND );
	glBlendFunc( GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA );
	glDepthMask( GL_FALSE );

	billboardPass.bind();
	billboardPass.setMat4( billboardProjectionMatrix, camera->getProjectionMatrix() );
	billboardPass.setMat4( billboardViewMatrix, camera->getViewMatrix() );
	billboardPass.setVec2( billboardScreenSize, glm::vec2( WINDOW_WIDTH, WINDOW_HEIGHT ) );
	billboardPass.setFloat( billboardDeltaTime, elapsedTime );

	billboardPass.setInt( billboardDiffuseMap, 0 );
	billboardPass.setInt( billboardNormalMap, 1 );
	billboardPass.setInt( billboardSpecularMap, 2 );
	billboardPass.setInt( billboardMaskMap, 3 );

	glActiveTexture( GL_TEXTURE4 );
	glBindTexture( GL_TEXTURE_2D, targets[TARGET_DEPTH] );
	billboardPass.setInt( billboardDepthTarget, 4 );

	glBindVertexArray( billboardVAO );
	glBindBuffer( GL_ARRAY_BUFFER, billboardVBO );
}

void Gbuffer::endBillboardPass()
{
	glBindVertexArray( 0 );

	glEnable( GL_DEPTH_TEST );
	glDisable( GL_BLEND );
	glDepthMask( GL_TRUE );
}

void Gbuffer::renderBillboards( Array<Billboard>& billboards )
{
	glBindVertexArray( billboardVAO );
	glBindBuffer( GL_ARRAY_BUFFER, billboardVBO );

	const int COUNT = billboards.getSize();
	const int SIZE = sizeof(Billboard) * COUNT;
	glBufferData( GL_ARRAY_BUFFER, SIZE, billboards.getConstData(), GL_DYNAMIC_DRAW );

	glDrawArrays( GL_POINTS, 0, COUNT );
	glBindVertexArray( 0 );
}

void Gbuffer::performFinalPass()
{
	glDisable( GL_DEPTH_TEST );
	glDrawBuffer( GL_COLOR_ATTACHMENT0+TARGET_FINAL );

	finalPass.bind();

	glActiveTexture( GL_TEXTURE0 );
	glBindTexture( GL_TEXTURE_2D, targets[TARGET_LIGHT] );
	glActiveTexture( GL_TEXTURE1 );
	glBindTexture( GL_TEXTURE_2D, targets[TARGET_BILLBOARD] );
	glActiveTexture( GL_TEXTURE2 );
	glBindTexture( GL_TEXTURE_2D, targets[TARGET_ALPHA] );

	finalPass.setInt( finalLightTarget, 0 );
	finalPass.setInt( finalBillboardTarget, 1 );
	finalPass.setInt( finalBillboardAlphaTarget, 2 );

	glBindVertexArray( quadVAO );
	glDrawArrays( GL_TRIANGLE_STRIP, 0, 4 );
	glBindVertexArray( 0 );

	glEnable( GL_DEPTH_TEST );
}

void Gbuffer::setDebugMode( int mode )
{
	debugMode = mode;
}

void Gbuffer::toggleDebugMode()
{
	debugMode = ( debugMode + 1 ) % MAX_DEBUG_MODES;
}

GLuint Gbuffer::getFBO() const
{
	return fbo;
}

GLuint Gbuffer::getTarget( int index ) const
{
	assert( index >= 0 && index < MAX_TARGETS );
	return targets[index];
}