#pragma once

#include "common.h"

namespace Rendering
{
	class Shader
	{
	public:
		Shader();
		~Shader();

		bool load( const char* vertex, const char* geometry, const char* fragment );
		void unload();

		inline void bind() { glUseProgram( program ); }
		inline GLint getLocation( const char* name ) { return glGetUniformLocation( program, name ); }

		inline void setInt( GLint location, int value ) { glUniform1i( location, value ); }
		inline void setFloat( GLint location, float value ) { glUniform1f( location, value ); }
		inline void setVec2( GLint location, const glm::vec2& value ) { glUniform2f( location, value.x, value.y ); }
		inline void setVec3( GLint location, const glm::vec3& value ) { glUniform3f( location, value.x, value.y, value.z ); }
		inline void setVec4( GLint location, const glm::vec4& value ) { glUniform4f( location, value.x, value.y, value.z, value.w ); }
		inline void setMat4( GLint location, const glm::mat4& value ) { glUniformMatrix4fv( location, 1, GL_FALSE, glm::value_ptr( value ) ); }

		inline void setInt( GLint location, const int* values, int n ) { glUniform1iv( location, n, values ); }
		inline void setFloat( GLint location, const float* values, int n ) { glUniform1fv( location, n, values ); }
		inline void setVec2( GLint location, const glm::vec2* values, int n ) { glUniform2fv( location, n, glm::value_ptr( *values ) ); }
		inline void setVec3( GLint location, const glm::vec3* values, int n ) { glUniform3fv( location, n, glm::value_ptr( *values ) ); }
		inline void setVec4( GLint location, const glm::vec4* values, int n ) { glUniform4fv( location, n, glm::value_ptr( *values ) ); }
		inline void setMat4( GLint location, const glm::mat4* values, int n ) { glUniformMatrix4fv( location, n, GL_FALSE, glm::value_ptr( *values ) ); }

		GLuint getProgram() const;
		bool getValid() const;

	private:
		GLuint loadShader( const char* path, GLenum type );

		GLuint program;
		bool valid;
	};
}