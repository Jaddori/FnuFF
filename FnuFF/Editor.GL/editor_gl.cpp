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
			gluPerspective( 90.0f, (float)width / (float)height, 0.1f, 100.0f );

			glMatrixMode( GL_MODELVIEW );
			glLoadIdentity();

			glEnable( GL_CULL_FACE );
			glEnable( GL_DEPTH_TEST );
			glEnable( GL_TEXTURE_2D );
			glEnable( GL_BLEND );
			glBlendFunc( GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA );

			glPointSize(8.0f);

			glewInit();
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

EXPORT void beginPoints()
{
	glBegin( GL_POINTS );
}

EXPORT void beginLines()
{
	glBegin( GL_LINES );
}

EXPORT void beginTriangles()
{
	glBegin( GL_TRIANGLES );
}

EXPORT void end()
{
	glEnd();
}

EXPORT void texCoord2f( float u, float v )
{
	glTexCoord2f( u, v );
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

EXPORT void viewMatrix( float px, float py, float pz, float dx, float dy, float dz )
{
	glMatrixMode( GL_MODELVIEW );
	glLoadIdentity();

	gluLookAt( px, py, pz, px+dx, py+dy, pz+dz, 0, 1, 0 );
}

EXPORT uint32_t loadTexture( const char* path )
{
	uint32_t result = 0;

	FILE* file = fopen( path, "rb" );
	if( file )
	{
		TargaHeader header;
		fread( &header, sizeof(header), 1, file );

		int bpp = header.bpp / 8;
		int size = header.width * header.height * bpp;

		GLenum format = GL_BGR;
		if( bpp == 4 )
			format = GL_BGRA;

		GLubyte* pixels = new GLubyte[size];
		fread( pixels, 1, size, file );

		fclose( file );

		// need flip image data vertically
		if( header.yorigin == 0 )
		{
			char* buffer = new char[header.width*bpp];
			int lineSize = header.width * bpp;
			for( int y=0; y<header.height / 2; y++ )
			{
				GLubyte* top = pixels + y * lineSize;
				GLubyte* bottom = pixels + ( header.height - y - 1  ) * lineSize;

				memcpy( buffer, top, lineSize );
				memcpy( top, bottom, lineSize );
				memcpy( bottom, buffer, lineSize );
			}
			delete[] buffer;
		}

		glGenTextures( 1, &result );

		glBindTexture( GL_TEXTURE_2D, result );
		glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST );
		glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST );
		glTexImage2D( GL_TEXTURE_2D, 0, GL_RGBA, header.width, header.height, 0, format, GL_UNSIGNED_BYTE, pixels );

		glBindTexture( GL_TEXTURE_2D, 0 );

		delete[] pixels;
	}

	return result;
}

EXPORT uint32_t uploadTexture( int width, int height, int bpp, char* pixels, bool flipVertically )
{
	uint32_t result = 0;

	GLenum format = GL_BGR;
	if( bpp == 4 )
		format = GL_BGRA;

	if( flipVertically )
	{
		int lineSize = width*bpp;
		char* buffer = new char[lineSize];

		for( int y=0; y<height/2; y++ )
		{
			char* top = pixels + y * lineSize;
			char* bottom = pixels + ( height - y - 1 ) * lineSize;

			memcpy( buffer, top, lineSize );
			memcpy( top, bottom, lineSize );
			memcpy( bottom, buffer, lineSize );
		}

		delete[] buffer;
	}

	glGenTextures( 1, &result );

	glBindTexture( GL_TEXTURE_2D, result );
	glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST );
	glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST );
	glTexImage2D( GL_TEXTURE_2D, 0, GL_RGBA, width, height, 0, format, GL_UNSIGNED_BYTE, pixels );
	glBindTexture( GL_TEXTURE_2D, 0 );

	return result;
}

EXPORT void setTexture( uint32_t id )
{
	glBindTexture( GL_TEXTURE_2D, id );
}

EXPORT void pointSize( float size )
{
	glPointSize( size );
}

EXPORT void enablePointSprite( bool enabled )
{
	if( enabled )
	{
		glEnable( GL_POINT_SPRITE );
		glTexEnvi( GL_POINT_SPRITE, GL_COORD_REPLACE, GL_TRUE );
	}
	else
		glDisable( GL_POINT_SPRITE );
}

EXPORT void enableDepthMask( bool enabled )
{
	glDepthMask( enabled ? GL_TRUE : GL_FALSE );
}

EXPORT void unproject( int x, int y, int z, float* outx, float* outy, float* outz )
{
	GLdouble viewMatrix[16];
	GLdouble projectionMatrix[16];
	GLint viewport[4];

	glGetDoublev( GL_MODELVIEW_MATRIX, viewMatrix );
	glGetDoublev( GL_PROJECTION_MATRIX, projectionMatrix );
	glGetIntegerv( GL_VIEWPORT, viewport );

	GLdouble gx, gy, gz;
	gluUnProject( x, y, z, viewMatrix, projectionMatrix, viewport, &gx, &gy, &gz );

	*outx = gx;
	*outy = gy;
	*outz = gz;
}