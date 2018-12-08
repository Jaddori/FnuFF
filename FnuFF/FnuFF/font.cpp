#include "font.h"
using namespace Rendering;

Font::Font()
{
}

Font::~Font()
{
}

bool Font::load( const char* infoPath, const char* texturePath )
{
	bool result = false;

	FILE* file = fopen( infoPath, "rb" );
	if( file )
	{
		fread( &info, sizeof(info), 1, file );
		fclose( file );

		LOG_INFO( "Loaded font from \"%s\".", infoPath );

		result = texture.load( texturePath );
	}
	else
	{
		LOG_ERROR( "Failed to load font from \"%s\".", infoPath );
	}

	return result;
}

void Font::unload()
{
	texture.unload();
}

void Font::upload()
{
	if( !uploaded )
		texture.upload();
	uploaded = true;
}

void Font::measureText( const char* text, glm::vec2* result )
{
	glm::vec2 temp( 0.0f, info.height );
	float currentX = 0.0f;

	const char* cur = text;
	while( *cur )
	{
		if( *cur == '\n' )
		{
			if( currentX > temp.x )
				temp.x = currentX;
			temp.y += info.height;
		}
		else if( *cur == '\t' )
		{
			currentX += info.widths[0] * FONT_TAB_WIDTH;
		}
		else if( *cur >= FONT_FIRST && *cur <= FONT_LAST )
		{
			char c = *cur - FONT_FIRST;
			currentX += info.widths[c];
		}

		cur++;
	}

	if( currentX >= temp.x )
		temp.x = currentX;

	result->x = temp.x;
	result->y = temp.y;
}

int Font::getBitmapSize() const
{
	return info.bitmapSize;
}

int Font::getHeight() const
{
	return info.height;
}

int Font::getWidth( char c ) const
{
	LOG_ASSERT( c >= 0 && c <= FONT_RANGE, "Character outside font range in getWidth." );
	return info.widths[c];
}

int Font::getHorizontalOffset( char c ) const
{
	LOG_ASSERT( c >= 0 && c <= FONT_RANGE, "Character outside font range in getHorizontalOffset." );
	return info.horizontalOffsets[c];
}

int Font::getVerticalOffset( char c ) const
{
	LOG_ASSERT( c >= 0 && c <= FONT_RANGE, "Character outside font range in getVerticalOffset." );
	return info.verticalOffsets[c];
}

void Font::getUV( char c, glm::vec4* result ) const
{
	LOG_ASSERT( c >= 0 && c <= FONT_RANGE, "Character outside font range in getUV." );

	float s = (float)info.horizontalOffsets[c];
	float t = (float)info.verticalOffsets[c]-info.height;
	float u = s + info.widths[c];
	float v = t + info.height;
	
	*result = ( glm::vec4( s, t, u, v ) / (float)info.bitmapSize );
}

bool Font::getUploaded() const
{
	return uploaded;
}

const FontInfo& Font::getInfo() const
{
	return info;
}

const Texture* Font::getTexture() const
{
	return &texture;
}