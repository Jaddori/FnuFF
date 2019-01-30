#include <stdio.h>
#include <string>
#include "glm\glm.hpp"
#include "glm\gtc\type_ptr.hpp"

struct plane_t
{
	glm::vec3 normal;
	float d;
};

struct vertex_t
{
	glm::vec3 position;
	glm::vec2 uv;
};

struct face_t;
struct lumel_t
{
	glm::vec3 position;
	glm::vec3 normal;
	face_t* parent;
};
lumel_t** g_all_lumels;
int g_total_lumels = 0;

struct solid_t;
struct face_t
{
	plane_t plane;

	int vertexCount;
	vertex_t* vertices;

	int lumelWidth, lumelHeight, lumelSize, lumelCount;
	lumel_t* lumels;

	int textureIndex;

	solid_t* parent;
};

struct solid_t
{
	int planeCount;
	plane_t* planes;

	int faceCount;
	face_t* faces;
};

#pragma pack(push, 1)
struct tga_header_t
{
	int8_t idLength;
	int8_t colormapType;
	int8_t imageType;
	int16_t colormapOrigin;
	int16_t colormapLength;
	int8_t colormapDepth;
	int16_t xorigin;
	int16_t yorigin;
	int16_t width;
	int16_t height;
	int8_t bpp;
	int8_t imageDescriptor;
};
#pragma pack(pop)

struct tga_t
{
	tga_header_t header;
	char* pixels;
};

struct level_t
{
	int solidCount;
	solid_t* solids;

	int textureCount;
	tga_t* textures;
};
level_t g_level = {};

inline char read_byte( FILE* file )
{
	char b;
	fread( &b, sizeof(b), 1, file );
	return b;
}

inline void read_bytes( FILE* file, char* dst, int n )
{
	fread( dst, sizeof( char ), n, file );
}

inline int read_int( FILE* file )
{
	int i;
	fread( &i, sizeof( i ), 1, file );
	return i;
}

inline void read_ints( FILE* file, int* dst, int n )
{
	fread( dst, sizeof( int ), n, file );
}

inline float read_float( FILE* file )
{
	float f;
	fread( &f, sizeof( f ), 1, file );
	return f;
}

inline float read_floats( FILE* file, float* dst, int n )
{
	fread( dst, sizeof( float ), n, file );
}

void load_face( face_t* face, FILE* file )
{
	fread( &face->plane, sizeof( face->plane ), 1, file );

	int vertexCount = read_int( file );
	face->vertexCount = vertexCount;
	face->vertices = new vertex_t[vertexCount];
	fread( face->vertices, sizeof( vertex_t ), vertexCount, file );

	fread( &face->lumelWidth, sizeof( int ), 4, file );
	int lumelCount = face->lumelCount;
	face->lumels = new lumel_t[lumelCount];

	g_total_lumels += lumelCount;

	for( int i = 0; i < lumelCount; i++ )
	{
		fread( face->lumels + i, sizeof( lumel_t ) - sizeof( solid_t* ), 1, file );
		face->lumels[i].parent = face;
	}

	int textureIndex = read_int( file );
	face->textureIndex = textureIndex;
}

void load_solid( solid_t* solid, FILE* file )
{
	int planeCount = read_int( file );
	solid->planeCount = planeCount;
	solid->planes = new plane_t[planeCount];
	fread( solid->planes, sizeof( plane_t ), planeCount, file );

	int faceCount = read_int( file );
	solid->faceCount = faceCount;
	solid->faces = new face_t[faceCount];

	for( int i = 0; i < faceCount; i++ )
	{
		load_face( solid->faces + i, file );
		solid->faces[i].parent = solid;
	}
}

bool load_texture( tga_t* texture, FILE* file )
{
	fread( &texture->header, sizeof( texture->header ), 1, file );

	tga_header_t& header = texture->header;

	if( header.width <= 0 || header.height <= 0 || header.bpp < 24 )
		return false;

	int bpp = header.bpp / 8;
	int size = header.width * header.height * bpp;
	texture->pixels = new char[size];

	if( header.imageType == 2 ) // uncompressed RGB(A)
	{
		fread( texture->pixels, sizeof( char ), size, file );
	}
	else // RLE compressed RGB(A)
	{
		char pixelBuffer[4] = {};

		int offset = 0;
		while( offset < size )
		{
			char packet = 0;
			fread( &packet, sizeof( packet ), 1, file );

			if( ( packet & 0x80 ) ) // RLE)
			{
				packet ^= 0x80;
				int count = packet + 1;
				fread( pixelBuffer, 1, bpp, file );

				for( int i = 0; i < count; i++ )
				{
					memcpy( texture->pixels + offset, pixelBuffer, bpp );
					offset += bpp;
				}
			}
			else // raw pixels
			{
				int count = packet + 1;
				fread( texture->pixels + offset, 1, count*bpp, file );

				offset += count * bpp;
			}
		}
	}

	// flip vertically?
	if( header.yorigin == 0 )
	{
		int lineSize = header.width * bpp;
		char* buffer = new char[lineSize];

		for( int y = 0; y < header.height / 2; y++ )
		{
			char* top = texture->pixels + y * lineSize;
			char* bottom = texture->pixels + ( header.height - y - 1 ) * lineSize;

			memcpy( buffer, top, lineSize );
			memcpy( top, bottom, lineSize );
			memcpy( bottom, buffer, lineSize );
		}

		delete[] buffer;
	}

	return true;
}

bool load_map( const char* path )
{
	FILE* file = fopen( path, "rb" );

	if( !file )
		return false;

	// read solids
	int solidCount = read_int( file );
	g_level.solidCount = solidCount;
	g_level.solids = new solid_t[solidCount];

	for( int i = 0; i < solidCount; i++ )
	{
		load_solid( g_level.solids + i, file );
	}

	// read entities

	// read textures
	int textureCount = read_int( file );
	g_level.textureCount = textureCount;
	g_level.textures = new tga_t[g_level.textureCount];

	for( int i = 0; i < textureCount; i++ )
	{
		load_texture( g_level.textures+i, file );
	}

	fclose( file );

	return true;
}

struct transfer_t
{
	lumel_t* a;
	lumel_t** b;
	int count;
};
transfer_t* g_transfers;
int g_transfer_count = 0;

#define MAX_LIGHT_DISTANCE 15
float EPSILON = glm::epsilon<float>();

float plane_intersect( const glm::vec3& start, const glm::vec3& direction, const plane_t& plane )
{
	float denom = glm::dot( plane.normal, direction );
	if( fabs( denom ) > EPSILON )
	{
		glm::vec3 center = plane.normal * plane.d;
		float t = glm::dot( center - start, plane.normal ) / denom;
		if( t >= -0.0f )
		{
			return t;
		}
	}
	return -1.0f;
}

bool plane_behind( const glm::vec3& point, const plane_t& plane )
{
	float normalDistance = glm::dot( plane.normal, point );
	bool greater = ( normalDistance - 0.0001f > plane.d );
	return greater;
}

bool trace( int from, int to )
{
	lumel_t* a = g_all_lumels[from];
	lumel_t* b = g_all_lumels[to];

	if( glm::dot( a->normal, b->normal ) < 0.9f )
	{
		glm::vec3 p0 = a->position;
		glm::vec3 p1 = b->position;

		if( glm::dot( p1, a->normal ) > glm::dot( p0, a->normal ) )
		{
			glm::vec3 dir = glm::normalize( p0 - p1 );
			float dist = glm::distance( p0, p1 );

			if( dist < MAX_LIGHT_DISTANCE )
			{
				for( int i = 0; i < g_level.solidCount; i++ )
				{
					solid_t* solid = g_level.solids+i;

					if( solid == a->parent->parent ||
						solid == b->parent->parent )
						continue;

					for( int j = 0; j < solid->planeCount; j++ )
					{
						float len = plane_intersect( p0, dir, solid->planes[j] );
						if( len < dist )
						{
							glm::vec3 p2 = p0 + dir * len;

							bool behindAll = true;
							for( int k = 0; k < solid->planeCount && behindAll; k++ )
							{
								if( j == k )
									continue;

								if( !plane_behind( p2, solid->planes[k] ) )
								{
									behindAll = false;
								}
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
	}

	return true;
}

void build_transfers()
{
	g_all_lumels = new lumel_t*[g_total_lumels];
	int index = 0;

	for( int i = 0; i < g_level.solidCount; i++ )
	{
		solid_t& solid = g_level.solids[i];
		for( int j = 0; j < solid.faceCount; j++ )
		{
			face_t& face = solid.faces[j];
			for( int k = 0; k < face.lumelCount; k++ )
			{
				g_all_lumels[index] = &face.lumels[k];
				index++;
			}
		}
	}

	g_transfers = new transfer_t[g_total_lumels];
	g_transfer_count = g_total_lumels;

	lumel_t** buffer = new lumel_t*[g_total_lumels];

	for( int i = 0; i < g_total_lumels; i++ )
	{
		g_transfers[i].a = g_all_lumels[i];

		int bufferIndex = 0;
		for( int j = 0; j < g_total_lumels; j++ )
		{
			if( i == j )
				continue;
			if( g_all_lumels[i]->parent == g_all_lumels[j]->parent )
				continue;

			if( trace( i, j ) )
			{
				buffer[bufferIndex] = g_all_lumels[j];
				bufferIndex++;
			}
		}

		if( bufferIndex > 0 )
		{
			g_transfers[i].b = new lumel_t*[bufferIndex];
			memcpy( g_transfers[i].b, buffer, sizeof( lumel_t* )*bufferIndex );
		}

		g_transfers[i].count = bufferIndex;
	}

	delete[] buffer;

	delete[] g_all_lumels;
}

bool rad_world( int threads, int bounces )
{
	build_transfers();

	return true;
}

void fullbright_world()
{

}

int main( int argc, char* argv[] )
{
	const char* path = argv[1];
	int threads = 1, bounces = 1;
	bool fullbright = false, verbose = false;

	for( int i = 2; i < argc; i++ )
	{
		if( strncmp( argv[i], "-threads", 8 ) == 0 )
		{
			i++;
			threads = atoi( argv[i] );
		}
		else if( strncmp( argv[i], "-bounces", 8 ) == 0 )
		{
			i++;
			bounces = atoi( argv[i] );
		}
		else if( strncmp( argv[i], "-fullbright", 11 ) == 0 )
		{
			fullbright = true;
		}
		else if( strncmp( argv[i], "-verbose", 8 ) == 0 )
		{
			verbose = true;
		}
	}

	if( !load_map( path ) )
		return 1;

	if( fullbright )
	{
		fullbright_world();
	}
	else
	{
		if( !rad_world( threads, bounces ) )
			return 1;
	}

	return 0;
}