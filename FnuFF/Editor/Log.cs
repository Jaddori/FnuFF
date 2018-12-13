using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Editor
{
	public static class Log
	{
		public static Dictionary<string, List<Func<string>>> MessageFunctors { get; } = new Dictionary<string, List<Func<string>>>();

		public static void AddFunctor( string name, Func<string> functor )
		{
			if( !MessageFunctors.ContainsKey( name ) )
				MessageFunctors.Add( name, new List<Func<string>>() );

			MessageFunctors[name].Add( functor );
		}
		
		public static void Paint( string name, Graphics g )
		{
			if( !MessageFunctors.ContainsKey( name ) )
				return;

			var position = new Point( 4, 4 );

			using( var brush = new SolidBrush( Color.LightGray ) )
			{
				using( var font = new Font( FontFamily.GenericSansSerif, 8.0f ) )
				{
					foreach( var func in MessageFunctors[name] )
					{
						var str = func();
						g.DrawString( str, font, brush, position );
						position.Y += (int)g.MeasureString(str, font).Height + 4;
					}
				}
			}
		}
	}
}
