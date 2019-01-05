#pragma once

#include "common.h"
#include "mesh.h"
#include "shader.h"
#include "camera.h"
#include "texture.h"
#include "instanced_model.h"
#include "assets.h"
#include "transform.h"
#include "font.h"
#include "gbuffer.h"
#include "billboard.h"
#include "asset_pack.h"

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
		const Texture* normalMap;
		const Texture* specularMap;
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

		void queueVao( GLuint vao, int vertexCount, int textureIndex );
		void queueMesh( int meshIndex, Transform* transform );
		void queueQuad( int textureIndex, const glm::vec3& position, const glm::vec2& size, const glm::vec2& uvStart, const glm::vec2& uvEnd, const glm::vec4& color );
		void queueText( int fontIndex, const char* text, const glm::vec3& position, const glm::vec4& color );
		void queueBillboard( int diffuseIndex, int normalIndex, int specularIndex, int maskIndex, const glm::vec3& position, const glm::vec2& size, const glm::vec4& uv, bool spherical, const glm::vec3& scroll );
		void queueDirectionalLight( const glm::vec3& direction, const glm::vec3& color, float intensity );
		void queuePointLight( const glm::vec3& position, const glm::vec3& color, float intensity, float linear, float constant, float exponent );

		void setLightingEnabled( bool enabled );

		//Camera* getCamera();
		Camera* getPerspectiveCamera();
		Camera* getOrthographicCamera();
		Assets* getAssets();
		Gbuffer* getGbuffer();
		bool getLightingEnabled();

	private:
		void renderDeferred( float deltaTime );
		void renderForward();
		void renderBasic();

		Gbuffer gbuffer;

		Camera perspectiveCamera;
		Shader shader;
		Texture texture;
		const Texture* normalMap;
		const Texture* specularMap;

		GLuint projectionLocation;
		GLuint viewLocation;

		Assets assets;

		Shader basicShader;
		GLuint basicShaderProjectionLocation;
		GLuint basicShaderViewLocation;
		SwapArray<GLuint> vaoQueue;
		SwapArray<int> vaoCountQueue;
		SwapArray<int> textureIndices;

		Array<int> meshQueue;
		Array<Array<Transform*>> transformQueue;
		Array<Array<glm::mat4>> worldMatrixQueue;
		GLuint uniformBuffer;

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
		GLuint billboardVAO;
		GLuint billboardVBO;
		Array<BillboardCollection> billboardCollections;

		SwapArray<DirectionalLight> directionalLights;
		SwapArray<PointLight> pointLights;

		int writeIndex, readIndex;
		float elapsedTime;
		bool lightingEnabled;
	};
}