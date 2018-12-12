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
    public class CameraTests
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
            Assert.AreEqual( new Point( 1, 1 ), p1global );

            _sut.Position = new Point( 100, 100 );
            _sut.Zoom = 2.0f;

            var p2 = new Point( 10, 10 );
            var p2global = _sut.ToGlobal( p2 );
            Assert.AreEqual( new Point( 55, 55 ), p2global ); 
        }

        [TestMethod]
        public void ToGlobal_WithZoomAndNegativePosition_ReturnsCorrect()
        {
            _sut.Position = new Point( -10, -10 );
            _sut.Zoom = 10.0f;

            var p1 = new Point( 0, 0 );
            var p1global = _sut.ToGlobal( p1 );
            Assert.AreEqual( new Point( -1, -1 ), p1global );

            var p2 = new Point( 20, 20 );
            var p2global = _sut.ToGlobal( p2 );
            Assert.AreEqual( new Point( 1, 1 ), p2global );
        }
    }
}
