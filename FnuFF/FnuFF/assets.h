#pragma once

#include "common.h"
#include "texture.h"
#include "mesh.h"
#include "font.h"
#include "asset_pack.h"

#define ASSETS_PATH_MAX_LEN 128
#define ASSETS_HOTLOAD_DELAY 1000

namespace Rendering
{
	/*class Assets
	{
	public:
		Assets();
		~Assets();

		void unload();
		void upload();

#if _DEBUG
		void hotload();
#endif

		int loadTexture( const char* path );
		int loadMesh( const char* path );
		int loadFont( const char* info, const char* texture );

		const Texture* getTexture( int index ) const;
		const Mesh* getMesh( int index ) const;
		const Font* getFont( int index ) const;

	private:
		uint64_t hashPath( const char* path );

		Array<uint64_t> textureHashes;
		Array<Texture> textures;
		bool dirtyTextures;

		Array<uint64_t> meshHashes;
		Array<Mesh> meshes;
		bool dirtyMeshes;

		Array<uint64_t> fontHashes;
		Array<Font> fonts;
		bool dirtyFonts;

#if _DEBUG
		uint64_t getTimestamp( const char* path );
		void reloadTexture( int index );
		void reloadMesh( int index );

		Array<char*> texturePaths;
		Array<char*> meshPaths;

		Array<uint64_t> textureTimestamps;
		Array<uint64_t> meshTimestamps;

		uint64_t lastHotload;
#endif
	};*/

	class Assets
	{
	public:
		Assets();
		~Assets();

		void unload();
		void upload();

		void loadPack( const char* path );
		int loadFont( const char* info, const char* texture );

		int getTextureIndex( const char* name, int len = 0 );

		const Texture* getTexture( int index ) const;
		const Font* getFont( int index ) const;

	private:
		uint64_t hashPath( const char* path );

		Array<AssetPack> packs;
		Array<const Texture*> textures;
		Array<const char*> textureNames;
		bool dirtyPacks;

		Array<uint64_t> fontHashes;
		Array<Font> fonts;
		bool dirtyFonts;
	};
}