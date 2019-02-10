using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Editor.Entities;

namespace Editor
{
	public static class Lightmap
	{
		private class ThreadData
		{
			public int first, last, current;
			public Lumel[] allLumels;
			public Lumel[] normalLumels;
			public List<GeometrySolid> solids;
		}

		public const int SIZE = 128;
		public const float LUMEL_SIZE = 16.0f;
		public const float MAX_LIGHT_DISTANCE = 15.0f;
		public static Triple DEFAULT_AMBIENT_LIGHT = new Triple( 0.05f );
		public static Triple DEFAULT_SKY_LIGHT = new Triple( 1.0f );

		private static List<ThreadData> _threadData = new List<ThreadData>();
		private static bool _done = false;
		private static byte[] _pixels;
		
		public static void Generate( Level level )
		{
			_done = false;

			Triple[,] map = new Triple[SIZE, SIZE];
			int[] rover = new int[SIZE];

			var ambientLight = DEFAULT_AMBIENT_LIGHT;
			var skyLight = DEFAULT_SKY_LIGHT;

			var worldLightEntity = level.Entities.FirstOrDefault( x => x.Data.GetType() == typeof( WorldLight ) );
			if( worldLightEntity != null )
			{
				var data = worldLightEntity.Data as WorldLight;
				ambientLight = new Triple( data.AmbientColor.R, data.AmbientColor.G, data.AmbientColor.B ) * data.AmbientIntensity;
				skyLight = new Triple( data.SkyColor.R, data.SkyColor.G, data.SkyColor.B ) * data.SkyIntensity;

				ambientLight /= 255.0f;
				skyLight /= 255.0f;
			}

			// initialize lumels
			foreach( var solid in level.Solids )
			{
				foreach( var face in solid.Faces )
				{
					face.BuildLumels( solid );

					if( face.TextureName == "sky" )
					{
						foreach( var lumel in face.Lumels )
						{
							//lumel.Emission = new Triple( SKY_LIGHT_INTENSITY );
							lumel.Emission = skyLight;
						}
					}
					else
					{
						foreach( var lumel in face.Lumels )
						{
							lumel.Emission = Triple.Zero;
						}
					}
				}
			}

			var startTime = DateTime.Now;
			
			var normalLumels = level.Solids.SelectMany( solid => solid.Faces.Where( face => face.PackName != "tools" ).SelectMany( x => x.Lumels ) ).ToArray();

			const int THREAD_COUNT = 2;
			var chunk = normalLumels.Length / THREAD_COUNT;
			if( chunk < 1 )
				chunk = 1;

			var first = 0;

			_threadData.Clear();
			var endTime = DateTime.Now;

			var transferTime = ( endTime - startTime ).TotalSeconds;

			startTime = DateTime.Now;
			var faceIndices = new Dictionary<Face, Point>();
			foreach( var solid in level.Solids )
			{
				foreach( var face in solid.Faces )
				{
					if( face.PackName == "tools" )
						continue;

					Point mapIndex;
					var allocated = AllocLightmap( rover, face.LumelWidth+2, face.LumelHeight+2, out mapIndex );
					if( !allocated )
						throw new OutOfMemoryException( "Lightmap texture is full." );

					faceIndices.Add( face, mapIndex );
					face.BuildLightmapUVs( mapIndex.X+1, mapIndex.Y+1 );
				}
			}

			var allLumels = level.Solids.SelectMany( solid => solid.Faces.Where( face => face.PackName != "tools" || face.TextureName == "sky" ).SelectMany( face => face.Lumels ) ).ToArray();

			foreach( var l in allLumels )
			{
				for( int i = 0; i < level.Solids.Count && !l.Blocked; i++ )
				{
					var solid = level.Solids[i];
					if( l.Parent == solid )
						continue;

					var behind = true;
					for( int j = 0; j < solid.Faces.Count && behind; j++ )
					{
						if( solid.Faces[j].Plane.InFront( l.Position ) )
							behind = false;
					}

					if( behind )
						l.Blocked = true;
				}

				l.Incidence = ambientLight;
			}

			allLumels = allLumels.Where( lumel => !lumel.Blocked ).ToArray();

			var emissiveLumels = allLumels.Where( x => x.Emission.Length() > 0.0f );
			var dimLumels = allLumels.Where( x => x.Emission.Length() <= 0.0f );

			foreach( var a in emissiveLumels )
			{
				foreach( var b in dimLumels )
				{
					var infront = b.Position.Dot( a.Normal ) > a.Position.Dot( a.Normal );
					if( infront )
					{
						var dir = a.Position - b.Position;
						var dist = dir.Normalize() / Grid.SIZE_BASE;

						if( dist > MAX_LIGHT_DISTANCE )
							continue;

						var dot = dir.Dot( b.Normal );
						if( dot > 0 )
						{
							if( Trace( level.Solids, a.Position, b.Position, a.Parent, b.Parent ) )
							{
								b.Incidence += a.Emission * (dot*dot)/(dist*dist);
							}
						}
					}
				}
			}

			var allLights = level.Entities.Where( x => x.Data.GetType() == typeof( PointLight ) ).ToArray();
			foreach( var a in allLights )
			{
				var light = a.Data as PointLight;
				foreach( var b in dimLumels )
				{
					var dir = a.Position - b.Position;
					var dist = dir.Normalize() / Grid.SIZE_BASE;

					if( dist > MAX_LIGHT_DISTANCE )
						continue;

					var dot = dir.Dot( b.Normal );
					if( dot > 0 )
					{
						if( Trace( level.Solids, a.Position, b.Position, b.Parent ) )
						{
							var color = new Triple( light.Color.R / 255.0f, light.Color.G / 255.0f, light.Color.B / 255.0f );
							b.Incidence += ( color * light.Intensity ) * ( dot * dot ) / ( dist * dist );
						}
					}
				}
			}

			foreach( var solid in level.Solids )
			{
				foreach( var curFace in solid.Faces )
				{
					if( curFace.PackName == "tools" )
						continue;

					var mapIndex = faceIndices[curFace];
					for( int y = 0; y < curFace.LumelHeight; y++ )
					{
						for( int x = 0; x < curFace.LumelWidth; x++ )
						{
							var lumelIndex = Extensions.IndexFromXY( x, y, curFace.LumelWidth );
							var lumel = curFace.Lumels[lumelIndex];

							var value = lumel.Incidence;
							if( value.Length() > 1.0f )
								value.Normalize();

							map[mapIndex.X + x + 1, mapIndex.Y + y + 1] = value;
						}
					}

					// blur data
					for( int y = 0; y < curFace.LumelHeight; y++ )
					{
						for( int x = 0; x < curFace.LumelWidth; x++ )
						{
							var sum = Triple.Zero;
							var hits = 0;

							for( int ny = -1; ny <= 1; ny++ )
							{
								for( int nx = -1; nx <= 1; nx++ )
								{
									if( x + nx < 0 )
										continue;
									if( y + ny < 0 )
										continue;
									if( x + nx >= curFace.LumelWidth )
										continue;
									if( y + ny >= curFace.LumelHeight )
										continue;
									if( nx == 0 && ny == 0 )
										continue;

									var neighbour = map[mapIndex.X + x + nx + 1, mapIndex.Y + y + ny + 1];
									sum += neighbour;
									hits++;
								}
							}

							if( sum.Length() > 0 && hits > 1 )
							{
								var average = sum / hits;
								map[mapIndex.X + x + 1, mapIndex.Y + y + 1] = average;
							}
						}
					}

					// left
					for( int y = 0; y < curFace.LumelHeight; y++ )
					{
						map[mapIndex.X, mapIndex.Y + y + 1] = map[mapIndex.X + 1, mapIndex.Y + y + 1];
					}

					// right
					for( int y = 0; y < curFace.LumelHeight; y++ )
					{
						map[mapIndex.X + curFace.LumelWidth + 1, mapIndex.Y + y + 1] = map[mapIndex.X + curFace.LumelWidth, mapIndex.Y + y + 1];
					}

					// top
					for( int x = 0; x < curFace.LumelWidth + 2; x++ )
					{
						map[mapIndex.X + x, mapIndex.Y] = map[mapIndex.X + x, mapIndex.Y + 1];
					}

					// bottom
					for( int x = 0; x < curFace.LumelWidth + 2; x++ )
					{
						map[mapIndex.X + x, mapIndex.Y + curFace.LumelHeight + 1] = map[mapIndex.X + x, mapIndex.Y + curFace.LumelHeight];
					}
				}
			}

			endTime = DateTime.Now;

			var radianceTime = ( endTime - startTime ).TotalSeconds;

			startTime = DateTime.Now;

			_pixels = new byte[SIZE * SIZE * 3];
			for( int y = 0; y < SIZE; y++ )
			{
				for( int x = 0; x < SIZE; x++ )
				{
					var color = map[x, y];
					color *= 255.0f;
					if( color.X > 255.0f )
						color.X = 255.0f;
					if( color.Y > 255.0f )
						color.Y = 255.0f;
					if( color.Z > 255.0f )
						color.Z = 255.0f;

					var r = (byte)color.X;
					var g = (byte)color.Y;
					var b = (byte)color.Z;

					var index = ( ( SIZE - 1 - y ) * SIZE + x ) * 3;

					_pixels[index] = b;
					_pixels[index + 1] = g;
					_pixels[index + 2] = r;
				}
			}

			endTime = DateTime.Now;

			var textureTime = ( endTime - startTime ).TotalSeconds;

			_done = true;

			var traces = level.Solids.Sum( solid => solid.Faces.Sum( face => face.Lumels.Sum( lumel => lumel.Traces ) ) );
			var averageTraces = traces / level.Solids.Sum( solid => solid.Faces.Sum( face => face.Lumels.Count ) );

			MessageBox.Show( "Transfer time: " + transferTime.ToString() + "s\nRadiance time: " + radianceTime.ToString() + "s\nTexture time: " + textureTime.ToString() + "s\nTraces: " + traces.ToString() + "\nAverage traces: " + averageTraces.ToString() );
		}

		private static void BuildTransfersAsync( object args )
		{
			var data = args as ThreadData;

			var dir = new Triple();
			for( int i = data.first; i < data.last; i++ )
			{
				var a = data.normalLumels[i];
				a.Traces = 0;

				foreach( var b in data.allLumels )
				{
					if( a == b )
						continue;

					dir.X = b.Position.X - a.Position.X;
					dir.Y = b.Position.Y - a.Position.Y;
					dir.Z = b.Position.Z - a.Position.Z;
					var dist = dir.Normalize();

					dist /= Grid.SIZE_BASE;

					if( dist > MAX_LIGHT_DISTANCE )
						continue;
					
					if( a.Normal.Dot( b.Normal ) < 0 )
					{
						a.Traces++;
						if( Trace( data.solids, a.Position, b.Position, a.Parent, b.Parent ) )
						{
							a.Transfers.Add( b );
						}
					}
				}

				data.current = i;
			}
		}
		
		private static bool AllocLightmap( int[] rover, int width, int height, out Point index )
		{
			index = new Point( -1, -1 );
			int bestHeight = SIZE, tentativeHeight = 0;

			for( int i = 0; i < SIZE - width; i++ )
			{
				tentativeHeight = 0;

				var validSpot = true;
				for( int j = 0; j < width && validSpot; j++ )
				{
					if( rover[i + j] >= bestHeight )
						validSpot = false;
					if( rover[i + j] > tentativeHeight )
						tentativeHeight = rover[i + j];
				}

				if( validSpot )
				{
					bestHeight = tentativeHeight;
					index.X = i;
					index.Y = tentativeHeight;
				}
			}

			if( bestHeight + height > SIZE )
				return false;

			for( int i = 0; i < width; i++ )
				rover[index.X + i] = bestHeight + height;

			return true;
		}
		
		private static bool Trace( List<GeometrySolid> solids, Triple start, Triple end, params GeometrySolid[] ignoreSolids )
		{
			var ray = Ray.FromPoints( start, end );

			foreach( var solid in solids )
			{
				if( ignoreSolids.Contains( solid ) )
					continue;

				foreach( var face in solid.Faces )
				{
					var length = 0.0f;
					if( ray.Intersect( face.Plane, ref length ) )
					{
						if( length < ray.Length )
						{
							var p = ray.Start + ray.Direction * length;

#if true
							var behindAll = true;
							foreach( var otherFace in solid.Faces )
							{
								if( otherFace == face )
									continue;

								if( otherFace.Plane.InFront( p ) )
									behindAll = false;
							}

							if( behindAll )
							{
								return false;
							}
#else
							var x = p.Dot( face.Tangent );
							var y = p.Dot( face.Bitangent );

							if( x >= 0 && x <= face.Width && y >= 0 && y <= face.Height )
								return false;
#endif
						}
					}
				}
			}

			return true;
		}

		public static void PollProgress( out int completed, out int total, out bool done )
		{
			completed = 0;
			total = 0;

			foreach( var data in _threadData )
			{
				completed += data.current - data.first;
				total += data.last - data.first;
			}

			done = _done;
		}

		public static void Upload( string filename )
		{
			var lightmap = new Targa()
			{
				Width = SIZE,
				Height = SIZE,
				Bpp = 3,
				Pixels = _pixels
			};
			lightmap.Write( filename );

			if( EditorTool.CurrentLightmap != null )
				EditorTool.CurrentLightmap.Dispose();

			EditorTool.CurrentLightmap = lightmap;
			EditorTool.CurrentLightmapID = GL.UploadTexture( SIZE, SIZE, 3, _pixels );
		}
	}
}
