using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Editor
{
    [Designer(typeof(WorkspaceDesigner))]
    public class WorkspaceControl : Control
    {
        private const int MARGIN = 1;

        private Brush _dividerBrush;
        private Rectangle _horizontalDivider;
        private Rectangle _verticalDivider;

        private Panel[] _panels;

        public Panel[] Panels { get { return _panels; } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Panel PanelTopLeft { get { return _panels[0]; } }
        [DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
        public Panel PanelTopRight { get { return _panels[1]; } }
        [DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
        public Panel PanelBottomLeft { get { return _panels[2]; } }
        [DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
        public Panel PanelBottomRight { get { return _panels[3]; } }

        public WorkspaceControl()
        {
            _dividerBrush = new SolidBrush( Color.Black );
            _horizontalDivider = new Rectangle( 0, 0, 0, 0 );
            _verticalDivider = new Rectangle( 0, 0, 0, 0 );

            var colors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow };

            _panels = new Panel[4];
            for( int i = 0; i < _panels.Length; i++ )
            {
                _panels[i] = new Panel();
                _panels[i].BackColor = colors[i];
            }

            Controls.AddRange( _panels );
        }

        protected override void OnResize( EventArgs e )
        {
            base.OnResize( e );

            var halfWidth = Size.Width / 2;
            var halfHeight = Size.Height / 2;

            for( int i = 0; i < _panels.Length; i++ )
                _panels[i].Size = new Size( halfWidth - MARGIN*2, halfHeight - MARGIN*2 );

            _panels[0].Location = new Point( MARGIN, MARGIN );
            _panels[1].Location = new Point( MARGIN*2 + halfWidth, MARGIN );
            _panels[2].Location = new Point( MARGIN, MARGIN*2 + halfHeight );
            _panels[3].Location = new Point( MARGIN*2 + halfWidth, MARGIN*2 + halfHeight );

            _horizontalDivider = new Rectangle( 0, halfHeight-1, Size.Width, 2 );
            _verticalDivider = new Rectangle( halfWidth - 1, 0, 2, Size.Height );
        }
    }
}
