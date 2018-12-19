using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace Editor.UnitTests
{
	[TestClass]
	public class GeometrySolidTests
	{
		private Camera2D _camera;
		private GeometrySolid _sut;

		[TestInitialize]
		public void Initialize()
		{
			_camera = new Camera2D();
			_sut = new GeometrySolid( new Triple( 1, 2, 3 ), new Triple( 2, 4, 6 ) );
		}

		[TestCleanup]
		public void Cleanup()
		{
			_camera = null;
			_sut = null;
		}

		[TestMethod]
		public void Project_DirectionX_ReturnsCorrect()
		{
			_camera.Direction = new Triple( 1, 0, 0 );
			var bounds = _sut.Project( _camera, 64, 1 );

			Assert.AreEqual( new Rectangle( 3*64, -4*64, 3*64, 2*64 ), bounds );
		}

		[TestMethod]
		public void Project_DirectionY_ReturnsCorrect()
		{
			_camera.Direction = new Triple( 0, 1, 0 );
			var bounds = _sut.Project( _camera, 64, 1 );

			Assert.AreEqual( new Rectangle( 1 * 64, 3 * 64, 1 * 64, 3 * 64 ), bounds );
		}

		[TestMethod]
		public void Project_DirectionZ_ReturnsCorrect()
		{
			_camera.Direction = new Triple( 0, 0, 1 );
			var bounds = _sut.Project( _camera, 64, 1 );

			Assert.AreEqual( new Rectangle( 1 * 64, -4*64, 1 * 64, 2*64), bounds );
		}

		[TestMethod]
		public void Unproject_DirectionX_ReturnsCorrect()
		{
			_camera.Direction = new Triple( 1, 0, 0 );
			var bounds = new Rectangle( 64, 64, 64, 64 );
			_sut.Unproject( _camera, bounds, 64, 1 );

			Assert.AreEqual( new Triple( 1, -2, 1 ), _sut.Min );
			Assert.AreEqual( new Triple( 2, -1, 2 ), _sut.Max );
		}

		[TestMethod]
		public void Unroject_DirectionY_ReturnsCorrect()
		{
			_camera.Direction = new Triple( 0, 1, 0 );
			var bounds = new Rectangle( 64, 64, 64, 64 );
			_sut.Unproject( _camera, bounds, 64, 1 );

			Assert.AreEqual( new Triple( 1, 2, 1 ), _sut.Min );
			Assert.AreEqual( new Triple( 2, 4, 2 ), _sut.Max );
		}

		[TestMethod]
		public void Unroject_DirectionZ_ReturnsCorrect()
		{
			_camera.Direction = new Triple( 0, 0, 1 );
			var bounds = new Rectangle( 64, 64, 64, 64 );
			_sut.Unproject( _camera, bounds, 64, 1 );

			Assert.AreEqual( new Triple( 1, -2, 3 ), _sut.Min );
			Assert.AreEqual( new Triple( 2, -1, 6 ), _sut.Max );
		}

		[TestMethod]
		public void Unproject_CameraOffset_AccountsForOffset()
		{
			_camera.Direction = new Triple( 0, 0, 1 );
			_camera.Position = new Point( 64, 64 );
			var bounds = new Rectangle( 64, 64, 64, 64 );
			_sut.Unproject( _camera, bounds, 64, 1 );

			Assert.AreEqual( new Triple( 2, -3, 3 ), _sut.Min );
			Assert.AreEqual( new Triple( 3, -2, 6 ), _sut.Max );
		}
	}
}
