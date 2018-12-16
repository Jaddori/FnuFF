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
	public class Camera2DTests
	{
		private Camera2D _sut;

		[TestInitialize]
		public void Initialize()
		{
			_sut = new Camera2D();
		}

		[TestCleanup]
		public void Cleanup()
		{
			_sut = null;
		}

		[TestMethod]
		public void ToLocal_PositiveValues_ReturnsCorrect()
		{
			_sut.Position = new Point( 10, 10 );
			var p1 = new Point( 30, 30 );

			var p1local = _sut.ToLocal( p1 );
			Assert.AreEqual( new Point( 20, 20 ), p1local );

			_sut.Position = new Point( 200, 200 );
			var p2 = new Point( 220, 220 );

			var p2local = _sut.ToLocal( p2 );
			Assert.AreEqual( new Point( 20, 20 ), p2local );
		}

		[TestMethod]
		public void ToLocal_NegativeValues_ReturnsCorrect()
		{
			_sut.Position = new Point( -10, -10 );
			var p1 = new Point( 0, 0 );

			var p1local = _sut.ToLocal( p1 );
			Assert.AreEqual( new Point( 10, 10 ), p1local );

			var p2 = new Point( 20, 20 );

			var p2local = _sut.ToLocal( p2 );
			Assert.AreEqual( new Point( 30, 30 ), p2local );
		}

		[TestMethod]
		public void ToGlobal_PositiveValues_ReturnsCorrect()
		{
			_sut.Position = new Point( 20, 20 );
			var p1 = new Point( 10, 10 );

			var p1global = _sut.ToGlobal( p1 );
			Assert.AreEqual( new Point( 30, 30 ), p1global );

			_sut.Position = new Point( 100, 100 );
			var p2 = new Point( 100, 100 );

			var p2global = _sut.ToGlobal( p2 );
			Assert.AreEqual( new Point( 200, 200 ), p2global );
		}

		[TestMethod]
		public void ToGlobal_NegativeValues_ReturnsCorrect()
		{
			_sut.Position = new Point( -10, -10 );
			var p1 = new Point( 0, 0 );

			var p1global = _sut.ToGlobal( p1 );
			Assert.AreEqual( new Point( -10, -10 ), p1global );

			_sut.Position = new Point( -100, -100 );
			var p2 = new Point( 200, 200 );

			var p2global = _sut.ToGlobal( p2 );
			Assert.AreEqual( new Point( 100, 100 ), p2global );
		}

		[TestMethod]
		public void ToGlobal_WithZoomAndPositivePosition_ReturnsCorrect()
		{
			_sut.Position = new Point( 10, 10 );
			_sut.Zoom = 10.0f;

			var p1 = new Point( 0, 0 );
			var p1global = _sut.ToGlobal( p1 );
			Assert.AreEqual( new Point( 100, 100 ), p1global );

			_sut.Position = new Point( 100, 100 );
			_sut.Zoom = 2.0f;

			var p2 = new Point( 10, 10 );
			var p2global = _sut.ToGlobal( p2 );
			Assert.AreEqual( new Point( 220, 220 ), p2global );
		}

		[TestMethod]
		public void ToGlobal_WithZoomAndNegativePosition_ReturnsCorrect()
		{
			_sut.Position = new Point( -10, -10 );
			_sut.Zoom = 10.0f;

			var p1 = new Point( 0, 0 );
			var p1global = _sut.ToGlobal( p1 );
			Assert.AreEqual( new Point( -100, -100 ), p1global );

			var p2 = new Point( 20, 20 );
			var p2global = _sut.ToGlobal( p2 );
			Assert.AreEqual( new Point( 100, 100 ), p2global );
		}

		[TestMethod]
		public void Project_DirectionX_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 1, 0, 0 );
			var point = new Triple( 1, 2, 3 );

			var result = _sut.Project( point );

			Assert.AreEqual( new Point( 3, -2 ), result );
		}

		[TestMethod]
		public void Project_DirectionY_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 0, 1, 0 );
			var point = new Triple( 1, 2, 3 );

			var result = _sut.Project( point );

			Assert.AreEqual( new Point( 1,3 ), result );
		}

		[TestMethod]
		public void Project_DirectionZ_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 0, 0, 1 );
			var point = new Triple( 1, 2, 3 );

			var result = _sut.Project( point );

			Assert.AreEqual( new Point( 1,-2 ), result );
		}

		[TestMethod]
		public void Unproject_DirectionX_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 1, 0, 0 );
			var point = new Point( 1, 2 );

			var result = _sut.Unproject( point, 3 );

			Assert.AreEqual( new Triple( 3, -2, 1 ), result );
		}

		[TestMethod]
		public void Unproject_DirectionY_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 0, 1, 0 );
			var point = new Point( 1, 2 );

			var result = _sut.Unproject( point, 3 );

			Assert.AreEqual( new Triple( 1,3,2 ), result );
		}

		[TestMethod]
		public void Unproject_DirectionZ_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 0, 0, 1 );
			var point = new Point( 1, 2 );

			var result = _sut.Unproject( point, 3 );

			Assert.AreEqual( new Triple( 1,-2, 3 ), result );
		}

        /*[TestMethod]
        public void Project_DirectionX_ReturnsCorrect()
        {
            _sut.Direction = new Triple( 1, 0, 0 );
            var point = new Triple( 1, 2, 3 );

            var result = _sut.Project( point );

            Assert.AreEqual( new Point( 3, -2 ), result );
        }

        [TestMethod]
        public void Project_DirectionY_ReturnsCorrect()
        {
            _sut.Direction = new Triple( 0, 1, 0 );
            var point = new Triple( 1, 2, 3 );

            var result = _sut.Project( point );

            Assert.AreEqual( new Point( 1, -3 ), result );
        }

        [TestMethod]
        public void Project_DirectionZ_ReturnsCorrect()
        {
            _sut.Direction = new Triple( 0, 0, 1 );
            var point = new Triple( 1, 2, 3 );

            var result = _sut.Project( point );

            Assert.AreEqual( new Point( 1, -2 ), result );
        }

		[TestMethod]
		public void Project_AxisX_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 1, 0, 0 );
			var x = new Triple( 1, 0, 0 );
			var y = new Triple( 0, 1, 0 );
			var z = new Triple( 0, 0, 1 );

			var rx = _sut.Project( x );
			var ry = _sut.Project( y );
			var rz = _sut.Project( z );

			Assert.AreEqual( new Point( 0, 0 ), rx );
			Assert.AreEqual( new Point( 0, -1 ), ry );
			Assert.AreEqual( new Point( 1, 0 ), rz );
		}

		[TestMethod]
		public void Project_AxisY_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 0, 1, 0 );
			var x = new Triple( 1, 0, 0 );
			var y = new Triple( 0, 1, 0 );
			var z = new Triple( 0, 0, 1 );

			var rx = _sut.Project( x );
			var ry = _sut.Project( y );
			var rz = _sut.Project( z );

			Assert.AreEqual( new Point( 1, 0 ), rx );
			Assert.AreEqual( new Point( 0, 0 ), ry );
			Assert.AreEqual( new Point( 0, -1 ), rz );
		}

		[TestMethod]
		public void Project_AxisZ_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 0, 0, 1 );
			var x = new Triple( 1, 0, 0 );
			var y = new Triple( 0, 1, 0 );
			var z = new Triple( 0, 0, 1 );

			var rx = _sut.Project( x );
			var ry = _sut.Project( y );
			var rz = _sut.Project( z );

			Assert.AreEqual( new Point( 1, 0 ), rx );
			Assert.AreEqual( new Point( 0, -1 ), ry );
			Assert.AreEqual( new Point( 0, 0 ), rz );
		}

		[TestMethod]
		public void Project_Box01_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 0, 1, 0 );

			var min = new Triple( 64, -32, 64 );
			var max = new Triple( 128, 32, 128 );

			var rmin = _sut.Project( min );
			var rmax = _sut.Project( max );

			Assert.AreEqual( new Point( 64, -64 ), rmin );
			Assert.AreEqual( new Point( 128, -128 ), rmax );

			_sut.Position = new Point( 0, -128 );

			var lmin = _sut.ToLocal( rmin );
			var lmax = _sut.ToLocal( rmax );

			Assert.AreEqual( new Point( 64, 64 ), lmin );
			Assert.AreEqual( new Point( 128, 0 ), lmax );
		}

		[TestMethod]
		public void UnprojectProject_DirectionX_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 1, 0, 0 );

			var min = new Point( 0, 0 );
			var max = new Point( 10, 10 );

			var gmin = _sut.Unproject( min, 0 );
			var gmax = _sut.Unproject( max, 10 );

			Assert.AreEqual( new Triple( 0, 0, 0 ), gmin );
			Assert.AreEqual( new Triple( 10, -10, 10 ), gmax );

			var lmin = _sut.Project( gmin );
			var lmax = _sut.Project( gmax );

			Assert.AreEqual( new Point( 0, 0 ), lmin );
			Assert.AreEqual( new Point( 10, 10 ), lmax );
		}

		[TestMethod]
		public void UnprojectProject_DirectionY_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 0, 1, 0 );

			var min = new Point( 0, 0 );
			var max = new Point( 10, 10 );

			var gmin = _sut.Unproject( min, 0 );
			var gmax = _sut.Unproject( max, 10 );

			Assert.AreEqual( new Triple( 0, 0, 0 ), gmin );
			Assert.AreEqual( new Triple( 10, 10, -10 ), gmax );

			var lmin = _sut.Project( gmin );
			var lmax = _sut.Project( gmax );

			Assert.AreEqual( new Point( 0, 0 ), lmin );
			Assert.AreEqual( new Point( 10, 10 ), lmax );
		}

		[TestMethod]
		public void UnprojectProject_DirectionZ_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 0, 0, 1 );

			var min = new Point( 0, 0 );
			var max = new Point( 10, 10 );

			var gmin = _sut.Unproject( min, 0 );
			var gmax = _sut.Unproject( max, 10 );

			Assert.AreEqual( new Triple( 0, 0, 0 ), gmin );
			Assert.AreEqual( new Triple( 10, -10, 10 ), gmax );

			var lmin = _sut.Project( gmin );
			var lmax = _sut.Project( gmax );

			Assert.AreEqual( new Point( 0, 0 ), lmin );
			Assert.AreEqual( new Point( 10, 10 ), lmax );
		}

		[TestMethod]
        public void Unproject_DirectionX_ReturnsCorrect()
        {
            _sut.Direction = new Triple( 1, 0, 0 );
            var point = new Point( 1, 2 );

            var result = _sut.Unproject( point );

            Assert.AreEqual( new Triple( 0, -2, 1 ), result );
        }

        [TestMethod]
        public void Unproject_DirectionY_ReturnsCorrect()
        {
            _sut.Direction = new Triple( 0, 1, 0 );
            var point = new Point( 1, 2 );

            var result = _sut.Unproject( point );

            Assert.AreEqual( new Triple( 1, 0, -2 ), result );
		}

        [TestMethod]
        public void Unproject_DirectionZ_ReturnsCorrect()
        {
            _sut.Direction = new Triple( 0, 0, 1 );
            var point = new Point( 1, 2 );

            var result = _sut.Unproject( point );

            Assert.AreEqual( new Triple( 1, -2, 0 ), result );
        }

		[TestMethod]
		public void Unproject_WithDepth_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 0, 0, 1 );
			var point = new Point( 1, 2 );

			var result = _sut.Unproject( point, 3 );

			Assert.AreEqual( new Triple( 1, -2, 3 ), result );
		}

		[TestMethod]
		public void Unproject_Box01_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 0, 1, 0 );
			var p1 = new Point( 0, 0 );
			var p2 = new Point( 128, 128 );

			var r1 = _sut.Unproject( p1, -32 );
			var r2 = _sut.Unproject( p2, 32 );

			Assert.AreEqual( new Triple( 0, -32, 0 ), r1 );
			Assert.AreEqual( new Triple( 128, 32, -128 ), r2 );

			var p3 = _sut.Project( r1 );
			var p4 = _sut.Project( r2 );

			Assert.AreEqual( Point.Empty, p3 );
			Assert.AreEqual( new Point(128,128), p4 );
		}

		[TestMethod]
		public void Unproject_Box02_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 0, 1, 0 );
			var p1 = new Point( 64, 64 );
			var p2 = new Point( 128, 128 );

			var r1 = _sut.Unproject( p1, -32 );
			var r2 = _sut.Unproject( p2, 32 );

			Assert.AreEqual( new Triple( 64, -32, -64 ), r1 );
			Assert.AreEqual( new Triple( 128, 32, -128 ), r2 );

			var p3 = _sut.Project( r1 );
			var p4 = _sut.Project( r2 );

			Assert.AreEqual( new Point(64,64), p3 );
			Assert.AreEqual( new Point(128,128), p4 );
		}

		[TestMethod]
		public void Unproject_AxisX_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 1, 0, 0 );

			var x = new Point( 1, 0 );
			var y = new Point( 0, 1 );

			var rx = _sut.Unproject( x );
			var ry = _sut.Unproject( y );

			Assert.AreEqual( new Triple( 0, 0, 1 ), rx );
			Assert.AreEqual( new Triple( 0, -1, 0 ), ry );
		}

		[TestMethod]
		public void Unproject_AxisY_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 0, 1, 0 );

			var x = new Point( 1, 0 );
			var y = new Point( 0, 1 );

			var rx = _sut.Unproject( x );
			var ry = _sut.Unproject( y );

			Assert.AreEqual( new Triple( 1, 0, 0 ), rx );
			Assert.AreEqual( new Triple( 0, 0, -1 ), ry );
		}

		[TestMethod]
		public void Unproject_AxisZ_ReturnsCorrect()
		{
			_sut.Direction = new Triple( 0, 0, 1 );

			var x = new Point( 1, 0 );
			var y = new Point( 0, 1 );

			var rx = _sut.Unproject( x );
			var ry = _sut.Unproject( y );

			Assert.AreEqual( new Triple( 1, 0, 0 ), rx );
			Assert.AreEqual( new Triple( 0, -1, 0 ), ry );
		}

		[TestMethod]
		public void ProjectUnproject_Consistent()
		{
			_sut.Direction = new Triple( 0, 0, 1 );

			var p1 = new Triple( 1, 0, 0 );

			var r1 = _sut.Project( p1 );
			var r2 = _sut.Unproject( r1 );

			Assert.AreEqual( p1, r2 );
		}*/

		[TestMethod]
		public void Snap_NoOffsets_ReturnsCorrect()
		{
			const int GAP = 16;
			var p1 = new Point( 5, 5 );
			var p2 = new Point( 20, 20 );

			var r1 = _sut.Snap( GAP, p1 );
			var r2 = _sut.Snap( GAP, p2 );

			Assert.AreEqual( new Point( 0, 0 ), r1 );
			Assert.AreEqual( new Point( GAP, GAP ), r2 );
		}
    }
}
