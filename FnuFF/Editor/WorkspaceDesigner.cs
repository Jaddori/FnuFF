using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;

namespace Editor
{
    public class WorkspaceDesigner : ControlDesigner
    {
        public override void Initialize( IComponent component )
        {
            base.Initialize( component );

            WorkspaceControl ws = component as WorkspaceControl;

            EnableDesignMode( ws.PanelTopLeft, "Top Left" );
            EnableDesignMode( ws.PanelTopRight, "Top Right" );
            EnableDesignMode( ws.PanelBottomLeft, "Bottom Left" );
            EnableDesignMode( ws.PanelBottomRight, "Bottom Right" );
        }
    }
}
