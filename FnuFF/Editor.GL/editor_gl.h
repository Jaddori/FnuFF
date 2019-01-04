#pragma once

#include <Windows.h>
//#include <gl/GL.h>
//#include <gl/GLU.h>
#include "GL\glew.h"
#include <stdio.h>
#include <stdint.h>

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

#define EXPORT extern "C" __declspec(dllexport)

EXPORT void setPixelFormat( HWND windowHandle );
EXPORT HGLRC createContext( HWND windowHandle, int width, int height );
EXPORT void destroyContext( HGLRC context );

EXPORT void clearColor( float r, float g, float b, float a );
EXPORT void swapBuffers( HWND windowHandle );

EXPORT void beginPoints();
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

EXPORT void pointSize( float size );
EXPORT void enablePointSprite( bool enabled );
EXPORT void enableDepthMask( bool enabled );

EXPORT void unproject( int x, int y, int z, float* outx, float* outy, float* outz );