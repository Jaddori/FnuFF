using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Editor
{
    public partial class EditorForm : Form
    {
        private List<FlatButtonControl> _toolbarButtons;

        public EditorForm()
        {
            InitializeComponent();
        }

        private void Form1_Load( object sender, EventArgs e )
        {
            btn_select.Tag = EditorTools.Select;
            btn_solid.Tag = EditorTools.Solid;

            _toolbarButtons = new List<FlatButtonControl>();
            _toolbarButtons.Add( btn_select );
            _toolbarButtons.Add( btn_solid );
        }

        private void toolbarButton_Click( object sender, EventArgs e )
        {
            foreach( var b in _toolbarButtons )
                b.Selected = false;

            var button = sender as FlatButtonControl;
            button.Selected = true;

            EditorTool.Current = (EditorTools)button.Tag;

            Text = "FnuFF Editor - " + EditorTool.Current.ToString();
        }
    }
}
