using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Editor
{
	public static class GL
	{
		private const string DLL_PATH = "./Editor.GL.dll";

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void setPixelFormat( IntPtr windowHandle );
		
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static IntPtr createContext( IntPtr windowHandle, int width, int height );
		
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void destroyContext( IntPtr context );
		
		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void clearColor( float r, float g, float b, float a );

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void swapBuffers( IntPtr windowHandle );

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void beginPoints();

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void beginLines();

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void beginTriangles();

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void end();

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void texCoord2f( float u, float v );

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void vertex2f( float u, float v );

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void vertex3f( float x, float y, float z );

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void color4f( float r, float g, float b, float a );

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void viewMatrix( float px, float py, float pz, float dx, float dy, float dz );

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static UInt32 loadTexture( string path );

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void unloadTexture( UInt32 id );

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static UInt32 uploadTexture( int width, int height, int bpp, byte[] pixels, bool flipVertically );

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void setTexture( UInt32 texture );

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void pointSize( float size );

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void enablePointSprite( bool enabled );

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void enableDepthMask( bool enabled );

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void unproject( int x, int y, int z, ref float outx, ref float outy, ref float outz );

		private static IntPtr _context;
		
		public static void CreateContext( IntPtr windowHandle, int width, int height )
		{
			setPixelFormat( windowHandle );
			_context = createContext( windowHandle, width, height );
		}
		
		public static void DestroyContext( IntPtr context )
		{
			destroyContext( _context );
		}
		
		public static void ClearColor( float r, float g, float b, float a )
		{
			clearColor( r, g, b, a );
		}

		public static void SwapBuffers( IntPtr windowHandle )
		{
			swapBuffers( windowHandle );
		}

		public static void BeginPoints()
		{
			beginPoints();
		}

		public static void BeginLines()
		{
			beginLines();
		}

		public static void BeginTriangles()
		{
			beginTriangles();
		}

		public static void End()
		{
			end();
		}

		public static void TexCoord2f( float u, float v )
		{
			texCoord2f( u, v );
		}

		public static void Vertex2f( float u, float v )
		{
			vertex2f( u, v );
		}

		public static void Vertex3f( float x, float y, float z )
		{
			vertex3f( x, y, z );
		}

		public static void Color4f( float r, float g, float b, float a )
		{
			color4f( r, g, b, a );
		}

		public static void Color4f( float all )
		{
			color4f( all, all, all, all );
		}

		public static void Color4f( float rgb, float a )
		{
			color4f( rgb, rgb, rgb, a );
		}

		public static void ViewMatrix( Triple position, Triple direction )
		{
			viewMatrix( position.X, position.Y, position.Z, direction.X, direction.Y, direction.Z );
		}

		public static UInt32 LoadTexture( string path )
		{
			var id = loadTexture( path );
			return id;
		}

		public static void UnloadTexture( UInt32 id )
		{
			unloadTexture( id );
		}

		public static UInt32 UploadTexture( int width, int height, int bpp, byte[] pixels, bool flipVertically = true )
		{
			var id = uploadTexture( width, height, bpp, pixels, flipVertically );
			return id;
		}

		public static void SetTexture( UInt32 texture )
		{
			setTexture( texture );
		}

		public static void PointSize( float size )
		{
			pointSize( size );
		}

		public static void EnablePointSprite( bool enabled )
		{
			enablePointSprite( enabled );
		}

		public static void EnableDepthMask( bool enabled )
		{
			enableDepthMask( enabled );
		}

		public static Triple Unproject( int x, int y, int z )
		{
			float outx = 0.0f, outy = 0.0f, outz = 0.0f;
			unproject( x, y, z, ref outx, ref outy, ref outz );

			return new Triple( outx, outy, outz );
		}
	}
}
