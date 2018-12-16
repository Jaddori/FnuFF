#include "editor_gl.h"

EXPORT void setPixelFormat( HWND windowHandle )
{
	PIXELFORMATDESCRIPTOR pfd =
	{ 
		sizeof(PIXELFORMATDESCRIPTOR),  //  size of this pfd  
		1,                     // version number  
		PFD_DRAW_TO_WINDOW |   // support window  
		PFD_SUPPORT_OPENGL |   // support OpenGL  
		PFD_DOUBLEBUFFER,      // double buffered  
		PFD_TYPE_RGBA,         // RGBA type  
		24,                    // 24-bit color depth  
		0, 0, 0, 0, 0, 0,      // color bits ignored  
		0,                     // no alpha buffer  
		0,                     // shift bit ignored  
		0,                     // no accumulation buffer  
		0, 0, 0, 0,            // accum bits ignored  
		32,                    // 32-bit z-buffer      
		0,                     // no stencil buffer  
		0,                     // no auxiliary buffer  
		PFD_MAIN_PLANE,        // main layer  
		0,                     // reserved  
		0, 0, 0                // layer masks ignored  
	};

	HDC deviceHandle = GetDC( windowHandle );
	int pixelFormat = ChoosePixelFormat( deviceHandle, &pfd );

	SetPixelFormat( deviceHandle, pixelFormat, &pfd );
	ReleaseDC( windowHandle, deviceHandle );
}

EXPORT HGLRC createContext( HWND windowHandle, int width, int height )
{
	HDC deviceHandle = GetDC( windowHandle );

	HGLRC context = wglCreateContext( deviceHandle );
	if( context )
	{
		BOOL result = wglMakeCurrent( deviceHandle, context );

		if( result )
		{
			glViewport( 0, 0, width, height );
			
			glMatrixMode( GL_PROJECTION );
			glLoadIdentity();
			gluPerspective( 90.0f, (float)width / (float)height, 0.1f, 1000.0f );

			glMatrixMode( GL_MODELVIEW );
			glLoadIdentity();

			glDisable( GL_CULL_FACE );
			glDisable( GL_DEPTH_TEST );
		}
		else
		{
			DWORD error = GetLastError();
			char str[128] = {};
			_snprintf( str, 128, "Error code: %d", error );
			MessageBoxA( NULL, str, "GL - Make current Error", MB_OK );
		}
	}
	else
	{
		DWORD error = GetLastError();
		char str[128] = {};
		_snprintf( str, 128, "Error code: %d", error );
		MessageBoxA( NULL, str, "GL - Create context Error", MB_OK );
	}

	ReleaseDC( windowHandle, deviceHandle );

	return context;
}

EXPORT void destroyContext( HGLRC context )
{
	wglMakeCurrent( NULL, NULL );
	wglDeleteContext( context );
}

EXPORT void clearColor( float r, float g, float b, float a )
{
	glClearColor( r, g, b, a );
	glClear( GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT );
}

EXPORT void swapBuffers( HWND windowHandle )
{
	HDC deviceHandle = GetDC( windowHandle );

	BOOL result = SwapBuffers( deviceHandle );
	if( !result )
	{
		DWORD error = GetLastError();
		char str[128] = {};
		_snprintf( str, 128, "Error code: %d", error );
		MessageBoxA( NULL, str, "GL - Swap buffers Error", MB_OK );
	}

	ReleaseDC( windowHandle, deviceHandle );
}

EXPORT void begin()
{
	glBegin( GL_TRIANGLES );
}

EXPORT void end()
{
	glEnd();
}

EXPORT void vertex2f( float u, float v )
{
	glVertex2f( u, v );
}

EXPORT void vertex3f( float x, float y, float z )
{
	glVertex3f( x, y, z );
}

EXPORT void color4f( float r, float g, float b, float a )
{
	glColor4f( r, g, b, a );
}