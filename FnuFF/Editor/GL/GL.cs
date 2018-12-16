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
		private extern static void begin();

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void end();

		[DllImport( DLL_PATH, CallingConvention = CallingConvention.Cdecl )]
		private extern static void vertex3f( float x, float y, float z );

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

		public static void Begin()
		{
			begin();
		}

		public static void End()
		{
			end();
		}

		public static void Vertex3f( float x, float y, float z )
		{
			vertex3f( x, y, z );
		}
	}
}
