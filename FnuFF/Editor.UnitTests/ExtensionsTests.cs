using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace Editor.UnitTests
{
	[TestClass]
	public class ExtensionsTests
	{
		[TestMethod]
		public void WindingSort2D_Left_SortsCorrectly()
		{
			var points = new[]
			{
				new PointF(0,-1),
				new PointF(-1,-1),
				new PointF(0,0),
				new PointF(-1,0)
			};

			var sorted = Extensions.WindingSort2D( points );

			Assert.AreEqual( points[1], sorted[0] );
			Assert.AreEqual( points[0], sorted[1] );
			Assert.AreEqual( points[2], sorted[2] );
			Assert.AreEqual( points[3], sorted[3] );
		}

		[TestMethod]
		public void WindingSort2D_Right_SortsCorrectly()
		{
			var points = new[]
			{
				new PointF(0,1),
				new PointF(1,1),
				new PointF(0,0),
				new PointF(1,0)
			};

			var sorted = Extensions.WindingSort2D( points );

			Assert.AreEqual( points[2], sorted[0] );
			Assert.AreEqual( points[3], sorted[1] );
			Assert.AreEqual( points[1], sorted[2] );
			Assert.AreEqual( points[0], sorted[3] );
		}

		[TestMethod]
		public void WindingSort2D_Top_SortsCorrectly()
		{
			var points = new[]
			{
				new PointF(0,0),
				new PointF(1,0),
				new PointF(0,1),
				new PointF(1,1)
			};

			var sorted = Extensions.WindingSort2D( points );

			Assert.AreEqual( points[0], sorted[0] );
			Assert.AreEqual( points[1], sorted[1] );
			Assert.AreEqual( points[3], sorted[2] );
			Assert.AreEqual( points[2], sorted[3] );
		}

		[TestMethod]
		public void WindingSort2D_Bottom_SortsCorrectly()
		{
			var points = new[]
			{
				new PointF(0,0),
				new PointF(-1,0),
				new PointF(0,-1),
				new PointF(-1,-1)
			};

			var sorted = Extensions.WindingSort2D( points );

			Assert.AreEqual( points[3], sorted[0] );
			Assert.AreEqual( points[2], sorted[1] );
			Assert.AreEqual( points[0], sorted[2] );
			Assert.AreEqual( points[1], sorted[3] );
		}

		[TestMethod]
		public void WindingSort2D_Front_SortsCorrectly()
		{
			var points = new[]
			{
				new PointF(0,-1),
				new PointF(0,0),
				new PointF(-1,-1),
				new PointF(-1,0)
			};

			var sorted = Extensions.WindingSort2D( points );

			Assert.AreEqual( points[2], sorted[0] );
			Assert.AreEqual( points[0], sorted[1] );
			Assert.AreEqual( points[1], sorted[2] );
			Assert.AreEqual( points[3], sorted[3] );
		}

		[TestMethod]
		public void WindingSort2D_Back_SortsCorrectly()
		{
			var points = new[]
			{
				new PointF(0,1),
				new PointF(0,0),
				new PointF(1,1),
				new PointF(1,0)
			};

			var sorted = Extensions.WindingSort2D( points );

			Assert.AreEqual( points[1], sorted[0] );
			Assert.AreEqual( points[3], sorted[1] );
			Assert.AreEqual( points[2], sorted[2] );
			Assert.AreEqual( points[0], sorted[3] );
		}

		[TestMethod]
		public void WindingIndex2D_Left_SortsCorrectly()
		{
			var points = new[]
			{
				new PointF(0,-1),
				new PointF(-1,-1),
				new PointF(0,0),
				new PointF(-1,0)
			};

			var sorted = Extensions.WindingIndex2D( points );

			Assert.AreEqual( 1, sorted[0] );
			Assert.AreEqual( 0, sorted[1] );
			Assert.AreEqual( 2, sorted[2] );
			Assert.AreEqual( 3, sorted[3] );
		}

		[TestMethod]
		public void IntersectPlanes_NoPlanes_ReturnsNoPoints()
		{
			var p0 = new Plane( new Triple( 0, 0, 1 ), 1 );
			var ps = new Plane[] { };

			var points = Extensions.IntersectPlanes( p0, ps );

			Assert.AreEqual( 0, points.Length );
		}

		[TestMethod]
		public void IntersectPlanes_OnePlane_ReturnsNoPoints()
		{
			var p0 = new Plane( new Triple( 0, 0, 1 ), 1 );
			var ps = new Plane[] { new Plane(new Triple(1, 0, 0 ), 5) };

			var points = Extensions.IntersectPlanes( p0, ps );

			Assert.AreEqual( 0, points.Length );
		}

		[TestMethod]
		public void IntersectPlanes_ParallellPlanes_ReturnsNoPoints()
		{
			var p0 = new Plane( new Triple( 0, 0, 1 ), 1 );
			var ps = new Plane[] { new Plane(new Triple(0,0,1), 2), new Plane(new Triple(0,0,1), 3)};

			var points = Extensions.IntersectPlanes( p0, ps );

			Assert.AreEqual( 0, points.Length );
		}

		[TestMethod]
		public void IntersectPlanes_Triangle_ReturnsThreePoints()
		{
			var p0 = new Plane( new Triple( 0, 0, 1 ), 1 );
			var ps = new Plane[]
			{
				new Plane(new Triple( -1, 1, 0 ), 1),
				new Plane(new Triple(1, 1, 0), 1 ),
				new Plane(new Triple(0,-1,0), 1),
			};

			var points = Extensions.IntersectPlanes( p0, ps );

			Assert.AreEqual( 3, points.Length );

			Assert.IsTrue( points[0].X == 0.0f && points[0].Y > 0.0f && points[0].Z == 1.0f );
			Assert.IsTrue( points[1].X < 0 && points[1].Y < 0 && points[1].Z == 1.0f );
			Assert.IsTrue( points[2].X > 0 && points[2].Y < 0 && points[2].Z == 1.0f );
		}

		[TestMethod]
		public void IntersectPlanes_Rectangle_ReturnsFourPoints()
		{
			var p0 = new Plane( new Triple( 0, 0, 1 ), 1 );
			var ps = new Plane[]
			{
				new Plane(new Triple( -1, 0, 0 ), 1),
				new Plane(new Triple(0, 1, 0), 1 ),
				new Plane(new Triple(1,0,0), 1),
				new Plane(new Triple(0,-1,0), 1)
			};

			var points = Extensions.IntersectPlanes( p0, ps );

			Assert.AreEqual( 4, points.Length );

			Assert.AreEqual( new Triple( -1, 1, 1 ), points[0] );
			Assert.AreEqual( new Triple( -1, -1, 1 ), points[1] );
			Assert.AreEqual( new Triple( 1, 1, 1 ), points[2] );
			Assert.AreEqual( new Triple( 1, -1, 1 ), points[3] );
		}

		[TestMethod]
		public void IntersectPlanes_Pentagon_ReturnsFivePoints()
		{
			var p0 = new Plane( new Triple( 0, 0, 1 ), 1 );
			var ps = new Plane[]
			{
				new Plane(new Triple( -2, -1, 0 ), 1),
				new Plane(new Triple(-1, 1, 0), 1 ),
				new Plane(new Triple(1,1,0), 1),
				new Plane(new Triple(2,-1,0), 1),
				new Plane(new Triple(0,-1,0), 1)
			};

			var points = Extensions.IntersectPlanes( p0, ps );

			Assert.AreEqual( 5, points.Length );

			Assert.IsTrue( points[0].X < 0 && Math.Abs(points[0].Y) < 0.5f && points[0].Z == 1.0f );
			Assert.IsTrue( points[1].X < 0 && points[1].Y < 0 && points[1].Z == 1.0f );
			Assert.IsTrue( points[2].X == 0.0f && points[2].Y > 0 && points[2].Z == 1.0f );
			Assert.IsTrue( points[3].X > 0 && Math.Abs( points[3].Y ) < 0.5f && points[3].Z == 1.0f );
			Assert.IsTrue( points[4].X > 0 && points[4].Y < 0 && points[4].Z == 1.0f );
		}
	}
}
