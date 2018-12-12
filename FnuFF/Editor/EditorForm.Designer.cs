namespace Editor
{
    partial class EditorForm
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
            this.status_editor = new System.Windows.Forms.StatusStrip();
            this.stlbl_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.menu_editor = new System.Windows.Forms.MenuStrip();
            this.pnl_left = new System.Windows.Forms.Panel();
            this.pnl_right = new System.Windows.Forms.Panel();
            this.ws_viewports = new Editor.WorkspaceControl();
            this.view_bottomLeft = new Editor.View2DControl();
            this.view_bottomRight = new Editor.View2DControl();
            this.view3DControl1 = new Editor.View3DControl();
            this.view_topRight = new Editor.View2DControl();
            this.btn_solid = new Editor.FlatButtonControl();
            this.btn_select = new Editor.FlatButtonControl();
            this.status_editor.SuspendLayout();
            this.pnl_left.SuspendLayout();
            this.ws_viewports.PanelBottomLeft.SuspendLayout();
            this.ws_viewports.PanelBottomRight.SuspendLayout();
            this.ws_viewports.PanelTopLeft.SuspendLayout();
            this.ws_viewports.PanelTopRight.SuspendLayout();
            this.ws_viewports.SuspendLayout();
            this.SuspendLayout();
            // 
            // status_editor
            // 
            this.status_editor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.status_editor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stlbl_status});
            this.status_editor.Location = new System.Drawing.Point(0, 726);
            this.status_editor.Name = "status_editor";
            this.status_editor.Size = new System.Drawing.Size(1239, 22);
            this.status_editor.TabIndex = 1;
            this.status_editor.Text = "statusStrip1";
            // 
            // stlbl_status
            // 
            this.stlbl_status.Name = "stlbl_status";
            this.stlbl_status.Size = new System.Drawing.Size(0, 17);
            // 
            // menu_editor
            // 
            this.menu_editor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.menu_editor.Location = new System.Drawing.Point(0, 0);
            this.menu_editor.Name = "menu_editor";
            this.menu_editor.Size = new System.Drawing.Size(1239, 24);
            this.menu_editor.TabIndex = 2;
            this.menu_editor.Text = "menuStrip1";
            // 
            // pnl_left
            // 
            this.pnl_left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.pnl_left.Controls.Add(this.btn_solid);
            this.pnl_left.Controls.Add(this.btn_select);
            this.pnl_left.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnl_left.Location = new System.Drawing.Point(0, 24);
            this.pnl_left.Name = "pnl_left";
            this.pnl_left.Size = new System.Drawing.Size(58, 702);
            this.pnl_left.TabIndex = 3;
            // 
            // pnl_right
            // 
            this.pnl_right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.pnl_right.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnl_right.Location = new System.Drawing.Point(1039, 24);
            this.pnl_right.Name = "pnl_right";
            this.pnl_right.Size = new System.Drawing.Size(200, 702);
            this.pnl_right.TabIndex = 4;
            // 
            // ws_viewports
            // 
            this.ws_viewports.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ws_viewports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ws_viewports.Location = new System.Drawing.Point(58, 24);
            this.ws_viewports.Name = "ws_viewports";
            // 
            // ws_viewports.Bottom Left
            // 
            this.ws_viewports.PanelBottomLeft.Controls.Add(this.view_bottomLeft);
            this.ws_viewports.PanelBottomLeft.Location = new System.Drawing.Point(1, 353);
            this.ws_viewports.PanelBottomLeft.Name = "Bottom Left";
            this.ws_viewports.PanelBottomLeft.Size = new System.Drawing.Size(488, 349);
            this.ws_viewports.PanelBottomLeft.TabIndex = 2;
            // 
            // ws_viewports.Bottom Right
            // 
            this.ws_viewports.PanelBottomRight.Controls.Add(this.view_bottomRight);
            this.ws_viewports.PanelBottomRight.Location = new System.Drawing.Point(492, 353);
            this.ws_viewports.PanelBottomRight.Name = "Bottom Right";
            this.ws_viewports.PanelBottomRight.Size = new System.Drawing.Size(488, 349);
            this.ws_viewports.PanelBottomRight.TabIndex = 3;
            // 
            // ws_viewports.Top Left
            // 
            this.ws_viewports.PanelTopLeft.Controls.Add(this.view3DControl1);
            this.ws_viewports.PanelTopLeft.Location = new System.Drawing.Point(1, 1);
            this.ws_viewports.PanelTopLeft.Name = "Top Left";
            this.ws_viewports.PanelTopLeft.Size = new System.Drawing.Size(488, 349);
            this.ws_viewports.PanelTopLeft.TabIndex = 0;
            // 
            // ws_viewports.Top Right
            // 
            this.ws_viewports.PanelTopRight.Controls.Add(this.view_topRight);
            this.ws_viewports.PanelTopRight.Location = new System.Drawing.Point(492, 1);
            this.ws_viewports.PanelTopRight.Name = "Top Right";
            this.ws_viewports.PanelTopRight.Size = new System.Drawing.Size(488, 349);
            this.ws_viewports.PanelTopRight.TabIndex = 1;
            this.ws_viewports.Size = new System.Drawing.Size(981, 702);
            this.ws_viewports.TabIndex = 5;
            this.ws_viewports.Text = "workspaceControl1";
            // 
            // view_bottomLeft
            // 
            this.view_bottomLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.view_bottomLeft.Direction = 0;
            this.view_bottomLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.view_bottomLeft.Location = new System.Drawing.Point(0, 0);
            this.view_bottomLeft.Name = "view_bottomLeft";
            this.view_bottomLeft.Size = new System.Drawing.Size(488, 349);
            this.view_bottomLeft.TabIndex = 0;
            // 
            // view_bottomRight
            // 
            this.view_bottomRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.view_bottomRight.Direction = 2;
            this.view_bottomRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.view_bottomRight.Location = new System.Drawing.Point(0, 0);
            this.view_bottomRight.Name = "view_bottomRight";
            this.view_bottomRight.Size = new System.Drawing.Size(488, 349);
            this.view_bottomRight.TabIndex = 0;
            // 
            // view3DControl1
            // 
            this.view3DControl1.BackColor = System.Drawing.Color.Black;
            this.view3DControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.view3DControl1.Location = new System.Drawing.Point(0, 0);
            this.view3DControl1.Name = "view3DControl1";
            this.view3DControl1.Size = new System.Drawing.Size(488, 349);
            this.view3DControl1.TabIndex = 0;
            this.view3DControl1.Text = "view3DControl1";
            // 
            // view_topRight
            // 
            this.view_topRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.view_topRight.Direction = 1;
            this.view_topRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.view_topRight.Location = new System.Drawing.Point(0, 0);
            this.view_topRight.Name = "view_topRight";
            this.view_topRight.Size = new System.Drawing.Size(488, 349);
            this.view_topRight.TabIndex = 0;
            // 
            // btn_solid
            // 
            this.btn_solid.Hovered = false;
            this.btn_solid.Image = global::Editor.Properties.Resources.icon_solid;
            this.btn_solid.Location = new System.Drawing.Point(0, 44);
            this.btn_solid.Name = "btn_solid";
            this.btn_solid.Pressed = false;
            this.btn_solid.Selected = false;
            this.btn_solid.Size = new System.Drawing.Size(58, 32);
            this.btn_solid.TabIndex = 2;
            this.btn_solid.Tag = "";
            this.btn_solid.UseVisualStyleBackColor = true;
            this.btn_solid.Click += new System.EventHandler(this.toolbarButton_Click);
            // 
            // btn_select
            // 
            this.btn_select.Hovered = false;
            this.btn_select.Image = global::Editor.Properties.Resources.icon_select;
            this.btn_select.Location = new System.Drawing.Point(0, 12);
            this.btn_select.Name = "btn_select";
            this.btn_select.Pressed = false;
            this.btn_select.Selected = true;
            this.btn_select.Size = new System.Drawing.Size(58, 32);
            this.btn_select.TabIndex = 1;
            this.btn_select.Tag = "";
            this.btn_select.UseVisualStyleBackColor = true;
            this.btn_select.Click += new System.EventHandler(this.toolbarButton_Click);
            // 
            // EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1239, 748);
            this.Controls.Add(this.ws_viewports);
            this.Controls.Add(this.pnl_right);
            this.Controls.Add(this.pnl_left);
            this.Controls.Add(this.status_editor);
            this.Controls.Add(this.menu_editor);
            this.MainMenuStrip = this.menu_editor;
            this.Name = "EditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FnuFF Editor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.status_editor.ResumeLayout(false);
            this.status_editor.PerformLayout();
            this.pnl_left.ResumeLayout(false);
            this.ws_viewports.PanelBottomLeft.ResumeLayout(false);
            this.ws_viewports.PanelBottomRight.ResumeLayout(false);
            this.ws_viewports.PanelTopLeft.ResumeLayout(false);
            this.ws_viewports.PanelTopRight.ResumeLayout(false);
            this.ws_viewports.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip status_editor;
        private System.Windows.Forms.ToolStripStatusLabel stlbl_status;
        private System.Windows.Forms.MenuStrip menu_editor;
        private System.Windows.Forms.Panel pnl_left;
        private System.Windows.Forms.Panel pnl_right;
        private FlatButtonControl btn_select;
        private FlatButtonControl btn_solid;
        private WorkspaceControl ws_viewports;
        private View2DControl view_topRight;
        private View2DControl view_bottomLeft;
        private View2DControl view_bottomRight;
        private View3DControl view3DControl1;
    }
}

