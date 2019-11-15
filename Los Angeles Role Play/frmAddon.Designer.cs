namespace LARPLauncher {
    partial class frmAddon {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.LeftPanel = new System.Windows.Forms.Panel();
            this.RightPanel = new System.Windows.Forms.Panel();
            this.TopPanel = new System.Windows.Forms.Panel();
            this.HeadLabel = new System.Windows.Forms.Label();
            this.AddonList = new System.Windows.Forms.Panel();
            this.CloseLabel = new System.Windows.Forms.Label();
            this.BottomPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // LeftPanel
            // 
            this.LeftPanel.BackColor = System.Drawing.Color.DodgerBlue;
            this.LeftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.LeftPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftPanel.Name = "LeftPanel";
            this.LeftPanel.Size = new System.Drawing.Size(1, 502);
            this.LeftPanel.TabIndex = 5;
            // 
            // RightPanel
            // 
            this.RightPanel.BackColor = System.Drawing.Color.DodgerBlue;
            this.RightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.RightPanel.Location = new System.Drawing.Point(301, 0);
            this.RightPanel.Name = "RightPanel";
            this.RightPanel.Size = new System.Drawing.Size(1, 502);
            this.RightPanel.TabIndex = 6;
            // 
            // TopPanel
            // 
            this.TopPanel.BackColor = System.Drawing.Color.DodgerBlue;
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(1, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(300, 1);
            this.TopPanel.TabIndex = 8;
            // 
            // HeadLabel
            // 
            this.HeadLabel.BackColor = System.Drawing.Color.Transparent;
            this.HeadLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HeadLabel.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.HeadLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.HeadLabel.Location = new System.Drawing.Point(1, 1);
            this.HeadLabel.Name = "HeadLabel";
            this.HeadLabel.Size = new System.Drawing.Size(300, 50);
            this.HeadLabel.TabIndex = 9;
            this.HeadLabel.Text = "애드온 관리자";
            this.HeadLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.HeadLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HeadCaption_MouseDown);
            this.HeadLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HeadCaption_MouseMove);
            // 
            // AddonList
            // 
            this.AddonList.AutoScroll = true;
            this.AddonList.BackColor = System.Drawing.Color.WhiteSmoke;
            this.AddonList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddonList.Location = new System.Drawing.Point(1, 51);
            this.AddonList.Name = "AddonList";
            this.AddonList.Size = new System.Drawing.Size(300, 450);
            this.AddonList.TabIndex = 11;
            // 
            // CloseLabel
            // 
            this.CloseLabel.AutoSize = true;
            this.CloseLabel.BackColor = System.Drawing.Color.LightCoral;
            this.CloseLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CloseLabel.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.CloseLabel.ForeColor = System.Drawing.Color.White;
            this.CloseLabel.Location = new System.Drawing.Point(281, 1);
            this.CloseLabel.Name = "CloseLabel";
            this.CloseLabel.Size = new System.Drawing.Size(20, 20);
            this.CloseLabel.TabIndex = 0;
            this.CloseLabel.Text = "×";
            // 
            // BottomPanel
            // 
            this.BottomPanel.BackColor = System.Drawing.Color.DodgerBlue;
            this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomPanel.Location = new System.Drawing.Point(1, 501);
            this.BottomPanel.Name = "BottomPanel";
            this.BottomPanel.Size = new System.Drawing.Size(300, 1);
            this.BottomPanel.TabIndex = 7;
            // 
            // frmAddon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(302, 502);
            this.Controls.Add(this.CloseLabel);
            this.Controls.Add(this.AddonList);
            this.Controls.Add(this.HeadLabel);
            this.Controls.Add(this.TopPanel);
            this.Controls.Add(this.BottomPanel);
            this.Controls.Add(this.RightPanel);
            this.Controls.Add(this.LeftPanel);
            this.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmAddon";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmAddon";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel LeftPanel;
        private System.Windows.Forms.Panel RightPanel;
        private System.Windows.Forms.Panel TopPanel;
        private System.Windows.Forms.Label HeadLabel;
        private System.Windows.Forms.Panel AddonList;
        private System.Windows.Forms.Label CloseLabel;
        private System.Windows.Forms.Panel BottomPanel;
    }
}