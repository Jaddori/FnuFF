namespace Editor
{
	partial class EntityTabContentControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lbl_type = new System.Windows.Forms.Label();
			this.cmb_type = new System.Windows.Forms.ComboBox();
			this.lbl_data = new System.Windows.Forms.Label();
			this.pg_data = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// lbl_type
			// 
			this.lbl_type.AutoSize = true;
			this.lbl_type.ForeColor = System.Drawing.Color.White;
			this.lbl_type.Location = new System.Drawing.Point(8, 8);
			this.lbl_type.Name = "lbl_type";
			this.lbl_type.Size = new System.Drawing.Size(34, 13);
			this.lbl_type.TabIndex = 0;
			this.lbl_type.Text = "Type:";
			// 
			// cmb_type
			// 
			this.cmb_type.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cmb_type.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
			this.cmb_type.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmb_type.ForeColor = System.Drawing.Color.White;
			this.cmb_type.FormattingEnabled = true;
			this.cmb_type.Location = new System.Drawing.Point(3, 24);
			this.cmb_type.Name = "cmb_type";
			this.cmb_type.Size = new System.Drawing.Size(286, 21);
			this.cmb_type.TabIndex = 1;
			this.cmb_type.SelectedIndexChanged += new System.EventHandler(this.cmb_type_SelectedIndexChanged);
			// 
			// lbl_data
			// 
			this.lbl_data.AutoSize = true;
			this.lbl_data.ForeColor = System.Drawing.Color.White;
			this.lbl_data.Location = new System.Drawing.Point(8, 48);
			this.lbl_data.Name = "lbl_data";
			this.lbl_data.Size = new System.Drawing.Size(33, 13);
			this.lbl_data.TabIndex = 3;
			this.lbl_data.Text = "Data:";
			// 
			// pg_data
			// 
			this.pg_data.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pg_data.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
			this.pg_data.CategoryForeColor = System.Drawing.Color.Silver;
			this.pg_data.CategorySplitterColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
			this.pg_data.CommandsBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
			this.pg_data.CommandsForeColor = System.Drawing.Color.White;
			this.pg_data.DisabledItemForeColor = System.Drawing.Color.White;
			this.pg_data.HelpBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.pg_data.HelpBorderColor = System.Drawing.Color.Silver;
			this.pg_data.HelpForeColor = System.Drawing.Color.White;
			this.pg_data.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.pg_data.Location = new System.Drawing.Point(3, 64);
			this.pg_data.Name = "pg_data";
			this.pg_data.Size = new System.Drawing.Size(286, 503);
			this.pg_data.TabIndex = 4;
			this.pg_data.ViewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
			this.pg_data.ViewBorderColor = System.Drawing.Color.Silver;
			this.pg_data.ViewForeColor = System.Drawing.Color.White;
			// 
			// EntityTabContentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pg_data);
			this.Controls.Add(this.lbl_data);
			this.Controls.Add(this.cmb_type);
			this.Controls.Add(this.lbl_type);
			this.Name = "EntityTabContentControl";
			this.Size = new System.Drawing.Size(292, 570);
			this.Load += new System.EventHandler(this.EntityTabContentControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbl_type;
		private System.Windows.Forms.ComboBox cmb_type;
		private System.Windows.Forms.Label lbl_data;
		private System.Windows.Forms.PropertyGrid pg_data;
	}
}
