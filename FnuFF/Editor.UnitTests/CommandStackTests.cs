using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Editor.UndoRedo;

namespace Editor.UnitTests
{
	[TestClass]
	public class CommandStackTests
	{
		private CommandStack _sut;

		[TestInitialize]
		public void Initialize()
		{
			_sut = new CommandStack();
		}

		[TestCleanup]
		public void Cleanup()
		{
			_sut = null;
		}

		[TestMethod]
		public void Do_IsFull_WrapsAround()
		{
			var command = new CommandStackTestCommand();

			const int MAX = CommandStack.MAX_COMMANDS;

			for( int i = 0; i < MAX; i++ )
				_sut.Do( command );

			Assert.AreEqual( MAX - 1, _sut.Index );
			Assert.AreEqual( MAX, _sut.Commands.Count );

			for( int i = 0; i < 10; i++ )
				_sut.Do( command );

			Assert.AreEqual( MAX - 1, _sut.Index );
			Assert.AreEqual( MAX, _sut.Commands.Count );
		}

		[TestMethod]
		public void Undo_Empty_DoesNothing()
		{
			Assert.AreEqual( 0, _sut.Index );
			Assert.AreEqual( 1, _sut.Commands.Count );

			_sut.Undo();

			Assert.AreEqual( 0, _sut.Index );
			Assert.AreEqual( 1, _sut.Commands.Count );
		}

		[TestMethod]
		public void Undo_ValidCommand_Undoes()
		{
			var command = new CommandStackTestCommand();

			_sut.Do( command );
			_sut.Undo();

			Assert.AreEqual( 1, command.Undos );
		}

		[TestMethod]
		public void Redo_TopOfStack_DoesNothing()
		{
			Assert.AreEqual( 0, _sut.Index );
			Assert.AreEqual( 1, _sut.Commands.Count );

			_sut.Redo();

			Assert.AreEqual( 0, _sut.Index );
			Assert.AreEqual( 1, _sut.Commands.Count );

			var command = new CommandStackTestCommand();
			_sut.Do( command );

			Assert.AreEqual( 1, _sut.Index );
			Assert.AreEqual( 2, _sut.Commands.Count );

			_sut.Redo();

			Assert.AreEqual( 1, _sut.Index );
			Assert.AreEqual( 2, _sut.Commands.Count );
		}

		[TestMethod]
		public void Redo_ValidCommand_Redoes()
		{
			var command = new CommandStackTestCommand();

			_sut.Do( command );
			_sut.Undo();
			_sut.Redo();

			Assert.AreEqual( 1, command.Undos );
		}
	}
}
