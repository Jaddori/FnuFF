namespace Editor
{
	partial class TextureBrowserForm
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
			this.pnl_preview = new System.Windows.Forms.Panel();
			this.flow_preview = new System.Windows.Forms.FlowLayoutPanel();
			this.pnl_filter = new System.Windows.Forms.Panel();
			this.txt_name = new System.Windows.Forms.TextBox();
			this.lbl_name = new System.Windows.Forms.Label();
			this.cmb_pack = new System.Windows.Forms.ComboBox();
			this.lbl_pack = new System.Windows.Forms.Label();
			this.pnl_preview.SuspendLayout();
			this.pnl_filter.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnl_preview
			// 
			this.pnl_preview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pnl_preview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.pnl_preview.Controls.Add(this.flow_preview);
			this.pnl_preview.Location = new System.Drawing.Point(0, 0);
			this.pnl_preview.Name = "pnl_preview";
			this.pnl_preview.Size = new System.Drawing.Size(1169, 605);
			this.pnl_preview.TabIndex = 0;
			// 
			// flow_preview
			// 
			this.flow_preview.AutoScroll = true;
			this.flow_preview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flow_preview.Location = new System.Drawing.Point(0, 0);
			this.flow_preview.Name = "flow_preview";
			this.flow_preview.Size = new System.Drawing.Size(1169, 605);
			this.flow_preview.TabIndex = 0;
			// 
			// pnl_filter
			// 
			this.pnl_filter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
			this.pnl_filter.Controls.Add(this.txt_name);
			this.pnl_filter.Controls.Add(this.lbl_name);
			this.pnl_filter.Controls.Add(this.cmb_pack);
			this.pnl_filter.Controls.Add(this.lbl_pack);
			this.pnl_filter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnl_filter.Location = new System.Drawing.Point(0, 603);
			this.pnl_filter.MaximumSize = new System.Drawing.Size(0, 100);
			this.pnl_filter.MinimumSize = new System.Drawing.Size(0, 100);
			this.pnl_filter.Name = "pnl_filter";
			this.pnl_filter.Size = new System.Drawing.Size(1169, 100);
			this.pnl_filter.TabIndex = 1;
			// 
			// txt_name
			// 
			this.txt_name.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
			this.txt_name.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txt_name.ForeColor = System.Drawing.Color.White;
			this.txt_name.Location = new System.Drawing.Point(12, 68);
			this.txt_name.Name = "txt_name";
			this.txt_name.Size = new System.Drawing.Size(285, 20);
			this.txt_name.TabIndex = 0;
			this.txt_name.TextChanged += new System.EventHandler(this.txt_name_TextChanged);
			// 
			// lbl_name
			// 
			this.lbl_name.AutoSize = true;
			this.lbl_name.ForeColor = System.Drawing.Color.White;
			this.lbl_name.Location = new System.Drawing.Point(12, 52);
			this.lbl_name.Name = "lbl_name";
			this.lbl_name.Size = new System.Drawing.Size(38, 13);
			this.lbl_name.TabIndex = 3;
			this.lbl_name.Text = "Name:";
			// 
			// cmb_pack
			// 
			this.cmb_pack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
			this.cmb_pack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmb_pack.ForeColor = System.Drawing.Color.White;
			this.cmb_pack.FormattingEnabled = true;
			this.cmb_pack.Location = new System.Drawing.Point(12, 28);
			this.cmb_pack.Name = "cmb_pack";
			this.cmb_pack.Size = new System.Drawing.Size(285, 21);
			this.cmb_pack.TabIndex = 1;
			this.cmb_pack.SelectedIndexChanged += new System.EventHandler(this.cmb_pack_SelectedIndexChanged);
			// 
			// lbl_pack
			// 
			this.lbl_pack.AutoSize = true;
			this.lbl_pack.ForeColor = System.Drawing.Color.White;
			this.lbl_pack.Location = new System.Drawing.Point(12, 12);
			this.lbl_pack.Name = "lbl_pack";
			this.lbl_pack.Size = new System.Drawing.Size(35, 13);
			this.lbl_pack.TabIndex = 2;
			this.lbl_pack.Text = "Pack:";
			// 
			// TextureBrowserForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1169, 703);
			this.Controls.Add(this.pnl_filter);
			this.Controls.Add(this.pnl_preview);
			this.Name = "TextureBrowserForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "FnuFF - Texture Browser";
			this.Load += new System.EventHandler(this.TextureBrowserForm_Load);
			this.pnl_preview.ResumeLayout(false);
			this.pnl_filter.ResumeLayout(false);
			this.pnl_filter.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnl_preview;
		private System.Windows.Forms.Panel pnl_filter;
		private System.Windows.Forms.FlowLayoutPanel flow_preview;
		private System.Windows.Forms.TextBox txt_name;
		private System.Windows.Forms.Label lbl_name;
		private System.Windows.Forms.ComboBox cmb_pack;
		private System.Windows.Forms.Label lbl_pack;
	}
}