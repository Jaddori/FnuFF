using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace Editor.UnitTests
{
	[TestClass]
	public class GeometrySolidTests
	{
		private Camera2D _camera;
		private Solid _sut;

		[TestInitialize]
		public void Initialize()
		{
			_camera = new Camera2D();
			_sut = new Solid( new Triple( 1, 2, 3 ), new Triple( 2, 4, 6 ) );
		}

		[TestCleanup]
		public void Cleanup()
		{
			_camera = null;
			_sut = null;
		}
	}
}
