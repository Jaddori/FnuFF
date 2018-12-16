#pragma once

#include <Windows.h>
#include <gl/GL.h>
#include <gl/GLU.h>
#include <stdio.h>

#define EXPORT extern "C" __declspec(dllexport)

EXPORT void setPixelFormat( HWND windowHandle );
EXPORT HGLRC createContext( HWND windowHandle, int width, int height );
EXPORT void destroyContext( HGLRC context );

EXPORT void clearColor( float r, float g, float b, float a );
EXPORT void swapBuffers( HWND windowHandle );

EXPORT void begin();
EXPORT void end();
EXPORT void vertex2f( float u, float v );
EXPORT void vertex3f( float x, float y, float z );
EXPORT void color4f( float r, float g, float b, float a );