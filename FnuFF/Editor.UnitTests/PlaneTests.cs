using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Editor.UnitTests
{
	[TestClass]
	public class PlaneTests
	{
		private Plane _sut;

		[TestInitialize]
		public void Initialize()
		{
			_sut = new Plane( new Triple( 1, 0, 0 ), 5.0f );
		}

		[TestCleanup]
		public void Cleanup()
		{
			_sut = null;
		}

		[TestMethod]
		public void Equals_Same_ReturnsTrue()
		{
			var plane = new Plane( new Triple( 1, 0, 0 ), 5.0f );

			var result = _sut.Equals( plane );

			Assert.IsTrue( result );
		}

		[TestMethod]
		public void Equals_NotSame_ReturnsFalse()
		{
			_sut.D = 1.0f;
			var p1 = new Plane( new Triple( 1, 0, 0 ), 5.0f );
			var p2 = new Plane( new Triple( 0, 0, 1 ), 1.0f );

			var r1 = _sut.Equals( p1 );
			var r2 = _sut.Equals( p2 );

			Assert.IsFalse( r1 );
			Assert.IsFalse( r2 );
		}

		[TestMethod]
		public void Equals_EpsilonOffset_ReturnsTrue()
		{
			_sut.D = 4.9999f;
			var plane = new Plane( new Triple( 1, 0, 0 ), 5.0f );

			var result = _sut.Equals( plane );

			Assert.IsTrue( result );
		}

		[TestMethod]
		public void InFront_IsInFront_ReturnsTrue()
		{
			_sut.D = 2.0f;
			var point = new Triple( 5, 0, 0 );

			var result = _sut.InFront( point );

			Assert.IsTrue( result );
		}

		[TestMethod]
		public void InFront_IsBehind_ReturnsFalse()
		{
			var point = new Triple( -1, 0, 0 );

			var result = _sut.InFront( point );

			Assert.IsFalse( result );
		}

		[TestMethod]
		public void OnPlane_IsOnPlane_ReturnsTrue()
		{
			var point = new Triple( 5, 0, 0 );

			var result = _sut.OnPlane( point );

			Assert.IsTrue( result );
		}

		[TestMethod]
		public void OnPlane_EpsilonOffset_ReturnsTrue()
		{
			var point = new Triple( 4.9999f, 0, 0 );

			var result = _sut.OnPlane( point );

			Assert.IsTrue( result );
		}

		[TestMethod]
		public void OnPlane_IsInFront_ReturnsFalse()
		{
			var point = new Triple( 10, 0, 0 );

			var result = _sut.OnPlane( point );

			Assert.IsFalse( result );
		}

		[TestMethod]
		public void OnPlane_IsBehind_ReturnsFalse()
		{
			var point = new Triple( -10, 0, 0 );

			var result = _sut.OnPlane( point );

			Assert.IsFalse( result );
		}
	}
}
