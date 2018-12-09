#pragma once

#include "common.h"

namespace Physics
{
	struct Ray
	{
		glm::vec3 start, direction;
		float length;
	};

	struct Sphere
	{
		glm::vec3 center;
		float radius;
	};

	struct AABB
	{
		glm::vec3 minPosition, maxPosition;
	};

	struct Plane
	{
		glm::vec3 normal;
		float offset;
	};

	struct Triangle
	{
		glm::vec3 v[3];
	};

	struct Hit
	{
		glm::vec3 position;
		float length;
		glm::vec3 normal;
	};

	class CollisionSolver
	{
	public:
		CollisionSolver();
		~CollisionSolver();

		bool ray( const Ray& ray, const Sphere& sphere, Hit* hit = NULL );
		bool ray( const Ray& ray, const AABB& aabb, Hit* hit = NULL );
		bool ray( const Ray& ray, const Plane& plane, Hit* hit = NULL );
		bool ray( const Ray& ray, const Triangle& triangle, Hit* hit = NULL );

		bool sphere( const Sphere& a, const Sphere& b, Hit* hit = NULL );

		bool aabb( const AABB& a, const AABB& b );
	};
}