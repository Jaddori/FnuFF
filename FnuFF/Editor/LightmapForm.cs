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

		public LightmapForm( Level level )
		{
			InitializeComponent();
			_level = level;
		}

		private void LightmapForm_Load( object sender, EventArgs e )
		{
			btn_close.Enabled = false;
		}

		private void LightmapForm_Shown( object sender, EventArgs e )
		{
			PollProgress();
			btn_close.Enabled = false;

			timer_progress.Start();

			_thread = new Thread( new ThreadStart( ThreadWork ) );
			_thread.Start();
		}

		private void ThreadWork()
		{
			Lightmap.Generate( _level );
		}

		private void PollProgress()
		{
			int completed, total;
			bool done;
			Lightmap.PollProgress( out completed, out total, out done );

			lbl_progress.Text = $"Lumel: {completed}/{total}";

			if( pb_progress.Maximum != total )
				pb_progress.Maximum = total;
			if( pb_progress.Value != completed )
				pb_progress.Value = completed;

			//if( completed >= total )
			if( done )
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
