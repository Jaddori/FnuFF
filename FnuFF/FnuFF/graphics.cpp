#include "graphics.h"
using namespace Rendering;

Graphics::Graphics()
	: writeIndex( 0 ), readIndex( 1 )
{
}

Graphics::~Graphics()
{
}

void Graphics::load()
{
	solidShader.load( "./assets/shaders/solid.vs", NULL, "./assets/shaders/solid.fs" );
	solidShaderProjectionLocation = solidShader.getLocation( "projectionMatrix" );
	solidShaderViewLocation = solidShader.getLocation( "viewMatrix" );

	perspectiveCamera.updatePerspective( WINDOW_WIDTH, WINDOW_HEIGHT );
	perspectiveCamera.setPosition( glm::vec3( 0, 0, -10 ) );

	assets.loadPack( "./assets/textures/pack01.bin" );
	
	textShader.load( "./assets/shaders/font.vs", "./assets/shaders/font.gs", "./assets/shaders/font.fs" );
	textProjectionLocation = textShader.getLocation( "projectionMatrix" );

	orthographicCamera.updateOrthographic( WINDOW_WIDTH, WINDOW_HEIGHT );

	glGenVertexArrays( 1, &textVAO );
	glBindVertexArray( textVAO );

	glEnableVertexAttribArray( 0 );
	glEnableVertexAttribArray( 1 );
	glEnableVertexAttribArray( 2 );
	glEnableVertexAttribArray( 3 );

	glGenBuffers( 1, &textVBO );
	glBindBuffer( GL_ARRAY_BUFFER, textVBO );
	glBufferData( GL_ARRAY_BUFFER, sizeof(Glyph)*GRAPHICS_MAX_GLYPHS, NULL, GL_DYNAMIC_DRAW );

	glVertexAttribPointer( 0, 3, GL_FLOAT, GL_FALSE, sizeof(Glyph), 0 );
	glVertexAttribPointer( 1, 4, GL_FLOAT, GL_FALSE, sizeof(Glyph), (void*)(sizeof(GLfloat)*3) );
	glVertexAttribPointer( 2, 2, GL_FLOAT, GL_FALSE, sizeof(Glyph), (void*)(sizeof(GLfloat)*7) );
	glVertexAttribPointer( 3, 4, GL_FLOAT, GL_FALSE, sizeof(Glyph), (void*)(sizeof(GLfloat)*9) );

	glBindVertexArray( 0 );

	// quads
	if( quadShader.load( "./assets/shaders/quad.vs",
							"./assets/shaders/quad.gs",
							"./assets/shaders/quad.fs" ) )
	{
		quadShader.bind();
		quadProjectionLocation = quadShader.getLocation( "projectionMatrix" );

		glGenVertexArrays( 1, &quadVAO );
		glBindVertexArray( quadVAO );

		glEnableVertexAttribArray( 0 ); // position
		glEnableVertexAttribArray( 1 ); // size
		glEnableVertexAttribArray( 2 ); // uv start + uv end
		glEnableVertexAttribArray( 3 ); // color

		glGenBuffers( 1, &quadVBO );
		glBindBuffer( GL_ARRAY_BUFFER, quadVBO );
		glBufferData( GL_ARRAY_BUFFER, sizeof(Quad)*GRAPHICS_MAX_QUADS, NULL, GL_DYNAMIC_DRAW );

		glVertexAttribPointer( 0, 3, GL_FLOAT, GL_FALSE, sizeof(Quad), 0 );
		glVertexAttribPointer( 1, 2, GL_FLOAT, GL_FALSE, sizeof(Quad), (void*)(sizeof(GLfloat)*3) );
		glVertexAttribPointer( 2, 4, GL_FLOAT, GL_FALSE, sizeof(Quad), (void*)(sizeof(GLfloat)*5) );
		glVertexAttribPointer( 3, 4, GL_FLOAT, GL_FALSE, sizeof(Quad), (void*)(sizeof(GLfloat)*9) );

		glBindVertexArray( 0 );
	}

	// billboards
	LOG_INFO( "Loading billboard shader." );
	if( billboardShader.load( "./assets/shaders/billboard.vs",
		"./assets/shaders/billboard.gs",
		"./assets/shaders/billboard.fs" ) )
	{
		LOG_INFO( "Retrieving uniform locations from billboard shader." );
		billboardShader.bind();
		billboardProjectionLocation = billboardShader.getLocation( "projectionMatrix" );
		billboardViewLocation = billboardShader.getLocation( "viewMatrix" );
		billboardDeltaTimeLocation = billboardShader.getLocation( "deltaTime" );
		billboardDiffuseLocation = billboardShader.getLocation( "diffuseMap" );
		billboardMaskLocation = billboardShader.getLocation( "maskMap" );

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
		LOG_ERROR( "Failed to load billboard shader." );
	}
}

void Graphics::finalize()
{
	perspectiveCamera.finalize();
	orthographicCamera.finalize();

	writeIndex = ( writeIndex + 1 ) % 2;
	readIndex = ( readIndex + 1 ) % 2;

	// swap glyphs
	const int GLYPH_COLLECTION_COUNT = glyphCollections.getSize();
	for( int i=0; i<GLYPH_COLLECTION_COUNT; i++ )
		glyphCollections[i].glyphs[writeIndex].clear();

	// swap quads
	const int QUAD_COLLECTION_COUNT = quadCollections.getSize();
	for( int i=0; i<QUAD_COLLECTION_COUNT; i++ )
		quadCollections[i].quads[writeIndex].clear();

	const int TRANSP_QUAD_COLLECTION_COUNT = transparentQuadCollections.getSize();
	for( int i=0; i<TRANSP_QUAD_COLLECTION_COUNT; i++ )
		transparentQuadCollections[i].quads[writeIndex].clear();

	// swap billboards
	const int BILLBOARD_COLLECTION_COUNT = billboardCollections.getSize();
	for( int i=0; i<BILLBOARD_COLLECTION_COUNT; i++ )
		billboardCollections[i].billboards[writeIndex].clear();

	// swap solids
	solidQueue.swap();
	solidQueue.getWrite().clear();
}

void Graphics::render( float deltaTime )
{
	// set render flags
	glEnable( GL_CULL_FACE );
	glEnable( GL_DEPTH_TEST );
	glEnable( GL_BLEND );
	glBlendFunc( GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA );

	elapsedTime += deltaTime;

	renderSolids();
	renderBillboards();
	renderQuads();
	renderText();
}

void Graphics::renderSolids()
{
	solidShader.bind();
	solidShader.setMat4( solidShaderProjectionLocation, perspectiveCamera.getProjectionMatrix() );
	solidShader.setMat4( solidShaderViewLocation, perspectiveCamera.getViewMatrix() );

	const int SOLID_COUNT = solidQueue.getRead().getSize();

	// render opaque faces
	for( int curSolid = 0; curSolid < SOLID_COUNT; curSolid++ )
	{
		const Solid* solid = solidQueue.getRead()[curSolid];

		const int FACE_COUNT = solid->getFaceCount();
		for( int curFace = 0; curFace < FACE_COUNT; curFace++ )
		{
			int textureIndex = solid->getTextureIndex( curFace );
			const Texture* texture = assets.getTexture( textureIndex );
			if( !texture->hasAlpha() )
			{
				texture->bind();

				GLuint vao = solid->getVAO( curFace );
				int vertexCount = solid->getVertexCount( curFace );

				glBindVertexArray( vao );
				glDrawArrays( GL_TRIANGLES, 0, vertexCount );
			}
		}
	}

	// TODO: Fix depth sorting
	// render transparent faces

	glDepthMask( GL_FALSE );
	for( int curSolid = 0; curSolid < SOLID_COUNT; curSolid++ )
	{
		const Solid* solid = solidQueue.getRead()[curSolid];

		const int FACE_COUNT = solid->getFaceCount();
		for( int curFace = 0; curFace < FACE_COUNT; curFace++ )
		{
			int textureIndex = solid->getTextureIndex( curFace );
			const Texture* texture = assets.getTexture( textureIndex );
			if( texture->hasAlpha() )
			{
				texture->bind();

				GLuint vao = solid->getVAO( curFace );
				int vertexCount = solid->getVertexCount( curFace );

				glBindVertexArray( vao );
				glDrawArrays( GL_TRIANGLES, 0, vertexCount );
			}
		}
	}
	glDepthMask( GL_TRUE );

	glBindVertexArray( 0 );
}

void Graphics::renderBillboards()
{
	billboardShader.bind();
	billboardShader.setMat4( billboardProjectionLocation, perspectiveCamera.getProjectionMatrix() );
	billboardShader.setMat4( billboardViewLocation, perspectiveCamera.getViewMatrix() );

	billboardShader.setFloat( billboardDeltaTimeLocation, elapsedTime );

	glBindVertexArray( billboardVAO );
	glBindBuffer( GL_ARRAY_BUFFER, billboardVBO );

	const int BILLBOARD_COLLECTION_COUNT = billboardCollections.getSize();
	for( int curCollection = 0; curCollection < BILLBOARD_COLLECTION_COUNT; curCollection++ )
	{
		BillboardCollection& collection = billboardCollections[curCollection];

		collection.diffuseMap->bind( GL_TEXTURE0 );
		collection.maskMap->bind( GL_TEXTURE1 );

		billboardShader.setInt( billboardDiffuseLocation, 0 );
		billboardShader.setInt( billboardMaskLocation, 1 );

		const int BILLBOARD_COUNT = collection.billboards[readIndex].getSize();
		int offset = 0;
		while( offset < BILLBOARD_COUNT )
		{
			int count = BILLBOARD_COUNT - offset;
			if( count > GRAPHICS_MAX_BILLBOARDS )
				count = GRAPHICS_MAX_BILLBOARDS;

			glBufferSubData( GL_ARRAY_BUFFER, 0, sizeof(Billboard)*count, collection.billboards[readIndex].getData()+offset );
			glDrawArrays( GL_POINTS, 0, count );

			offset += count;
		}
	}

	glBindVertexArray( 0 );
}

void Graphics::renderQuads()
{
	glDisable( GL_DEPTH_TEST );

	quadShader.bind();
	quadShader.setMat4( quadProjectionLocation, orthographicCamera.getProjectionMatrix() );

	glBindVertexArray( quadVAO );
	glBindBuffer( GL_ARRAY_BUFFER, quadVBO );

	const int QUAD_COLLECTION_COUNT = quadCollections.getSize();
	for( int curCollection = 0; curCollection < QUAD_COLLECTION_COUNT; curCollection++ )
	{
		QuadCollection& collection = quadCollections[curCollection];

		if( collection.texture )
			collection.texture->bind();
		else
			glBindTexture( GL_TEXTURE_2D, 0 );

		const int QUAD_COUNT = collection.quads[readIndex].getSize();
		int offset = 0;
		while( offset < QUAD_COUNT )
		{
			int count = QUAD_COUNT - offset;
			if( count > GRAPHICS_MAX_QUADS )
				count = GRAPHICS_MAX_QUADS;

			glBufferSubData( GL_ARRAY_BUFFER, 0, sizeof(Quad)*count, collection.quads[readIndex].getData()+offset );
			glDrawArrays( GL_POINTS, 0, count );

			offset += count;
		}
	}

	glBindVertexArray( 0 );

	glEnable( GL_DEPTH_TEST );
}

void Graphics::renderText()
{
	glDisable( GL_DEPTH_TEST );

	textShader.bind();
	textShader.setMat4( textProjectionLocation, orthographicCamera.getProjectionMatrix() );

	glBindVertexArray( textVAO );
	glBindBuffer( GL_ARRAY_BUFFER, textVBO );

	const int GLYPH_COLLECTION_COUNT = glyphCollections.getSize();
	for( int curCollection = 0; curCollection < GLYPH_COLLECTION_COUNT; curCollection++ )
	{
		GlyphCollection& collection = glyphCollections[curCollection];

		if( collection.texture )
			collection.texture->bind();
		else
			glBindTexture( GL_TEXTURE_2D, 0 );

		const int GLYPH_COUNT = collection.glyphs[readIndex].getSize();
		int offset = 0;
		while( offset < GLYPH_COUNT )
		{
			int count = GLYPH_COUNT - offset;
			if( count > GRAPHICS_MAX_GLYPHS )
				count = GRAPHICS_MAX_GLYPHS;

			glBufferSubData( GL_ARRAY_BUFFER, 0, sizeof(Glyph)*count, collection.glyphs[readIndex].getData()+offset );
			glDrawArrays( GL_POINTS, 0, count );

			offset += count;
		}
	}

	glBindVertexArray( 0 );

	glEnable( GL_DEPTH_TEST );
}

void Graphics::queueSolid( const Solid* solid )
{
	solidQueue.getWrite().add( solid );
}

void Graphics::queueQuad( int textureIndex, const glm::vec3& position, const glm::vec2& size, const glm::vec2& uvStart, const glm::vec2& uvEnd, const glm::vec4& color )
{
	if( color.a < 1.0f - EPSILON ) // transparent
	{
		const Texture* texture = NULL;
		if( textureIndex >= 0 )
			texture = assets.getTexture( textureIndex );

		const int QUAD_COLLECTION_COUNT = transparentQuadCollections.getSize();
		int collectionIndex = -1;
		for( int i=0; i<QUAD_COLLECTION_COUNT && collectionIndex < 0; i++ )
			if( transparentQuadCollections[i].texture == texture )
				collectionIndex = i;

		if( collectionIndex < 0 )
		{
			QuadCollection& collection = transparentQuadCollections.append();
			collection.texture = texture;
			collection.quads[writeIndex].expand( GRAPHICS_MAX_QUADS );
			collection.quads[readIndex].expand( GRAPHICS_MAX_QUADS );

			collectionIndex = QUAD_COLLECTION_COUNT;
		}

		QuadCollection& collection = transparentQuadCollections[collectionIndex];
		collection.quads[writeIndex].add( { position, size, uvStart, uvEnd, color } );
	}
	else // opaque
	{
		const Texture* texture = NULL;
		if( textureIndex >= 0 )
			texture = assets.getTexture( textureIndex );

		const int QUAD_COLLECTION_COUNT = quadCollections.getSize();
		int collectionIndex = -1;
		for( int i=0; i<QUAD_COLLECTION_COUNT && collectionIndex < 0; i++ )
			if( quadCollections[i].texture == texture )
				collectionIndex = i;

		if( collectionIndex < 0 )
		{
			QuadCollection& collection = quadCollections.append();
			collection.texture = texture;
			collection.quads[writeIndex].expand( GRAPHICS_MAX_QUADS );
			collection.quads[readIndex].expand( GRAPHICS_MAX_QUADS );

			collectionIndex = QUAD_COLLECTION_COUNT;
		}

		QuadCollection& collection = quadCollections[collectionIndex];
		collection.quads[writeIndex].add( { position, size, uvStart, uvEnd, color } );
	}
}

void Graphics::queueText( int fontIndex, const char* text, const glm::vec3& position, const glm::vec4& color )
{
	const Font* font = assets.getFont( fontIndex );

	const int GLYPH_COLLECTION_COUNT = glyphCollections.getSize();
	int collectionIndex = -1;
	for( int i=0; i<GLYPH_COLLECTION_COUNT && collectionIndex < 0; i++ )
		if( glyphCollections[i].texture == font->getTexture() )
			collectionIndex = i;

	if( collectionIndex < 0 )
	{
		GlyphCollection& collection = glyphCollections.append();
		collection.texture = font->getTexture();
		collection.glyphs[writeIndex].expand( GRAPHICS_MAX_GLYPHS );
		collection.glyphs[readIndex].expand( GRAPHICS_MAX_GLYPHS );

		collectionIndex = GLYPH_COLLECTION_COUNT;
	}

	GlyphCollection& collection = glyphCollections[collectionIndex];

	glm::vec3 offset;
	int index = 0;

	const char* cur = text;
	while( *cur )
	{
		if( *cur == '\n' )
		{
			offset.x = 0;
			offset.y += font->getHeight();
		}
		else if( *cur == '\t' )
		{
			offset.x += font->getWidth( 0 ) * FONT_TAB_WIDTH;
		}
		else if( *cur >= FONT_FIRST && *cur <= FONT_LAST )
		{
			char c = *cur - FONT_FIRST;

			Glyph& glyph = collection.glyphs[writeIndex].append();

			glyph.position = position + offset;

			// avoid sub-pixel positions
			glyph.position.x = (int)( glyph.position.x + 0.5f );
			glyph.position.y = (int)( glyph.position.y + 0.5f );

			font->getUV( c, &glyph.uv );
			glyph.size.x = (float)font->getWidth( c );
			glyph.size.y = (float)font->getHeight();
			glyph.color = color;

			offset.x += glyph.size.x;

			index++;
		}

		cur++;
	}
}

void Graphics::queueBillboard( int diffuseIndex, int maskIndex, const glm::vec3& position, const glm::vec2& size, const glm::vec4& uv, bool spherical, const glm::vec3& scroll )
{
	const Texture* diffuseMap = assets.getTexture( diffuseIndex );
	const Texture* maskMap = assets.getTexture( maskIndex );

	const int BILLBOARD_COLLECTION_COUNT = billboardCollections.getSize();

	int index = -1;
	for( int i=0; i<BILLBOARD_COLLECTION_COUNT && index < 0; i++ )
	{
		if( billboardCollections[i].diffuseMap == diffuseMap &&
			billboardCollections[i].maskMap == maskMap )
		{
			index = i;
		}
	}

	if( index < 0 ) // these are new textures
	{
		BillboardCollection& collection = billboardCollections.append();
		collection.diffuseMap = diffuseMap;
		collection.maskMap = maskMap;
		collection.billboards[writeIndex].expand( GRAPHICS_MAX_BILLBOARDS );
		collection.billboards[readIndex].expand( GRAPHICS_MAX_BILLBOARDS );

		index = BILLBOARD_COLLECTION_COUNT;
	}

	Billboard& billboard = billboardCollections[index].billboards[writeIndex].append();
	billboard.position = position;
	billboard.uv = uv;
	billboard.size = size;
	billboard.spherical = ( spherical ? 1.0f : 0.0f );
	billboard.scroll = scroll;
}

Camera* Graphics::getPerspectiveCamera()
{
	return &perspectiveCamera;
}

Camera* Graphics::getOrthographicCamera()
{
	return &orthographicCamera;
}

Assets* Graphics::getAssets()
{
	return &assets;
}