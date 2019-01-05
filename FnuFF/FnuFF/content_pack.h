#pragma once

#include "common.h"
#include "texture.h"

#define CONTENT_PACK_NAME_LEN 64
#define TEXTURE_NAME_LEN 64

class ContentPack
{
public:
	ContentPack();
	~ContentPack();

	bool load( const char* path );
	void unload();
	void upload();

	int getIndex( const char* name );

	const Rendering::Texture* getTexture( int index ) const;

private:
	typedef char name_t[TEXTURE_NAME_LEN];

	char name[CONTENT_PACK_NAME_LEN];
	int textureCount;
	name_t* names;
	Rendering::Texture* textures;
	bool uploaded;
};