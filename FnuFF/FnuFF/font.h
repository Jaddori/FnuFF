#pragma once

#include "common.h"
#include "texture.h"

#define FONT_FIRST 32
#define FONT_LAST 127
#define FONT_RANGE (FONT_LAST-FONT_FIRST)
#define FONT_TAB_WIDTH 4

namespace Rendering
{
	struct FontInfo
	{
		uint8_t height, paddingX, paddingY, shadowX, shadowY;
		uint16_t bitmapSize;
		uint8_t widths[FONT_RANGE];
		uint16_t horizontalOffsets[FONT_RANGE];
		uint16_t verticalOffsets[FONT_RANGE];
	};

	class Font
	{
	public:
		Font();
		~Font();

		bool load( const char* infoPath, const char* texturePath );
		void unload();
		void upload();

		void measureText( const char* text, glm::vec2* result );

		int getBitmapSize() const;
		int getHeight() const;
		int getWidth( char c ) const;
		int getHorizontalOffset( char c ) const;
		int getVerticalOffset( char c ) const;
		void getUV( char c, glm::vec4* result ) const;
		bool getUploaded() const;

		const FontInfo& getInfo() const;
		const Texture* getTexture() const;

	private:
		FontInfo info;
		Texture texture;
		bool uploaded;
	};
};