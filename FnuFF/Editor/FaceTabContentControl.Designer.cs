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
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.pb_texture = new System.Windows.Forms.PictureBox();
			this.btn_browse = new System.Windows.Forms.Button();
			this.lbl_offset = new System.Windows.Forms.Label();
			this.lbl_rotation = new System.Windows.Forms.Label();
			this.lbl_scalex = new System.Windows.Forms.Label();
			this.num_offsetx = new System.Windows.Forms.NumericUpDown();
			this.num_offsety = new System.Windows.Forms.NumericUpDown();
			this.num_scaley = new System.Windows.Forms.NumericUpDown();
			this.num_scalex = new System.Windows.Forms.NumericUpDown();
			this.num_rotation = new System.Windows.Forms.NumericUpDown();
			this.lbl_lumelSize = new System.Windows.Forms.Label();
			this.num_lumelSize = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.pb_texture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.num_offsetx)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.num_offsety)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.num_scaley)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.num_scalex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.num_rotation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.num_lumelSize)).BeginInit();
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
			this.txt_textureName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.txt_textureName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txt_textureName.ForeColor = System.Drawing.Color.White;
			this.txt_textureName.Location = new System.Drawing.Point(6, 19);
			this.txt_textureName.Name = "txt_textureName";
			this.txt_textureName.ReadOnly = true;
			this.txt_textureName.Size = new System.Drawing.Size(149, 20);
			this.txt_textureName.TabIndex = 1;
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
			// btn_browse
			// 
			this.btn_browse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_browse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.btn_browse.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
			this.btn_browse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_browse.ForeColor = System.Drawing.Color.White;
			this.btn_browse.Location = new System.Drawing.Point(76, 45);
			this.btn_browse.Name = "btn_browse";
			this.btn_browse.Size = new System.Drawing.Size(79, 23);
			this.btn_browse.TabIndex = 3;
			this.btn_browse.Text = "Browse";
			this.btn_browse.UseVisualStyleBackColor = false;
			this.btn_browse.Click += new System.EventHandler(this.btn_browse_Click);
			// 
			// lbl_offset
			// 
			this.lbl_offset.AutoSize = true;
			this.lbl_offset.ForeColor = System.Drawing.Color.White;
			this.lbl_offset.Location = new System.Drawing.Point(3, 122);
			this.lbl_offset.Name = "lbl_offset";
			this.lbl_offset.Size = new System.Drawing.Size(38, 13);
			this.lbl_offset.TabIndex = 4;
			this.lbl_offset.Text = "Offset:";
			// 
			// lbl_rotation
			// 
			this.lbl_rotation.AutoSize = true;
			this.lbl_rotation.ForeColor = System.Drawing.Color.White;
			this.lbl_rotation.Location = new System.Drawing.Point(3, 200);
			this.lbl_rotation.Name = "lbl_rotation";
			this.lbl_rotation.Size = new System.Drawing.Size(50, 13);
			this.lbl_rotation.TabIndex = 5;
			this.lbl_rotation.Text = "Rotation:";
			// 
			// lbl_scalex
			// 
			this.lbl_scalex.AutoSize = true;
			this.lbl_scalex.ForeColor = System.Drawing.Color.White;
			this.lbl_scalex.Location = new System.Drawing.Point(3, 161);
			this.lbl_scalex.Name = "lbl_scalex";
			this.lbl_scalex.Size = new System.Drawing.Size(37, 13);
			this.lbl_scalex.TabIndex = 6;
			this.lbl_scalex.Text = "Scale:";
			// 
			// num_offsetx
			// 
			this.num_offsetx.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.num_offsetx.DecimalPlaces = 2;
			this.num_offsetx.ForeColor = System.Drawing.Color.White;
			this.num_offsetx.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.num_offsetx.Location = new System.Drawing.Point(6, 138);
			this.num_offsetx.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.num_offsetx.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.num_offsetx.Name = "num_offsetx";
			this.num_offsetx.Size = new System.Drawing.Size(63, 20);
			this.num_offsetx.TabIndex = 7;
			this.num_offsetx.ValueChanged += new System.EventHandler(this.num_offsetx_ValueChanged);
			// 
			// num_offsety
			// 
			this.num_offsety.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.num_offsety.DecimalPlaces = 2;
			this.num_offsety.ForeColor = System.Drawing.Color.White;
			this.num_offsety.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.num_offsety.Location = new System.Drawing.Point(75, 138);
			this.num_offsety.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.num_offsety.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.num_offsety.Name = "num_offsety";
			this.num_offsety.Size = new System.Drawing.Size(63, 20);
			this.num_offsety.TabIndex = 8;
			this.num_offsety.ValueChanged += new System.EventHandler(this.num_offsety_ValueChanged);
			// 
			// num_scaley
			// 
			this.num_scaley.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.num_scaley.DecimalPlaces = 2;
			this.num_scaley.ForeColor = System.Drawing.Color.White;
			this.num_scaley.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.num_scaley.Location = new System.Drawing.Point(75, 177);
			this.num_scaley.Name = "num_scaley";
			this.num_scaley.Size = new System.Drawing.Size(63, 20);
			this.num_scaley.TabIndex = 10;
			this.num_scaley.ValueChanged += new System.EventHandler(this.num_scaley_ValueChanged);
			// 
			// num_scalex
			// 
			this.num_scalex.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.num_scalex.DecimalPlaces = 2;
			this.num_scalex.ForeColor = System.Drawing.Color.White;
			this.num_scalex.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.num_scalex.Location = new System.Drawing.Point(6, 177);
			this.num_scalex.Name = "num_scalex";
			this.num_scalex.Size = new System.Drawing.Size(63, 20);
			this.num_scalex.TabIndex = 9;
			this.num_scalex.ValueChanged += new System.EventHandler(this.num_scalex_ValueChanged);
			// 
			// num_rotation
			// 
			this.num_rotation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.num_rotation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.num_rotation.ForeColor = System.Drawing.Color.White;
			this.num_rotation.Increment = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.num_rotation.Location = new System.Drawing.Point(6, 216);
			this.num_rotation.Maximum = new decimal(new int[] {
            361,
            0,
            0,
            0});
			this.num_rotation.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.num_rotation.Name = "num_rotation";
			this.num_rotation.Size = new System.Drawing.Size(149, 20);
			this.num_rotation.TabIndex = 11;
			this.num_rotation.ValueChanged += new System.EventHandler(this.num_rotation_ValueChanged);
			// 
			// lbl_lumelSize
			// 
			this.lbl_lumelSize.AutoSize = true;
			this.lbl_lumelSize.ForeColor = System.Drawing.Color.White;
			this.lbl_lumelSize.Location = new System.Drawing.Point(3, 239);
			this.lbl_lumelSize.Name = "lbl_lumelSize";
			this.lbl_lumelSize.Size = new System.Drawing.Size(59, 13);
			this.lbl_lumelSize.TabIndex = 12;
			this.lbl_lumelSize.Text = "Lumel size:";
			// 
			// num_lumelSize
			// 
			this.num_lumelSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.num_lumelSize.ForeColor = System.Drawing.Color.White;
			this.num_lumelSize.Location = new System.Drawing.Point(6, 255);
			this.num_lumelSize.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
			this.num_lumelSize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.num_lumelSize.Name = "num_lumelSize";
			this.num_lumelSize.Size = new System.Drawing.Size(149, 20);
			this.num_lumelSize.TabIndex = 13;
			this.num_lumelSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.num_lumelSize.ValueChanged += new System.EventHandler(this.num_lumelSize_ValueChanged);
			// 
			// FaceTabContentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.num_lumelSize);
			this.Controls.Add(this.lbl_lumelSize);
			this.Controls.Add(this.num_rotation);
			this.Controls.Add(this.num_scaley);
			this.Controls.Add(this.num_scalex);
			this.Controls.Add(this.btn_browse);
			this.Controls.Add(this.num_offsety);
			this.Controls.Add(this.pb_texture);
			this.Controls.Add(this.num_offsetx);
			this.Controls.Add(this.txt_textureName);
			this.Controls.Add(this.lbl_offset);
			this.Controls.Add(this.lbl_textureName);
			this.Controls.Add(this.lbl_scalex);
			this.Controls.Add(this.lbl_rotation);
			this.Name = "FaceTabContentControl";
			this.Size = new System.Drawing.Size(158, 542);
			this.Resize += new System.EventHandler(this.FaceTabContentControl_Resize);
			((System.ComponentModel.ISupportInitialize)(this.pb_texture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.num_offsetx)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.num_offsety)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.num_scaley)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.num_scalex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.num_rotation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.num_lumelSize)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbl_textureName;
		private System.Windows.Forms.TextBox txt_textureName;
		private System.Windows.Forms.PictureBox pb_texture;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Button btn_browse;
		private System.Windows.Forms.Label lbl_offset;
		private System.Windows.Forms.Label lbl_rotation;
		private System.Windows.Forms.Label lbl_scalex;
		private System.Windows.Forms.NumericUpDown num_rotation;
		private System.Windows.Forms.NumericUpDown num_scaley;
		private System.Windows.Forms.NumericUpDown num_scalex;
		private System.Windows.Forms.NumericUpDown num_offsety;
		private System.Windows.Forms.NumericUpDown num_offsetx;
		private System.Windows.Forms.Label lbl_lumelSize;
		private System.Windows.Forms.NumericUpDown num_lumelSize;
	}
}
