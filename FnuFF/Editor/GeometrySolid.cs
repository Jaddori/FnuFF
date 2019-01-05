using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using Editor.UndoRedo;

namespace Editor
{
	public class GeometrySolid
	{
		private static Random random = new Random();
		//private static UInt32 textureID = UInt32.MaxValue;

		[XmlIgnore]
		public static CommandStack CommandStack;

		private Color _color;
		private bool _hovered;
		private bool _selected;
		private List<Face> _faces;

		[XmlIgnore]
		public Color Color { get { return _color; } set { _color = value; } }

		[XmlIgnore]
		public bool Hovered { get { return _hovered; } set { _hovered = value; } }

		[XmlIgnore]
		public bool Selected { get { return _selected; } set { _selected = value; } }

		public List<Face> Faces => _faces;

		public GeometrySolid()
		{
			//if( textureID == UInt32.MaxValue )
			//textureID = GL.LoadTexture( "./assets/textures/bricks.dds" );

			_hovered = _selected = false;
			_faces = new List<Face>();

			GenerateColor();
		}

		public GeometrySolid( Triple min, Triple max )
		{
			//if( textureID == UInt32.MaxValue )
			//textureID = GL.LoadTexture( "./assets/textures/bricks.dds" );

			_hovered = _selected = false;
			_faces = new List<Face>();

			GenerateColor();
			GenerateFaces( min, max );
		}

		private void GenerateColor()
		{
			var r = random.NextDouble();
			var g = random.NextDouble();
			var b = random.NextDouble();

			var magnitude = Math.Sqrt( r * r + g * g + b * b );

			r /= magnitude;
			g /= magnitude;
			b /= magnitude;

			_color = Color.FromArgb( (int)( r * 255 ), (int)( g * 255 ), (int)( b * 255 ) );
		}

		public void GenerateFaces( Triple min, Triple max )
		{
			// left
			var left = new Face();
			left.Plane.Normal = new Triple( 0, 0, -1 );
			left.Plane.D = min.Dot( left.Plane.Normal );

			// right
			var right = new Face();
			right.Plane.Normal = new Triple( 0, 0, 1 );
			right.Plane.D = max.Dot( right.Plane.Normal );

			// top
			var top = new Face();
			top.Plane.Normal = new Triple( 0, 1, 0 );
			top.Plane.D = max.Dot( top.Plane.Normal );

			// bottom
			var bottom = new Face();
			bottom.Plane.Normal = new Triple( 0, -1, 0 );
			bottom.Plane.D = min.Dot( bottom.Plane.Normal );

			// front
			var front = new Face();
			front.Plane.Normal = new Triple( -1, 0, 0 );
			front.Plane.D = min.Dot( front.Plane.Normal );

			// back
			var back = new Face();
			back.Plane.Normal = new Triple( 1, 0, 0 );
			back.Plane.D = max.Dot( back.Plane.Normal );

			_faces.AddRange( new[] { left, right, top, bottom, front, back } );
		}

		public bool Clip( Plane plane )
		{
			var didClip = false;

			// check if any faces need to removed
			var allVerticesBehind = true;
			var facesToRemove = new List<Face>();
			foreach( var face in _faces )
			{
				var otherPlanes = _faces.Where( x => x != face ).Select( x => x.Plane ).ToArray();
				var points = Extensions.IntersectPlanes( face.Plane, otherPlanes );

				var allInFront = true;
				for( int i = 0; i < points.Length && allInFront; i++ )
				{
					if( !plane.InFront( points[i] ) )
						allInFront = false;
					else
						allVerticesBehind = false;
				}

				if( allInFront )
				{
					facesToRemove.Add( face );
				}
			}

			// make sure at least one vertex is in front and one behind the clipping plane
			if( !allVerticesBehind && facesToRemove.Count < _faces.Count )
			{
				// remove faces that are behind the plane
				for( int i = 0; i < facesToRemove.Count; i++ )
					_faces.Remove( facesToRemove[i] );

				// add new face
				var newFace = new Face( plane.Normal, plane.D );
				_faces.Add( newFace );

				didClip = true;
			}

			return didClip;
		}

		public void Paint3D()
		{
			float red = _color.R / 255.0f;
			float green = _color.G / 255.0f;
			float blue = _color.B / 255.0f;
			float alpha = 1.0f;

			if( _selected )
			{
				red = green = blue = 1.0f;
			}

			var cur = 0;
			foreach( var face in _faces )
			{
				//cur++;
				//if( cur != 4 )
				//	continue;

				var otherPlanes = _faces.Where( x => x != face ).Select( x => x.Plane ).ToArray();
				var points = Extensions.IntersectPlanes( face.Plane, otherPlanes );
				var indices = Extensions.WindingIndex3D( points, face.Plane.Normal );
				var texCoords = points.Select( x => Extensions.EmitTextureCoordinates( face.Plane.Normal, x ) ).ToArray();
				
				var rr = red;
				var gg = green;
				var bb = blue;

				if( face.Plane.Normal.Dot( new Triple( 1, 0, 0 ) ) < 0 ||
					face.Plane.Normal.Dot( new Triple( 0, 1, 0 ) ) < 0 ||
					face.Plane.Normal.Dot( new Triple( 0, 0, 1 ) ) < 0 )
				{
					rr -= 0.25f;
					gg -= 0.25f;
					bb -= 0.25f;
				}

				if( string.IsNullOrEmpty( face.TextureName ) )
				{
					GL.SetTexture( 0 );
					GL.Color4f( rr, gg, bb, alpha );
				}
				else
				{
					//var textureID = TextureMap.GetID( face.TextureName );
					/*UInt32 textureID = 0;
					GL.SetTexture( textureID );
					GL.Color4f( 1.0f, 1.0f, 1.0f, 1.0f );*/
				}

				var textureID = TextureMap.GetID( "pack02", "bricks" );
				GL.SetTexture( textureID );
				GL.Color4f( 1.0f, 1.0f, 1.0f, 1.0f );

				if( face.Hovered )
				{
					GL.Color4f( 1.0f, 0.5f, 0.5f, 1.0f );
				}

				GL.BeginTriangles();

				var v0 = points[indices[0]];
				var uv0 = texCoords[indices[0]];

				for( int j = 1; j < indices.Length - 1; j++ )
				{
					var i1 = indices[j];
					var i2 = indices[j + 1];

					var v1 = points[i1];
					var v2 = points[i2];

					var uv1 = texCoords[i1];
					var uv2 = texCoords[i2];

					var v1v0 = v0 - v1;
					var v2v0 = v0 - v2;

					var n = v2v0.Cross( v1v0 );
					n.Normalize();

					var flip = face.Plane.Normal.Dot( n ) > 0;
					if( flip )
					{
						var temp = v2;
						v2 = v1;
						v1 = temp;

						var tempUV = uv2;
						uv2 = uv1;
						uv1 = tempUV;
					}

					//GL.Color4f( rr, gg, bb, alpha );
					GL.TexCoord2f( uv0.X, uv0.Y );
					GL.Vertex3f( v0.X, v0.Y, v0.Z );

					GL.TexCoord2f( uv1.X, uv1.Y );
					GL.Vertex3f( v1.X, v1.Y, v1.Z );

					GL.TexCoord2f( uv2.X, uv2.Y );
					GL.Vertex3f( v2.X, v2.Y, v2.Z );
				}

				GL.End();
			}

			GL.SetTexture( 0 );
		}
    }
}
