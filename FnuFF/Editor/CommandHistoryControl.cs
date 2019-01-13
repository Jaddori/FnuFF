using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using Editor.UndoRedo;

namespace Editor
{
	public class CommandHistoryControl : UserControl
	{
		private CommandStack _commandStack;
		private Color _undoColor;
		private Color _redoColor;
		private Color _currentColor;
		private int _hoverIndex;
		private bool _pressed;

		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public CommandStack CommandStack
		{
			get { return _commandStack; }
			set
			{
				_commandStack = value;
				_commandStack.OnDo += ( command ) => { UpdateScrollbars(true); Invalidate(); };
				_commandStack.OnRedo += (command) => { UpdateScrollbars(); Invalidate(); };
				_commandStack.OnUndo += ( command ) => { UpdateScrollbars(); Invalidate(); };
			}
		}

		public Color UndoColor
		{
			get { return _undoColor; }
			set
			{
				_undoColor = value;
				Invalidate();
			}
		}

		public Color RedoColor
		{
			get { return _redoColor; }
			set
			{
				_redoColor = value;
				Invalidate();
			}
		}

		public Color CurrentColor
		{
			get { return _currentColor; }
			set
			{
				_currentColor = value;
				Invalidate();
			}
		}

		public CommandHistoryControl()
		{
			DoubleBuffered = true;
			AutoScroll = true;

			_commandStack = null;
			_undoColor = Color.White;
			_redoColor = Color.DarkGray;
			_currentColor = Color.Red;
			_hoverIndex = -1;
			_pressed = false;
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			var g = e.Graphics;

			if( DesignMode )
			{
				var bounds = new Rectangle( 0, 0, Size.Width - 1, Size.Height - 1 );
				using( var pen = new Pen( Color.DarkGray ) )
					e.Graphics.DrawRectangle( pen, bounds );

				var demoCommands = new[] { "Undo command", "Current command", "Redo command" };
				var index = 1;

				using( var undoBrush = new SolidBrush( UndoColor ) )
				using( var redoBrush = new SolidBrush( RedoColor ) )
				using( var currentBrush = new SolidBrush( CurrentColor ) )
				{
					var offset = 0;
					for( int i = 0; i < demoCommands.Length; i++ )
					{
						var desc = demoCommands[i];
						var textSize = g.MeasureString( desc, Font );
						var rect = new Rectangle( Padding.Left, Padding.Top + offset, Size.Width - Padding.Horizontal, Font.Height + Padding.Vertical );
						var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

						var brush = undoBrush;
						if( i == index )
							brush = currentBrush;
						else if( i > index )
							brush = redoBrush;

						g.DrawString( desc, Font, brush, rect, format );
						offset += rect.Height;
					}
				}

				return;
			}

			if( _commandStack == null )
				return;

			g.TranslateTransform( 0, AutoScrollPosition.Y );

			using( var undoBrush = new SolidBrush( UndoColor ) )
			using( var redoBrush = new SolidBrush( RedoColor ) )
			using( var currentBrush = new SolidBrush( CurrentColor ) )
			using( var hoverBrush = new SolidBrush( EditorColors.PRESSED ) )
			using( var pressBrush = new SolidBrush( EditorColors.PRESSED_DARK ) )
			{
				var offset = 0;
				for( int i = 0; i < _commandStack.Commands.Count; i++ )
				{
					var command = _commandStack.Commands[i];
					var desc = command.GetDescription();
					var textSize = g.MeasureString( desc, Font );
					var rect = new Rectangle( Padding.Left, Padding.Top + offset, Size.Width - Padding.Horizontal, Font.Height + Padding.Vertical );
					var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
					
					// draw hover/press indicator
					if( _hoverIndex == i )
					{
						var indicatorBrush = hoverBrush;
						if( _pressed )
							indicatorBrush = pressBrush;

						g.FillRectangle( indicatorBrush, rect );
					}

					// draw text
					var brush = undoBrush;
					if( i == _commandStack.Index )
						brush = currentBrush;
					else if( i > _commandStack.Index )
						brush = redoBrush;
					
					g.DrawString( desc, Font, brush, rect, format );
					offset += rect.Height;
				}
			}
		}

		protected override void OnMouseLeave( EventArgs e )
		{
			_hoverIndex = -1;

			base.OnMouseLeave( e );
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			_pressed = true;

			base.OnMouseDown( e );

			Invalidate();
		}

		protected override void OnMouseUp( MouseEventArgs e )
		{
			_pressed = false;
			
			var index = e.Y / ( Font.Height + Padding.Vertical );
			_commandStack.SetIndex( index );
			_hoverIndex = -1;
			
			base.OnMouseUp( e );

			Invalidate();
		}

		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			var prevIndex = _hoverIndex;
			_hoverIndex = e.Y / (Font.Height + Padding.Vertical);

			if( _hoverIndex != prevIndex )
			{
				if( _hoverIndex < _commandStack.Commands.Count || prevIndex < _commandStack.Commands.Count )
				{
					Invalidate();
				}
			}
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			UpdateScrollbars();
			Invalidate();
		}

		public void UpdateScrollbars( bool updatePosition = false )
		{
			if( _commandStack != null )
			{
				var lineHeight = Font.Height + Padding.Vertical;
				AutoScrollMinSize = new Size( 0, _commandStack.Commands.Count * lineHeight );

				if( updatePosition )
				{
					AutoScrollPosition = new Point( 0, _commandStack.Commands.Count * lineHeight );
				}
			}
		}
	}
}
