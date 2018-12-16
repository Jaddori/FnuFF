using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Editor.UnitTests
{
	[TestClass]
	public class TripleTests
	{
		private Triple _sut;

		[TestInitialize]
		public void Initialize()
		{
			_sut = new Triple(1,2,3);
		}

		[TestCleanup]
		public void Cleanup()
		{
		}

		[TestMethod]
		public void Project_DirectionX_ReturnsCorrect()
		{
			var r1 = _sut.Project( new Triple( 1, 0, 0 ) );
			var r2 = _sut.Project( new Triple( -1, 0, 0 ) );

			Assert.AreEqual( new Point( 3, 2 ), r1 );
			Assert.AreEqual( new Point( -3, -2 ), r2 );
		}

		[TestMethod]
		public void Project_DirectionY_ReturnsCorrect()
		{
			var r1 = _sut.Project( new Triple( 0, 1, 0 ) );
			var r2 = _sut.Project( new Triple( 0, -1, 0 ) );

			Assert.AreEqual( new Point( 1, 3 ), r1 );
			Assert.AreEqual( new Point( -1, -3 ), r2 );
		}

		[TestMethod]
		public void Project_DirectionZ_ReturnsCorrect()
		{
			var r1 = _sut.Project( new Triple( 0, 0, 1 ) );
			var r2 = _sut.Project( new Triple( 0, 0, -1 ) );

			Assert.AreEqual( new Point( 1, 2 ), r1 );
			Assert.AreEqual( new Point( -1, -2 ), r2 );
		}
	}
}
