namespace Editor
{
	partial class LightmapForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.btn_close = new System.Windows.Forms.Button();
			this.lbl_progress = new System.Windows.Forms.Label();
			this.pb_progress = new System.Windows.Forms.ProgressBar();
			this.timer_progress = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// btn_close
			// 
			this.btn_close.Enabled = false;
			this.btn_close.Location = new System.Drawing.Point(217, 54);
			this.btn_close.Name = "btn_close";
			this.btn_close.Size = new System.Drawing.Size(75, 23);
			this.btn_close.TabIndex = 0;
			this.btn_close.Text = "Close";
			this.btn_close.UseVisualStyleBackColor = true;
			this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
			// 
			// lbl_progress
			// 
			this.lbl_progress.AutoSize = true;
			this.lbl_progress.Location = new System.Drawing.Point(12, 9);
			this.lbl_progress.Name = "lbl_progress";
			this.lbl_progress.Size = new System.Drawing.Size(63, 13);
			this.lbl_progress.TabIndex = 1;
			this.lbl_progress.Text = "Lumels: 0/0";
			// 
			// pb_progress
			// 
			this.pb_progress.Location = new System.Drawing.Point(12, 25);
			this.pb_progress.Name = "pb_progress";
			this.pb_progress.Size = new System.Drawing.Size(280, 23);
			this.pb_progress.TabIndex = 2;
			// 
			// timer_progress
			// 
			this.timer_progress.Interval = 1000;
			this.timer_progress.Tick += new System.EventHandler(this.timer_progress_Tick);
			// 
			// LightmapForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(304, 89);
			this.Controls.Add(this.pb_progress);
			this.Controls.Add(this.lbl_progress);
			this.Controls.Add(this.btn_close);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LightmapForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FnuFF - Lightmap Generation";
			this.Load += new System.EventHandler(this.LightmapForm_Load);
			this.Shown += new System.EventHandler(this.LightmapForm_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btn_close;
		private System.Windows.Forms.Label lbl_progress;
		private System.Windows.Forms.ProgressBar pb_progress;
		private System.Windows.Forms.Timer timer_progress;
	}
}