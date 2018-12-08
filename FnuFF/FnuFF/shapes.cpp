#include "shapes.h"
using namespace Rendering;

DebugShapes::DebugShapes()
	: ignoreDepth( false ), visible( true )
{
	LOG_INFO( "Constructing." );
}

DebugShapes::~DebugShapes()
{
	LOG_INFO( "Destructing." );
}

bool DebugShapes::load()
{
	LOG_INFO( "Loading shaders." );

	bool result = true;

	if( !lineShader.load( "./assets/shaders/debug_line.vs",
							"./assets/shaders/debug_line.gs",
							"./assets/shaders/debug_shape.fs" ) )
	{
		LOG_ERROR( "Failed to load line shader." );
		result = false;
	}

	if( !sphereShader.load( "./assets/shaders/debug_sphere.vs",
		"./assets/shaders/debug_sphere.gs",
		"./assets/shaders/debug_shape.fs" ) )
	{
		LOG_ERROR( "Failed to load sphere shader." );
		result = false;
	}

	if( !aabbShader.load( "./assets/shaders/debug_aabb.vs",
		"./assets/shaders/debug_aabb.gs",
		"./assets/shaders/debug_shape.fs" ) )
	{
		LOG_ERROR( "Failed to load aabb shader." );
		result = false;
	}

	if( !obbShader.load( "./assets/shaders/debug_obb.vs",
		"./assets/shaders/debug_obb.gs",
		"./assets/shaders/debug_shape.fs" ) )
	{
		LOG_ERROR( "Failed to load obb shader." );
		result = false;
	}

	return result;
}

void DebugShapes::unload()
{
	if( lineVAO )
		glDeleteVertexArrays( 1, &lineVAO );
	if( lineVBO )
		glDeleteBuffers( 1, &lineVBO );

	if( sphereVAO )
		glDeleteVertexArrays( 1, &sphereVAO );
	if( sphereVBO )
		glDeleteBuffers( 1, &sphereVBO );

	if( aabbVAO )
		glDeleteVertexArrays( 1, &aabbVAO );
	if( aabbVBO )
		glDeleteBuffers( 1, &aabbVBO );

	if( obbVAO )
		glDeleteVertexArrays( 1, &obbVAO );
	if( obbVBO )
		glDeleteBuffers( 1, &obbVBO );
}

void DebugShapes::upload()
{
	LOG_INFO( "Uploading shaders." );

	if( lineShader.getValid() )
	{
		lineProjectionMatrixLocation = lineShader.getLocation( "projectionMatrix" );
		lineViewMatrixLocation = lineShader.getLocation( "viewMatrix" );

		glGenVertexArrays( 1, &lineVAO );
		glBindVertexArray( lineVAO );

		glEnableVertexAttribArray( 0 );
		glEnableVertexAttribArray( 1 );
		glEnableVertexAttribArray( 2 );

		glGenBuffers( 1, &lineVBO );
		glBindBuffer( GL_ARRAY_BUFFER, lineVBO );
		glBufferData( GL_ARRAY_BUFFER, sizeof(DebugLine)*SHAPES_MAX_LINES, nullptr, GL_STREAM_DRAW );

		const int STRIDE = sizeof(DebugLine);
		glVertexAttribPointer( 0, 3, GL_FLOAT, GL_FALSE, STRIDE, 0 );
		glVertexAttribPointer( 1, 3, GL_FLOAT, GL_FALSE, STRIDE, (void*)(sizeof(glm::vec3) ) );
		glVertexAttribPointer( 2, 4, GL_FLOAT, GL_FALSE, STRIDE, (void*)(sizeof(glm::vec3)*2) );

		glBindVertexArray( 0 );
	}

	if( sphereShader.getValid() )
	{
		sphereProjectionMatrixLocation = sphereShader.getLocation( "projectionMatrix" );
		sphereViewMatrixLocation = sphereShader.getLocation( "viewMatrix" );

		glGenVertexArrays( 1, &sphereVAO );
		glBindVertexArray( sphereVAO );

		glEnableVertexAttribArray( 0 );
		glEnableVertexAttribArray( 1 );
		glEnableVertexAttribArray( 2 );

		glGenBuffers( 1, &sphereVBO );
		glBindBuffer( GL_ARRAY_BUFFER, sphereVBO );
		glBufferData( GL_ARRAY_BUFFER, sizeof(DebugSphere)*SHAPES_MAX_SPHERES, nullptr, GL_STREAM_DRAW );

		const int STRIDE = sizeof(DebugSphere);
		glVertexAttribPointer( 0, 3, GL_FLOAT, GL_FALSE, STRIDE, 0 );
		glVertexAttribPointer( 1, 1, GL_FLOAT, GL_FALSE, STRIDE, (void*)(sizeof(glm::vec3) ) );
		glVertexAttribPointer( 2, 4, GL_FLOAT, GL_FALSE, STRIDE, (void*)(sizeof(glm::vec3)+ sizeof(float) ) );

		glBindVertexArray( 0 );
	}

	if( aabbShader.getValid() )
	{
		aabbProjectionMatrixLocation = aabbShader.getLocation( "projectionMatrix" );
		aabbViewMatrixLocation = aabbShader.getLocation( "viewMatrix" );

		glGenVertexArrays( 1, &aabbVAO );
		glBindVertexArray( aabbVAO );

		glEnableVertexAttribArray( 0 );
		glEnableVertexAttribArray( 1 );
		glEnableVertexAttribArray( 2 );

		glGenBuffers( 1, &aabbVBO );
		glBindBuffer( GL_ARRAY_BUFFER, aabbVBO );
		glBufferData( GL_ARRAY_BUFFER, sizeof(DebugAABB)*SHAPES_MAX_AABB, nullptr, GL_STREAM_DRAW );

		const int STRIDE = sizeof(DebugAABB);
		glVertexAttribPointer( 0, 3, GL_FLOAT, GL_FALSE, STRIDE, 0 );
		glVertexAttribPointer( 1, 3, GL_FLOAT, GL_FALSE, STRIDE, (void*)(sizeof(glm::vec3) ) );
		glVertexAttribPointer( 2, 4, GL_FLOAT, GL_FALSE, STRIDE, (void*)(sizeof(glm::vec3)* 2 ) );

		glBindVertexArray( 0 );
	}

	if( obbShader.getValid() )
	{
		obbProjectionMatrixLocation = obbShader.getLocation( "projectionMatrix" );
		obbViewMatrixLocation = obbShader.getLocation( "viewMatrix" );

		glGenVertexArrays( 1, &obbVAO );
		glBindVertexArray( obbVAO );

		glEnableVertexAttribArray( 0 );
		glEnableVertexAttribArray( 1 );
		glEnableVertexAttribArray( 2 );
		glEnableVertexAttribArray( 3 );
		glEnableVertexAttribArray( 4 );
		glEnableVertexAttribArray( 5 );

		glGenBuffers( 1, &obbVBO );
		glBindBuffer( GL_ARRAY_BUFFER, obbVBO );
		glBufferData( GL_ARRAY_BUFFER, sizeof(DebugOBB)*SHAPES_MAX_OBB, nullptr, GL_STREAM_DRAW );

		const int STRIDE = sizeof(DebugOBB);
		glVertexAttribPointer( 0, 3, GL_FLOAT, GL_FALSE, STRIDE, 0 );
		glVertexAttribPointer( 1, 3, GL_FLOAT, GL_FALSE, STRIDE, (void*)(sizeof(glm::vec3) ) );
		glVertexAttribPointer( 2, 3, GL_FLOAT, GL_FALSE, STRIDE, (void*)(sizeof(glm::vec3)* 2 ) );
		glVertexAttribPointer( 3, 3, GL_FLOAT, GL_FALSE, STRIDE, (void*)(sizeof(glm::vec3)* 3 ) );
		glVertexAttribPointer( 4, 3, GL_FLOAT, GL_FALSE, STRIDE, (void*)(sizeof(glm::vec3)* 4 ) );
		glVertexAttribPointer( 5, 4, GL_FLOAT, GL_FALSE, STRIDE, (void*)(sizeof(glm::vec3)* 5 ) );

		glBindVertexArray( 0 );
	}
}

void DebugShapes::renderShapes( const glm::mat4& projectionMatrix, const glm::mat4& viewMatrix, SwapArray<DebugLine>& lines, SwapArray<DebugSphere>& sphers, SwapArray<DebugAABB>& aabbs, SwapArray<DebugOBB>& obbs )
{
	if( visible )
	{
		glEnable( GL_BLEND );
		glBlendFunc( GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA );

		if( ignoreDepth )
			glDisable( GL_DEPTH_TEST );

		const int LINE_COUNT = lines.getRead().getSize();
		const int SPHERE_COUNT = spheres.getRead().getSize();
		const int AABB_COUNT = aabbs.getRead().getSize();
		const int OBB_COUNT = obbs.getRead().getSize();

		if( LINE_COUNT > 0 )
		{
			lineShader.bind();
			lineShader.setMat4( lineProjectionMatrixLocation, projectionMatrix );
			lineShader.setMat4( lineViewMatrixLocation, viewMatrix );

			glBindVertexArray( lineVAO );
			glBindBuffer( GL_ARRAY_BUFFER, lineVBO );

			int offset = 0;
			while( offset < LINE_COUNT )
			{
				int count = LINE_COUNT - offset;
				if( count > SHAPES_MAX_LINES )
					count = SHAPES_MAX_LINES;

				glBufferSubData( GL_ARRAY_BUFFER, 0, sizeof(DebugLine)*count, lines.getRead().getData() + offset );
				glDrawArrays( GL_POINTS, 0, count );

				offset += count;
			}
		}

		if( SPHERE_COUNT > 0 )
		{
			sphereShader.bind();
			sphereShader.setMat4( sphereProjectionMatrixLocation, projectionMatrix );
			sphereShader.setMat4( sphereViewMatrixLocation, viewMatrix );

			glBindVertexArray( sphereVAO );
			glBindBuffer( GL_ARRAY_BUFFER, sphereVBO );

			int offset = 0;
			while( offset < SPHERE_COUNT )
			{
				int count = SPHERE_COUNT - offset;
				if( count > SHAPES_MAX_SPHERES )
					count = SHAPES_MAX_SPHERES;

				glBufferSubData( GL_ARRAY_BUFFER, 0, sizeof(DebugSphere)*count, spheres.getRead().getData() + offset );
				glDrawArrays( GL_POINTS, 0, count );

				offset += count;
			}
		}

		if( AABB_COUNT > 0 )
		{
			aabbShader.bind();
			aabbShader.setMat4( aabbProjectionMatrixLocation, projectionMatrix );
			aabbShader.setMat4( aabbViewMatrixLocation, viewMatrix );

			glBindVertexArray( aabbVAO );
			glBindBuffer( GL_ARRAY_BUFFER, aabbVBO );

			int offset = 0;
			while( offset < AABB_COUNT )
			{
				int count = AABB_COUNT - offset;
				if( count > SHAPES_MAX_AABB )
					count = SHAPES_MAX_AABB;

				glBufferSubData( GL_ARRAY_BUFFER, 0, sizeof(DebugAABB)*count, aabbs.getRead().getData() + offset );
				glDrawArrays( GL_POINTS, 0, count );

				offset += count;
			}
		}

		if( OBB_COUNT > 0 )
		{
			obbShader.bind();
			obbShader.setMat4( obbProjectionMatrixLocation, projectionMatrix );
			obbShader.setMat4( obbViewMatrixLocation, viewMatrix );

			glBindVertexArray( obbVAO );
			glBindBuffer( GL_ARRAY_BUFFER, obbVBO );

			int offset = 0;
			while( offset < OBB_COUNT )
			{
				int count = OBB_COUNT - offset;
				if( count > SHAPES_MAX_OBB )
					count = SHAPES_MAX_OBB;

				glBufferSubData( GL_ARRAY_BUFFER, 0, sizeof(DebugOBB)*count, obbs.getRead().getData() + offset );
				glDrawArrays( GL_POINTS, 0, count );

				offset += count;
			}
		}

		// reset
		glBindVertexArray( 0 );

		if( ignoreDepth )
			glEnable( GL_DEPTH_TEST );
		glDisable( GL_BLEND );
	}
}

void DebugShapes::finalize()
{
	lines.swap();
	spheres.swap();
	aabbs.swap();
	obbs.swap();

	omniLines.swap();
	omniSpheres.swap();
	omniAABBs.swap();
	omniOBBs.swap();

	lines.getWrite().clear();
	spheres.getWrite().clear();
	aabbs.getWrite().clear();
	obbs.getWrite().clear();

	omniLines.getWrite().clear();
	omniSpheres.getWrite().clear();
	omniAABBs.getWrite().clear();
	omniOBBs.getWrite().clear();
}

void DebugShapes::addLine( const DebugLine& line, bool ignoreDepth )
{
	if( ignoreDepth )
		omniLines.getWrite().add( line );
	else
		lines.getWrite().add( line );
}

void DebugShapes::addSphere( const DebugSphere& sphere, bool ignoreDepth )
{
	if( ignoreDepth )
		omniSpheres.getWrite().add( sphere );
	else
		spheres.getWrite().add( sphere );
}

void DebugShapes::addAABB( const DebugAABB& aabb, bool ignoreDepth )
{
	if( ignoreDepth )
		omniAABBs.getWrite().add( aabb );
	else
		aabbs.getWrite().add( aabb );
}

void DebugShapes::addOBB( const DebugOBB& obb, bool ignoreDepth )
{
	if( ignoreDepth )
		omniOBBs.getWrite().add( obb );
	else
		obbs.getWrite().add( obb );
}

void DebugShapes::setIgnoreDepth( bool ignore )
{
	ignoreDepth = ignore;
}

void DebugShapes::setVisible( bool v )
{
	visible = v;
}

bool DebugShapes::getIgnoreDepth() const
{
	return ignoreDepth;
}

bool DebugShapes::getVisible() const
{
	return visible;
}
