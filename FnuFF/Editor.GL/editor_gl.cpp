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
		int32_t magicNumber;
		fread( &magicNumber, sizeof(magicNumber), 1, file );

		if( magicNumber == DDS_MAGIC_NUMBER )
		{
			DDS_HEADER header = {};
			fread( &header, sizeof(header), 1, file );

			int width = header.width;
			int height = header.height;
			int size = header.pitchOrLinearSize;

			int format = -1;
			switch( header.format.fourCC )
			{
				default:
				case ID_DXT1: format = GL_COMPRESSED_RGBA_S3TC_DXT1_EXT; break;
				case ID_DXT3: format = GL_COMPRESSED_RGBA_S3TC_DXT3_EXT; break;
				case ID_DXT5: format = GL_COMPRESSED_RGBA_S3TC_DXT5_EXT; break;
			}

			GLbyte* pixels = new GLbyte[size];
			fread( pixels, 1, size, file );

			GLuint id;
			glGenTextures( 1, &id );
			glBindTexture( GL_TEXTURE_2D, id );
			glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR );
			glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR );
			glCompressedTexImage2D( GL_TEXTURE_2D, 0, format, width, height, 0, size, pixels );

			result = id;
			delete[] pixels;
		}

		fclose( file );
	}

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