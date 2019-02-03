using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Editor.Entities;

namespace Editor
{
	public partial class EntityTabContentControl : UserControl
	{
		private bool _silent;

		public EntityTabContentControl()
		{
			InitializeComponent();
		}

		private void EntityTabContentControl_Load( object sender, EventArgs e )
		{
			cmb_type.Items.Clear();
			foreach( var name in EntityList.Names )
			{
				cmb_type.Items.Add( name );
			}

			EditorTool.OnSelectedEntityChanged += ( prev, cur ) =>
			{
				_silent = true;

				if( cur == null )
				{
					cmb_type.SelectedIndex = -1;
					pg_data.SelectedObject = null;
				}
				else
				{
					var index = -1;
					for( int i = 0; i < EntityList.Types.Length && index < 0; i++ )
					{
						if( EntityList.Types[i] == cur.Data.GetType() )
							index = i;
					}

					if( index >= 0 )
					{
						cmb_type.SelectedIndex = index;
					}
					pg_data.SelectedObject = cur.Data;
				}

				_silent = false;
			};
		}

		private void cmb_type_SelectedIndexChanged( object sender, EventArgs e )
		{
			if( _silent )
				return;

			if( cmb_type.SelectedIndex < 0 || cmb_type.SelectedIndex >= cmb_type.Items.Count )
				return;

			if( EditorTool.SelectedEntity == null )
				return;

			EditorTool.SelectedEntity.Data = EntityList.CreateData( cmb_type.SelectedIndex );
			pg_data.SelectedObject = EditorTool.SelectedEntity.Data;
		}
	}
}
