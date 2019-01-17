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

		public static void Generate( Level level, string filename )
		{
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

			for( int curSolid = 0; curSolid < level.Solids.Count; curSolid++ )
			{
				var solid = level.Solids[curSolid];
				for( int curFace = 0; curFace < solid.Faces.Count; curFace++ )
				{
					var face = solid.Faces[curFace];

					if( face.PackName != "tools" )
					{
						face.GOOD_LINES.Clear();
						face.BAD_LINES.Clear();

						Point mapIndex;
						var allocated = AllocLightmap( rover, face.LumelWidth, face.LumelHeight, out mapIndex );
						if( allocated )
						{
							face.BuildLightmapUVs( mapIndex.X, mapIndex.Y );

							for( int curLumel = 0; curLumel < face.Lumels.Count; curLumel++ )
							{
								var a = face.Lumels[curLumel];

								var hits = 0;
								var radiance = 0.0f;

								for( int curOtherSolid = 0; curOtherSolid < level.Solids.Count; curOtherSolid++ )
								{
									if( curOtherSolid != curSolid )
									{
										var otherSolid = level.Solids[curOtherSolid];
										for( int curOtherFace = 0; curOtherFace < otherSolid.Faces.Count; curOtherFace++ )
										{
											var otherFace = otherSolid.Faces[curOtherFace];

											//if( otherFace.Plane.Normal.Dot( face.Plane.Normal ) < 0 && otherFace.TextureName == "sky" )
											{
												for( int curOtherLumel = 0; curOtherLumel < otherFace.Lumels.Count; curOtherLumel++ )
												{
													var b = otherFace.Lumels[curOtherLumel];

													var direction = b.Position - a.Position;
													direction.Normalize();

													if( face.Plane.Normal.Dot( direction ) > 0 && otherFace.TextureName == "sky" )
													{
														if( Trace( level, a.Position, b.Position, solid ) )
														{
															radiance += b.Radiance;
															hits++;

															//face.GOOD_LINES.Add( new Tuple<Triple, Triple>( a.Position, b.Position ) );
														}
														else
														{
															//face.BAD_LINES.Add( new Tuple<Triple, Triple>( a.Position, end ) );
														}
													}
												}
											}
										}
									}
								}

								//a.Radiance = radiance / hits;
								a.Radiance = radiance;
							}
							
							for( int y = 0; y < face.LumelHeight; y++ )
							{
								for( int x = 0; x < face.LumelWidth; x++ )
								{
									var lumelIndex = Extensions.IndexFromXY( x, y, face.LumelWidth );
									var lumel = face.Lumels[lumelIndex];

									map[mapIndex.X + x, mapIndex.Y + y] = lumel.Radiance;
								}
							}
						}
					}
				}
			}

			var stream = new FileStream( filename, FileMode.Create, FileAccess.Write );
			var writer = new BinaryWriter( stream );

			byte idlen = 0;
			byte colormapType = 0;
			byte imageType = 2;
			Int16 origin = 0;
			Int16 length = 0;
			byte depth = 0;
			Int16 xorigin = 0;
			Int16 yorigin = 0;
			Int16 width = SIZE;
			Int16 height = SIZE;
			byte bpp = 24;
			byte imageDesc = 0;

			writer.Write( idlen );
			writer.Write( colormapType );
			writer.Write( imageType );
			writer.Write( origin );
			writer.Write( length );
			writer.Write( depth );
			writer.Write( xorigin );
			writer.Write( yorigin );
			writer.Write( width );
			writer.Write( height );
			writer.Write( bpp );
			writer.Write( imageDesc );

			for( int y = SIZE - 1; y >= 0; y-- )
			{
				for( int x = 0; x < SIZE; x++ )
				{
					var radiance = map[x, y] * 255.0f;
					if( radiance > 255.0f )
						radiance = 255.0f;
					byte r = (byte)radiance;

					writer.Write( r );
					writer.Write( r );
					writer.Write( r );
				}
			}

			writer.Close();
			stream.Close();

			if( EditorTool.CurrentLightmap != null )
				EditorTool.CurrentLightmap.Dispose();

			EditorTool.CurrentLightmap = new Targa();
			EditorTool.CurrentLightmap.Load( filename );
			EditorTool.CurrentLightmapID = GL.LoadTexture( filename );
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
