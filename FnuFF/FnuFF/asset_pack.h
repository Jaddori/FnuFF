#pragma once

#include "common.h"
#include "texture.h"

#define ASSET_PACK_NAME_LEN 64

typedef char name_t[ASSET_PACK_NAME_LEN];

class AssetPack
{
public:
	AssetPack();
	~AssetPack();

	bool load( const char* path );
	void unload();
	void upload();

	int getIndex( const char* name );

	int getTextureCount() const;
	const Rendering::Texture* getTexture( int index ) const;
	const char* getName( int index ) const;

private:
	char name[ASSET_PACK_NAME_LEN];
	int textureCount;
	name_t* names;
	Rendering::Texture* textures;
	bool uploaded;
};