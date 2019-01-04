#pragma once

#include "common.h"

namespace Rendering
{
#pragma pack(push, 1)
	struct TargaHeader
	{
		int8_t idLength;
		int8_t colormapType;
		int8_t imageType;
		int16_t colormapOrigin;
		int16_t colormapLength;
		int8_t colormapDepth;
		int16_t xorigin;
		int16_t yorigin;
		int16_t width;
		int16_t height;
		int8_t bpp;
		int8_t imageDescriptor;
	};
#pragma pack(pop)

	class Texture
	{
	public:
		Texture();
		~Texture();

		bool load( const char* path );
		void unload();
		void upload();

		inline void bind( GLenum target = GL_TEXTURE0 ) const
		{
			glActiveTexture( target );
			glBindTexture( GL_TEXTURE_2D, id );
		}

		GLuint getID() const;
		int getWidth() const;
		int getHeight() const;

	private:
		GLuint id;
		int width, height, size;

		GLenum format;
		GLbyte* pixels;

		bool uploaded;
	};
}