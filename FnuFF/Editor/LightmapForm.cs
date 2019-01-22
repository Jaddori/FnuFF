using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Editor
{
	public partial class LightmapForm : Form
	{
		private Thread _thread;
		private Level _level;
		private string _filename;

		public LightmapForm( Level level, string filename )
		{
			InitializeComponent();
			_level = level;
			_filename = filename;
		}

		private void LightmapForm_Load( object sender, EventArgs e )
		{
			btn_close.Enabled = false;
		}

		private void LightmapForm_Shown( object sender, EventArgs e )
		{
			PollProgress();

			timer_progress.Start();

			_thread = new Thread( new ThreadStart( ThreadWork ) );
			_thread.Start();
		}

		private void ThreadWork()
		{
			Lightmap.Generate( _level, _filename );
		}

		private void PollProgress()
		{
			int done, total;
			Lightmap.PollProgress( out done, out total );

			lbl_progress.Text = $"Lumel: {done}/{total}";

			if( pb_progress.Maximum != total )
				pb_progress.Maximum = total;
			if( pb_progress.Value != done )
				pb_progress.Value = done;

			if( done >= total )
			{
				timer_progress.Stop();
				btn_close.Enabled = true;

				if( _thread != null )
					_thread.Join();
			}
		}

		private void timer_progress_Tick( object sender, EventArgs e )
		{
			PollProgress();
		}

		private void btn_close_Click( object sender, EventArgs e )
		{
			Close();
		}
	}
}
