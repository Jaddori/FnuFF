#pragma once

#include "common.h"
#include "mesh.h"
#include "shader.h"
#include "camera.h"
#include "texture.h"
#include "assets.h"
#include "transform.h"
#include "font.h"
#include "billboard.h"
#include "asset_pack.h"
#include "solid.h"

namespace Rendering
{
	struct Glyph
	{
		glm::vec3 position;
		glm::vec4 uv;
		glm::vec2 size;
		glm::vec4 color;
	};

	struct GlyphCollection
	{
		const Texture* texture;
		Array<Glyph> glyphs[2];
	};

	struct Quad
	{
		glm::vec3 position;
		glm::vec2 size;
		glm::vec2 uvStart, uvEnd;
		glm::vec4 color;
	};

	struct QuadCollection
	{
		const Texture* texture;
		Array<Quad> quads[2];
	};

	struct BillboardCollection
	{
		const Texture* diffuseMap;
		const Texture* maskMap;
		Array<Billboard> billboards[2];
	};

	class Graphics
	{
	public:
		Graphics();
		~Graphics();

		void load();

		void finalize();
		void render( float deltaTime );

		void queueSolid( const Solid* solid );
		void queueQuad( int textureIndex, const glm::vec3& position, const glm::vec2& size, const glm::vec2& uvStart, const glm::vec2& uvEnd, const glm::vec4& color );
		void queueText( int fontIndex, const char* text, const glm::vec3& position, const glm::vec4& color );
		void queueBillboard( int diffuseIndex, int maskIndex, const glm::vec3& position, const glm::vec2& size, const glm::vec4& uv, bool spherical, const glm::vec3& scroll );

		Camera* getPerspectiveCamera();
		Camera* getOrthographicCamera();
		Assets* getAssets();

	private:
		void renderSolids();
		void renderBillboards();
		void renderQuads();
		void renderText();

		Camera perspectiveCamera;
		Assets assets;

		Shader solidShader;
		GLuint solidShaderProjectionLocation;
		GLuint solidShaderViewLocation;
		GLuint solidShaderDiffuseLocation;
		SwapArray<const Solid*> solidQueue;
		Texture lightmap;
		GLuint lightmapLocation;

		Shader textShader;
		GLuint textProjectionLocation;
		Camera orthographicCamera;
		GLuint textVAO;
		GLuint textVBO;
		Array<GlyphCollection> glyphCollections;

		Shader quadShader;
		GLuint quadProjectionLocation;
		GLuint quadVAO;
		GLuint quadVBO;
		Array<QuadCollection> quadCollections;
		Array<QuadCollection> transparentQuadCollections;

		Shader billboardShader;
		GLint billboardProjectionLocation;
		GLint billboardViewLocation;
		GLint billboardDeltaTimeLocation;
		GLint billboardDiffuseLocation;
		GLint billboardMaskLocation;
		GLuint billboardVAO;
		GLuint billboardVBO;
		Array<BillboardCollection> billboardCollections;

		int writeIndex, readIndex;
		float elapsedTime;
	};
}