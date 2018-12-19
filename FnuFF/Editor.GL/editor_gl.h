#pragma once

#include <Windows.h>
//#include <gl/GL.h>
//#include <gl/GLU.h>
#include "GL\glew.h"
#include <stdio.h>
#include <stdint.h>

#define DDS_MAGIC_NUMBER 0x20534444 // 'DDS ' in hex
#define ID_DXT1 0x31545844
#define ID_DXT3 0x33545844
#define ID_DXT5 0x35545844

#define EXPORT extern "C" __declspec(dllexport)

struct DDS_PIXELFORMAT
{
	int32_t size;
	int32_t flags;
	int32_t fourCC;
	int32_t bitCount;
	int32_t rMask;
	int32_t gMask;
	int32_t bMask;
	int32_t aMask;
};

struct DDS_HEADER
{
	int32_t				size;
	int32_t				flags;
	int32_t				height;
	int32_t				width;
	int32_t				pitchOrLinearSize;
	int32_t				depth;
	int32_t				mipMaps;
	int32_t				reserved1[11];
	DDS_PIXELFORMAT		format;
	int32_t				caps1;
	int32_t				caps2;
	int32_t				caps3;
	int32_t				caps4;
	int32_t				reserved2;
};

EXPORT void setPixelFormat( HWND windowHandle );
EXPORT HGLRC createContext( HWND windowHandle, int width, int height );
EXPORT void destroyContext( HGLRC context );

EXPORT void clearColor( float r, float g, float b, float a );
EXPORT void swapBuffers( HWND windowHandle );

EXPORT void beginLines();
EXPORT void beginTriangles();
EXPORT void end();
EXPORT void texCoord2f( float u, float v );
EXPORT void vertex2f( float u, float v );
EXPORT void vertex3f( float x, float y, float z );
EXPORT void color4f( float r, float g, float b, float a );

EXPORT void viewMatrix( float px, float py, float pz, float dx, float dy, float dz );

EXPORT uint32_t loadTexture( const char* path );
EXPORT void setTexture( uint32_t id );