using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Editor
{
	public static class Lightmap
	{
		private class ThreadData
		{
			public int first, last;
			//public List<GeometrySolid> solids;
			public Lumel[] allLumels;
			public Lumel[] normalLumels;
			public List<GeometrySolid> solids;
		}

		public const int SIZE = 128;
		public const float LUMEL_SIZE = 16.0f;
		public const float MAX_LIGHT_DISTANCE = 15.0f;

		public static void Generate( Level level, string filename )
		{
			float[,] map = new float[SIZE, SIZE];
			int[] rover = new int[SIZE];

			const float SKY_LIGHT_INTENSITY = 800.0f;

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
							lumel.Emission = SKY_LIGHT_INTENSITY;
							lumel.Excidence = lumel.Emission;
						}
					}
					else
					{
						foreach( var lumel in face.Lumels )
						{
							lumel.Emission = 0.0f;
							lumel.Excidence = lumel.Emission;
						}
					}
				}
			}

			var startTime = DateTime.Now;

			var allLumels = level.Solids.SelectMany( solid => solid.Faces.Where( face => face.PackName != "tools" || face.TextureName == "sky" ).SelectMany( x => x.Lumels ) ).ToArray();
			var normalLumels = level.Solids.SelectMany( solid => solid.Faces.Where( face => face.PackName != "tools" ).SelectMany( x => x.Lumels ) ).ToArray();

			const int THREAD_COUNT = 12;
			//var chunk = level.Solids.Count / THREAD_COUNT;
			var chunk = normalLumels.Length;
			if( chunk < 1 )
				chunk = 1;

			var first = 0;

			var threads = new List<Thread>();
			for( int i = 0; i < THREAD_COUNT; i++ )
			{
				var thread = new Thread( new ParameterizedThreadStart( BuildTransfersAsync ) );

				var last = first + chunk;
				if( i >= THREAD_COUNT-1 )
					last = normalLumels.Length;

				//var data = new ThreadData() { first = first, last = last, solids = level.Solids };
				var data = new ThreadData() { first = first, last = last, allLumels = allLumels, normalLumels = normalLumels, solids = level.Solids };
				thread.Start( data );

				threads.Add( thread );

				first += chunk;

				if( last >= level.Solids.Count )
					break;
			}

			foreach( var thread in threads )
				thread.Join();
			
			//BuildTransfers( level );
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

			const int ITERATIONS = 2;
			for( int iteration = 0; iteration < ITERATIONS; iteration++ )
			{
				foreach( var curSolid in level.Solids )
				{
					foreach( var curFace in curSolid.Faces )
					{
						if( curFace.PackName != "tools" )
						{
							foreach( var a in curFace.Lumels )
							{
								var incidence = 0.0f;
								var hits = 0;

								foreach( var b in a.Transfers )
								{
									var dir = b.Position - a.Position;
									var len = dir.Normalize() / Grid.SIZE_BASE;

									incidence += ( curFace.Plane.Normal.Dot( dir ) * b.Excidence ) / len;
									hits++;
								}

								incidence /= hits;
								if( incidence > SKY_LIGHT_INTENSITY )
									incidence = SKY_LIGHT_INTENSITY;

								a.Incidence = incidence;
							}

							if( iteration >= ITERATIONS - 1 )
							{
								var mapIndex = faceIndices[curFace];
								for( int y = 0; y < curFace.LumelHeight; y++ )
								{
									for( int x = 0; x < curFace.LumelWidth; x++ )
									{
										var lumelIndex = Extensions.IndexFromXY( x, y, curFace.LumelWidth );
										var lumel = curFace.Lumels[lumelIndex];

										//map[mapIndex.X + x, mapIndex.Y + y] = lumel.Incidence;
										var value = lumel.Incidence;
										if( value > SKY_LIGHT_INTENSITY )
											value = SKY_LIGHT_INTENSITY;
										else if( value < 0.01f )
											value = 0.01f;
										map[mapIndex.X + x + 1, mapIndex.Y + y + 1] = value;
									}
								}

								// blur data
								for( int y = 0; y < curFace.LumelHeight; y++ )
								{
									for( int x = 0; x < curFace.LumelWidth; x++ )
									{
										var sum = 0.0f;
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

										if( sum > 0 && hits > 1 )
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
									map[mapIndex.X + curFace.LumelWidth + 1, mapIndex.Y+y+1] = map[mapIndex.X + curFace.LumelWidth, mapIndex.Y+y + 1];
								}

								// top
								for( int x = 0; x < curFace.LumelWidth+2; x++ )
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
					}
				}

				foreach( var solid in level.Solids )
				{
					foreach( var face in solid.Faces )
					{
						foreach( var lumel in face.Lumels )
						{
							var I = lumel.Incidence;
							var R = lumel.Reflectiveness;
							var E = lumel.Emission;

							lumel.Excidence = ( I * R ) + E;
							if( lumel.Excidence > SKY_LIGHT_INTENSITY )
								lumel.Excidence = SKY_LIGHT_INTENSITY;
						}
					}
				}
			}

			endTime = DateTime.Now;

			var radianceTime = ( endTime - startTime ).TotalSeconds;

			startTime = DateTime.Now;

			var pixels = new byte[SIZE * SIZE * 3];
			for( int y = 0; y < SIZE; y++ )
			{
				for( int x = 0; x < SIZE; x++ )
				{
					var rad = map[x, y] * 255.0f;
					if( rad > 255.0f )
						rad = 255.0f;
					//byte r = (byte)( map[x, y] * 255.0f );
					byte r = (byte)rad;

					var index = ( ( SIZE - 1 - y ) * SIZE + x ) * 3;

					pixels[index] = r;
					pixels[index + 1] = r;
					pixels[index + 2] = r;
				}
			}

			var lightmap = new Targa()
			{
				Width = SIZE,
				Height = SIZE,
				Bpp = 3,
				Pixels = pixels
			};
			lightmap.Write( filename );

			if( EditorTool.CurrentLightmap != null )
				EditorTool.CurrentLightmap.Dispose();

			EditorTool.CurrentLightmap = lightmap;
			EditorTool.CurrentLightmapID = GL.UploadTexture( SIZE, SIZE, 3, pixels );

			endTime = DateTime.Now;

			var textureTime = ( endTime - startTime ).TotalSeconds;

			MessageBox.Show( "Transfer time: " + transferTime.ToString() + "s\nRadiance time: " + radianceTime.ToString() + "s\nTexture time: " + textureTime.ToString() + "s" );
		}

		/*private static void BuildTransfers( Level level )
		{
			var totalLumelCount = level.Solids.Sum( a => a.Faces.Sum( b => b.Lumels.Count ) );

			var direction = new Triple();
			for( int curSolid = 0; curSolid < level.Solids.Count; curSolid++ )
			{
				var solid = level.Solids[curSolid];
				for( int curFace = 0; curFace < solid.Faces.Count; curFace++ )
				{
					var face = solid.Faces[curFace];

					if( face.PackName != "tools" )
					{
						for( int curLumel = 0; curLumel < face.Lumels.Count; curLumel++ )
						{
							var a = face.Lumels[curLumel];
							a.Transfers.Clear();
							a.Transfers.Capacity = totalLumelCount / 2;
							
							for( int curOtherSolid = 0; curOtherSolid < level.Solids.Count; curOtherSolid++ )
							{
								if( curOtherSolid != curSolid )
								{
									var otherSolid = level.Solids[curOtherSolid];
									for( int curOtherFace = 0; curOtherFace < otherSolid.Faces.Count; curOtherFace++ )
									{
										var otherFace = otherSolid.Faces[curOtherFace];

										if( otherFace.PackName != "tools" || otherFace.TextureName == "sky" )
										{
											for( int curOtherLumel = 0; curOtherLumel < otherFace.Lumels.Count; curOtherLumel++ )
											{
												var b = otherFace.Lumels[curOtherLumel];

												//var direction = b.Position - a.Position;
												direction.X = b.Position.X - a.Position.X;
												direction.Y = b.Position.Y - a.Position.Y;
												direction.Z = b.Position.Z - a.Position.Z;

												direction.Normalize();

												var dirPlaneDot = face.Plane.Normal.Dot( direction );
												var dirOtherPlaneDot = otherFace.Plane.Normal.Dot( direction );

												if( dirPlaneDot > 0 && dirOtherPlaneDot < 0 )
												{
													if( Trace( level, a.Position, b.Position, solid ) )
													{
														a.Transfers.Add( b );
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}*/

		private static void BuildTransfersAsync( object args )
		{
			var data = args as ThreadData;

			var dir = new Triple();
			for( int i = data.first; i < data.last; i++ )
			{
				var a = data.normalLumels[i];

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

					var dirPlaneDot = a.Normal.Dot( dir );
					var dirOtherPlaneDot = b.Normal.Dot( dir );

					if( dirPlaneDot > 0 && dirOtherPlaneDot < 0 )
					{
						if( Trace( data.solids, a.Position, b.Position, a.Parent, b.Parent ) )
						{
							a.Transfers.Add( b );
						}
					}
				}
			}
		}
		
		/*private static void BuildTransfersAsync( object args )
		{
			var data = args as ThreadData;

			var dir = new Triple();
			for( int i = data.first; i < data.last; i++ )
			{
				var solid = data.solids[i];

				foreach( var face in solid.Faces )
				{
					if( face.PackName == "tools" )
						continue;

					foreach( var a in face.Lumels )
					{
						foreach( var otherSolid in data.solids )
						{
							if( otherSolid != solid )
							{
								foreach( var otherFace in otherSolid.Faces )
								{
									if( otherFace.PackName == "tools" && otherFace.TextureName != "sky" )
										continue;

									if( face.Plane.Normal.Dot( otherFace.Plane.Normal ) > 0.9f )
										continue;

									foreach( var b in otherFace.Lumels )
									{
										dir.X = b.Position.X - a.Position.X;
										dir.Y = b.Position.Y - a.Position.Y;
										dir.Z = b.Position.Z - a.Position.Z;
										var dist = dir.Normalize();

										dist /= Grid.SIZE_BASE;

										if( dist > MAX_LIGHT_DISTANCE )
											continue;

										var dirPlaneDot = face.Plane.Normal.Dot( dir );
										var dirOtherPlaneDot = otherFace.Plane.Normal.Dot( dir );

										if( dirPlaneDot > 0 && dirOtherPlaneDot < 0 )
										{
											if( Trace( data.solids, a.Position, b.Position, solid ) )
											{
												a.Transfers.Add( b );
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}*/

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

		/*private static bool Trace( Level level, Triple start, Triple end, GeometrySolid currentSolid )
		{
			_traces++;

			var ray = Ray.FromPoints( start, end );

			foreach( var solid in level.Solids )
			{
				if( solid != currentSolid )
				{
					foreach( var face in solid.Faces )
					{
						var length = 0.0f;
						if( ray.Intersect( face.Plane, ref length ) )
						{
							if( length < ray.Length )
							{
								var p = ray.Start + ray.Direction * length;
								var otherPlanes = solid.Faces.Where( x => x != face ).Select( x => x.Plane );

								var behindAll = true;
								foreach( var plane in otherPlanes )
								{
									if( plane.InFront( p ) )
										behindAll = false;
								}

								if( behindAll )
								{
									return false;
								}
							}
						}
					}
				}
			}

			return true;
		}*/

		/*private static bool Trace( List<GeometrySolid> solids, Triple start, Triple end, GeometrySolid currentSolid )
		{
			_traces++;

			var ray = Ray.FromPoints( start, end );

			foreach( var solid in solids )
			{
				if( solid != currentSolid )
				{
					foreach( var face in solid.Faces )
					{
						var length = 0.0f;
						if( ray.Intersect( face.Plane, ref length ) )
						{
							if( length < ray.Length )
							{
								var p = ray.Start + ray.Direction * length;
								//var otherPlanes = solid.Faces.Where( x => x != face ).Select( x => x.Plane );

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
							}
						}
					}
				}
			}

			return true;
		}*/

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
						}
					}
				}
			}

			return true;
		}
	}
}
