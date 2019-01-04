namespace Editor
{
	partial class FaceTabContentControl
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
			this.lbl_textureName = new System.Windows.Forms.Label();
			this.txt_textureName = new System.Windows.Forms.TextBox();
			this.btn_browse = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.pb_texture = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pb_texture)).BeginInit();
			this.SuspendLayout();
			// 
			// lbl_textureName
			// 
			this.lbl_textureName.AutoSize = true;
			this.lbl_textureName.ForeColor = System.Drawing.Color.White;
			this.lbl_textureName.Location = new System.Drawing.Point(3, 3);
			this.lbl_textureName.Name = "lbl_textureName";
			this.lbl_textureName.Size = new System.Drawing.Size(46, 13);
			this.lbl_textureName.TabIndex = 0;
			this.lbl_textureName.Text = "Texture:";
			// 
			// txt_textureName
			// 
			this.txt_textureName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txt_textureName.Location = new System.Drawing.Point(6, 19);
			this.txt_textureName.Name = "txt_textureName";
			this.txt_textureName.Size = new System.Drawing.Size(149, 20);
			this.txt_textureName.TabIndex = 1;
			// 
			// btn_browse
			// 
			this.btn_browse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_browse.Location = new System.Drawing.Point(76, 45);
			this.btn_browse.Name = "btn_browse";
			this.btn_browse.Size = new System.Drawing.Size(79, 23);
			this.btn_browse.TabIndex = 3;
			this.btn_browse.Text = "Browse";
			this.btn_browse.UseVisualStyleBackColor = true;
			this.btn_browse.Click += new System.EventHandler(this.btn_browse_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.FileName = "openFileDialog1";
			// 
			// pb_texture
			// 
			this.pb_texture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pb_texture.Location = new System.Drawing.Point(6, 45);
			this.pb_texture.Name = "pb_texture";
			this.pb_texture.Size = new System.Drawing.Size(64, 64);
			this.pb_texture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pb_texture.TabIndex = 2;
			this.pb_texture.TabStop = false;
			// 
			// FaceTabContentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btn_browse);
			this.Controls.Add(this.pb_texture);
			this.Controls.Add(this.txt_textureName);
			this.Controls.Add(this.lbl_textureName);
			this.Name = "FaceTabContentControl";
			this.Size = new System.Drawing.Size(158, 126);
			((System.ComponentModel.ISupportInitialize)(this.pb_texture)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbl_textureName;
		private System.Windows.Forms.TextBox txt_textureName;
		private System.Windows.Forms.PictureBox pb_texture;
		private System.Windows.Forms.Button btn_browse;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
	}
}
