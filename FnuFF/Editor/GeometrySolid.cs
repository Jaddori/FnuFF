using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using Editor.UndoRedo;

namespace Editor
{
	public class GeometrySolid
	{
		private const float CLIP_MARGIN = 0.01f;
		public const int DIMENSION_TEXT_OFFSET = 12;
		private static Random random = new Random();

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
			_hovered = _selected = false;
			_faces = new List<Face>();

			GenerateColor();
		}

		public GeometrySolid( Triple min, Triple max )
		{
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

			foreach( var face in _faces )
			{
				face.PackName = TextureMap.CurrentPackName;
				face.TextureName = TextureMap.CurrentTextureName;

				face.BuildVertices( this );
			}
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
				for( int i = 0; i < points.Length && ( allInFront || allVerticesBehind ); i++ )
				{
					if( !plane.InFront( points[i], CLIP_MARGIN ) )
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
				newFace.PackName = _faces.Last().PackName;
				newFace.TextureName = _faces.Last().TextureName;
				_faces.Add( newFace );

				// rebuild vertices
				foreach( var face in _faces )
				{
					face.BuildVertices( this );
				}

				didClip = true;
			}

			return didClip;
		}

		public void Move( Triple movement )
		{
			foreach( var face in _faces )
			{
				for( int i = 0; i < face.Vertices.Count; i++ )
				{
					face.Vertices[i] += movement;
				}

				face.BuildPlane();
			}
		}

		public void Paint2D( Graphics g, Camera2D camera )
		{
			var color = Color.FromArgb( EditorColors.FADE, _color );
			if( _selected )
				color = Color.White;
			else if( _hovered )
				color = _color;

			var facePoints = new List<PointF>();

			var minPoint = new PointF( 99999, 99999 );
			var maxPoint = new PointF( -99999, -99999 );

			using( var pen = new Pen( color ) )
			{
				if( !_selected && !_hovered )
				{
					pen.DashPattern = EditorColors.DASH_PATTERN;
				}
				
				var faces = _faces.Where( x => x.Plane.Normal.Dot( camera.Direction ) > 0 ).ToArray();
				foreach( var face in faces )
				{
					var projectedPoints = face.Vertices.Select( x => camera.ToLocal( camera.Project( x ).Inflate( Grid.Gap ).Deflate( Grid.Size ) ) ).Distinct().ToArray();

					var windingPoints = Extensions.WindingSort2D( projectedPoints.ToArray() );

					var pointCount = windingPoints.Length;
					if( pointCount > 0 )
					{
						// draw lines
						for( int i = 0; i < pointCount - 1; i++ )
						{
							g.DrawLine( pen, windingPoints[i], windingPoints[i + 1] );
						}
						g.DrawLine( pen, windingPoints[pointCount - 1], windingPoints[0] );

						// draw center cross
						var topleft = new PointF( projectedPoints.Min( x => x.X ), projectedPoints.Min( x => x.Y ) );
						var bottomright = new PointF( projectedPoints.Max( x => x.X ), projectedPoints.Max( x => x.Y ) );
						var bounds = new RectangleF( topleft.X, topleft.Y, bottomright.X - topleft.X, bottomright.Y - topleft.Y );

						var centerBounds = Extensions.FromPoints( windingPoints );
						var center = centerBounds.GetCenter();
						centerBounds = Extensions.FromPoint( center, 8 );

						if( centerBounds.Width < bounds.Width && centerBounds.Height < bounds.Height )
						{
							var prevStyle = pen.DashStyle;
							pen.DashStyle = DashStyle.Solid;

							g.DrawLine( pen, centerBounds.TopLeft(), centerBounds.BottomRight() );
							g.DrawLine( pen, centerBounds.BottomLeft(), centerBounds.TopRight() );

							pen.DashStyle = prevStyle;
						}

						if( _selected && EditorTool.Current == EditorTools.Vertex )
						{
							foreach( var vertex in projectedPoints )
							{
								var handle = Extensions.FromPoint( vertex, 8 );
								g.FillRectangle( EditorColors.BRUSH_HANDLE, handle );
							}
						}
					}

					facePoints.AddRange( projectedPoints );

					foreach( var v in face.Vertices )
					{
						var point = camera.Project( v );
						if( point.X < minPoint.X )
							minPoint.X = point.X;
						if( point.Y < minPoint.Y )
							minPoint.Y = point.Y;
						if( point.X > maxPoint.X )
							maxPoint.X = point.X;
						if( point.Y > maxPoint.Y )
							maxPoint.Y = point.Y;
					}
				}
			}

			if( _selected )
			{
				var topleft = new PointF( facePoints.Min( x => x.X ), facePoints.Min( x => x.Y ) );
				var bottomright = new PointF( facePoints.Max( x => x.X ), facePoints.Max( x => x.Y ) );
				var bounds = new RectangleF( topleft.X, topleft.Y, bottomright.X - topleft.X, bottomright.Y - topleft.Y );

				var handles = Extensions.GetHandles( bounds, 8 );
				var drawBounds = bounds.Downcast();

				if( EditorTool.Current == EditorTools.Select )
				{
					// draw handle outline
					g.DrawRectangle( EditorColors.PEN_DASH_FADED_HANDLE_OUTLINE, drawBounds );

					// draw handles
					foreach( var handle in handles )
						g.FillRectangle( EditorColors.BRUSH_HANDLE, handle );
				}

				// draw dimensions text
				var size = new PointF( maxPoint.X - minPoint.X, maxPoint.Y - minPoint.Y );

				var numberFormat = new NumberFormatInfo() { NumberDecimalSeparator = "." };
				var leftText = size.Y.ToString( "0.0", numberFormat );
				var topText = size.X.ToString( "0.0", numberFormat );
				
				var leftTextSize = g.MeasureString( leftText, EditorColors.SOLID_DIMENSIONS_FONT );
				var topTextSize = g.MeasureString( topText, EditorColors.SOLID_DIMENSIONS_FONT );

				var center = bounds.GetCenter();
				var leftPosition = new PointF( bounds.Left - leftTextSize.Width - DIMENSION_TEXT_OFFSET, center.Y - leftTextSize.Height/2 );
				var topPosition = new PointF( center.X - topTextSize.Width / 2, bounds.Top - DIMENSION_TEXT_OFFSET - topTextSize.Height );
				
				g.DrawString( leftText, EditorColors.SOLID_DIMENSIONS_FONT, EditorColors.BRUSH_WHITE, leftPosition );
				g.DrawString( topText, EditorColors.SOLID_DIMENSIONS_FONT, EditorColors.BRUSH_WHITE, topPosition );
			}
		}

		public void PaintOpaque3D()
		{
			var opaqueFaces = _faces.Where( x => TextureMap.GetBPP( x.PackName, x.TextureName ) <= 3 ).ToArray();
			Paint3D( opaqueFaces );
		}

		public void PaintTransparent3D()
		{
			var transparentFaces = _faces.Where( x => TextureMap.GetBPP( x.PackName, x.TextureName ) >= 4 ).ToArray();
			Paint3D( transparentFaces );
		}

		public void Paint3D( Face[] faces )
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
			foreach( var face in faces )
			{
				//cur++;
				//if( cur != 4 )
				//	continue;

				//if( face.Plane.Normal.Dot( new Triple( 0, -1, 0 ) ) != 1 )
				//	continue;

				var showLumels = EditorFlags.ShowLumels || ( EditorFlags.ShowLumelsOnFace && EditorTool.SelectedFace == face );
				if( showLumels )
				{
					if( face.Lumels.Count <= 0 )
						face.BuildLumels( this );
				}

				var otherPlanes = _faces.Where( x => x != face ).Select( x => x.Plane ).ToArray();

				var shade = 1.0f;
				if( face.Plane.Normal.Dot( new Triple( 1, 0, 0 ) ) < 0 ||
					face.Plane.Normal.Dot( new Triple( 0, 1, 0 ) ) < 0 ||
					face.Plane.Normal.Dot( new Triple( 0, 0, 1 ) ) < 0 )
				{
					shade = 0.75f;
				}

				if( string.IsNullOrEmpty( face.TextureName ) )
				{
					GL.SetTexture( 0 );
					GL.Color4f( red * shade, green * shade, blue * shade, alpha );
				}
				else
				{
					var textureID = TextureMap.GetID( face.PackName, face.TextureName );
					GL.SetTexture( textureID );
					GL.Color4f( shade, alpha );

					if( EditorTool.CurrentLightmapID > 0 && face.LightmapUVs.Count > 0 )
						GL.SetTexture( EditorTool.CurrentLightmapID );
				}

				if( face.Hovered || _hovered )
				{
					GL.Color4f( 1.0f, 0.5f, 0.5f, alpha );
				}
				else if( face.Selected )
				{
					GL.Color4f( 0.75f, 0.35f, 0.35f, alpha );
				}

				GL.BeginTriangles();

				var v0 = face.Vertices[0] / Grid.SIZE_BASE;
				var uv0 = face.UVs[0];
				if( EditorTool.CurrentLightmapID > 0 && face.LightmapUVs.Count > 0 )
					uv0 = face.LightmapUVs[0].Div( 128 );

				for( int i = 1; i < face.Vertices.Count - 1; i++ )
				{
					var v1 = face.Vertices[i] / Grid.SIZE_BASE;
					var v2 = face.Vertices[i + 1] / Grid.SIZE_BASE;

					var uv1 = face.UVs[i];
					var uv2 = face.UVs[i+1];

					if( EditorTool.CurrentLightmapID > 0 && face.LightmapUVs.Count > 0 )
					{
						uv1 = face.LightmapUVs[i].Div( 128 );
						uv2 = face.LightmapUVs[i + 1].Div( 128 );
					}

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
					
					GL.TexCoord2f( uv0.X, uv0.Y );
					GL.Vertex3f( v0.X, v0.Y, v0.Z );

					GL.TexCoord2f( uv1.X, uv1.Y );
					GL.Vertex3f( v1.X, v1.Y, v1.Z );

					GL.TexCoord2f( uv2.X, uv2.Y );
					GL.Vertex3f( v2.X, v2.Y, v2.Z );
				}

				GL.End();

				if( showLumels )
				{
					GL.SetTexture( 0 );
					GL.BeginLines();
					GL.Color4f( 0.0f, 1.0f, 0.0f, 1.0f );

					foreach( var lumel in face.Lumels )
					{
						var p0 = lumel.Position / Grid.SIZE_BASE;
						var p1 = p0 + face.Plane.Normal * 0.1f;

						GL.Vertex3f( p0.X, p0.Y, p0.Z );
						GL.Vertex3f( p1.X, p1.Y, p1.Z );
					}

					GL.End();
				}

				// draw vertex handles
				if( _selected && EditorTool.Current == EditorTools.Vertex )
				{
					GL.EnablePointSprite( true );
					GL.EnableDepthMask( false );
					GL.SetTexture( 0 );
					GL.PointSize( 8 );

					GL.BeginPoints();

					GL.Color4f( 1.0f );
					foreach( var v in face.Vertices )
						GL.Vertex3f( v.X / Grid.SIZE_BASE, v.Y / Grid.SIZE_BASE, v.Z / Grid.SIZE_BASE );

					GL.End();
					
					GL.EnableDepthMask( true );
					GL.EnablePointSprite( false );
				}
			}

			GL.SetTexture( 0 );
		}

		public GeometrySolid Copy()
		{
			var result = new GeometrySolid();

			foreach( var face in _faces )
				result.Faces.Add( face.Copy() );

			return result;
		}
    }
}
