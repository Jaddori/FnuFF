using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.IO;

namespace Editor
{
	public static class Lightmap
	{
		public const int SIZE = 128;
		public const float LUMEL_SIZE = 16.0f;

		private static int _traces;

		public static int Traces => _traces;

		public static void Generate( Level level, string filename )
		{
			_traces = 0;

			float[,] map = new float[SIZE, SIZE];
			int[] rover = new int[SIZE];

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
							lumel.Radiance = 1.0f;
						}
					}
				}
			}

			BuildTransfers( level );

			var faceIndices = new Dictionary<Face, Point>();
			foreach( var solid in level.Solids )
			{
				foreach( var face in solid.Faces )
				{
					if( face.PackName == "tools" )
						continue;

					Point mapIndex;
					var allocated = AllocLightmap( rover, face.LumelWidth, face.LumelHeight, out mapIndex );
					if( !allocated )
						throw new OutOfMemoryException( "Lightmap texture is full." );

					faceIndices.Add( face, mapIndex );
					face.BuildLightmapUVs( mapIndex.X, mapIndex.Y );
				}
			}

			const int ITERATIONS = 9;
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
								var radiance = 0.0f;
								var hits = 0;

								foreach( var b in a.Transfers )
								{
									var dir = b.Position - a.Position;
									dir.Normalize();

									var addition = curFace.Plane.Normal.Dot( dir ) * b.Radiance;

									radiance += addition;
									hits++;
								}

								a.Radiance = radiance / hits;
								if( a.Radiance <= 0.0f )
									a.Radiance = 0.01f;
							}

							var mapIndex = faceIndices[curFace];
							for( int y = 0; y < curFace.LumelHeight; y++ )
							{
								for( int x = 0; x < curFace.LumelWidth; x++ )
								{
									var lumelIndex = Extensions.IndexFromXY( x, y, curFace.LumelWidth );
									var lumel = curFace.Lumels[lumelIndex];

									map[mapIndex.X + x, mapIndex.Y + y] = lumel.Radiance;
								}
							}
						}
					}
				}
			}

			// blur radiance data
			/*for( int y = 0; y < SIZE; y++ )
			{
				for( int x = 0; x < SIZE; x++ )
				{
					var rad = map[x, y];
					if( rad < 0.01f )
						continue;

					var sum = 0.0f;
					var hits = 1;
					for( int ny = -1; ny <= 1; ny++ )
					{
						for( int nx = -1; nx <= 1; nx++ )
						{
							if( x + nx < 0 || y + ny < 0 || x + nx >= SIZE || y + ny >= SIZE )
								continue;
							if( ny == 0 && nx == 0 )
								continue;

							var cur = map[x + nx, y + ny];
							if( cur >= 0.01f ) // make sure pixel is legal
							{
								sum += cur;
								hits++;
							}
						}
					}

					var final = ( rad + sum ) / hits;
					map[x, y] = final;
				}
			}*/

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
		}

		private static void BuildTransfers( Level level )
		{
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

												var direction = b.Position - a.Position;
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

		private static bool Trace( Level level, Triple start, Triple end, GeometrySolid currentSolid )
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
								var otherPlanes = solid.Faces.Where( x => x != face ).Select( x => x.Plane ).ToArray();

								var behindAll = true;
								for( int i = 0; i < otherPlanes.Length && behindAll; i++ )
								{
									if( otherPlanes[i].InFront( p ) )
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
		}
	}
}
