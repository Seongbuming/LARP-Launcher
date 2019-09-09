namespace Los_Angeles_Role_Play
{
    partial class frmMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.DownloadWorker = new System.ComponentModel.BackgroundWorker();
            this.UnitProgressBar_Background = new System.Windows.Forms.Panel();
            this.UnitProgressBar = new System.Windows.Forms.Panel();
            this.PercentageLabel = new System.Windows.Forms.Label();
            this.TotalProgressBar_Background = new System.Windows.Forms.Panel();
            this.TotalProgressBar = new System.Windows.Forms.Panel();
            this.TopPanel = new System.Windows.Forms.Panel();
            this.LeftPanel = new System.Windows.Forms.Panel();
            this.ButtonContainer2 = new System.Windows.Forms.Panel();
            this.Button_2_1 = new System.Windows.Forms.Label();
            this.Button_2_2 = new System.Windows.Forms.Label();
            this.RightPanel = new System.Windows.Forms.Panel();
            this.BottomPanel = new System.Windows.Forms.Panel();
            this.HeadLabel = new System.Windows.Forms.Label();
            this.AntiCleoWorker = new System.ComponentModel.BackgroundWorker();
            this.GameStart = new System.Windows.Forms.Timer(this.components);
            this.GameExit = new System.Windows.Forms.Timer(this.components);
            this.Button_1_1 = new System.Windows.Forms.Label();
            this.ButtonContainer1 = new System.Windows.Forms.Panel();
            this.ButtonContainer3 = new System.Windows.Forms.Panel();
            this.Button_3_2 = new System.Windows.Forms.Label();
            this.Button_3_1 = new System.Windows.Forms.Label();
            this.Button_3_3 = new System.Windows.Forms.Label();
            this.UnitProgressBar_Background.SuspendLayout();
            this.TotalProgressBar_Background.SuspendLayout();
            this.ButtonContainer2.SuspendLayout();
            this.ButtonContainer1.SuspendLayout();
            this.ButtonContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // DownloadWorker
            // 
            this.DownloadWorker.WorkerReportsProgress = true;
            this.DownloadWorker.WorkerSupportsCancellation = true;
            this.DownloadWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DownloadWorker_DoWork);
            this.DownloadWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.DownloadWorker_ProgressChanged);
            this.DownloadWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.DownloadWorker_RunWorkerCompleted);
            // 
            // UnitProgressBar_Background
            // 
            this.UnitProgressBar_Background.BackColor = System.Drawing.Color.Silver;
            this.UnitProgressBar_Background.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.UnitProgressBar_Background.Controls.Add(this.UnitProgressBar);
            this.UnitProgressBar_Background.Location = new System.Drawing.Point(49, 90);
            this.UnitProgressBar_Background.Name = "UnitProgressBar_Background";
            this.UnitProgressBar_Background.Size = new System.Drawing.Size(402, 5);
            this.UnitProgressBar_Background.TabIndex = 2;
            // 
            // UnitProgressBar
            // 
            this.UnitProgressBar.BackColor = System.Drawing.Color.PeachPuff;
            this.UnitProgressBar.Location = new System.Drawing.Point(100, 1);
            this.UnitProgressBar.Name = "UnitProgressBar";
            this.UnitProgressBar.Size = new System.Drawing.Size(200, 3);
            this.UnitProgressBar.TabIndex = 0;
            // 
            // PercentageLabel
            // 
            this.PercentageLabel.BackColor = System.Drawing.Color.Transparent;
            this.PercentageLabel.Font = new System.Drawing.Font("Malgun Gothic Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.PercentageLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.PercentageLabel.Location = new System.Drawing.Point(50, 96);
            this.PercentageLabel.Name = "PercentageLabel";
            this.PercentageLabel.Size = new System.Drawing.Size(400, 30);
            this.PercentageLabel.TabIndex = 3;
            this.PercentageLabel.Text = "안녕하세요!";
            this.PercentageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TotalProgressBar_Background
            // 
            this.TotalProgressBar_Background.BackColor = System.Drawing.Color.Silver;
            this.TotalProgressBar_Background.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.TotalProgressBar_Background.Controls.Add(this.TotalProgressBar);
            this.TotalProgressBar_Background.Location = new System.Drawing.Point(49, 132);
            this.TotalProgressBar_Background.Name = "TotalProgressBar_Background";
            this.TotalProgressBar_Background.Size = new System.Drawing.Size(402, 5);
            this.TotalProgressBar_Background.TabIndex = 3;
            // 
            // TotalProgressBar
            // 
            this.TotalProgressBar.BackColor = System.Drawing.Color.PaleTurquoise;
            this.TotalProgressBar.Location = new System.Drawing.Point(100, 1);
            this.TotalProgressBar.Name = "TotalProgressBar";
            this.TotalProgressBar.Size = new System.Drawing.Size(200, 3);
            this.TotalProgressBar.TabIndex = 1;
            // 
            // TopPanel
            // 
            this.TopPanel.BackColor = System.Drawing.Color.DodgerBlue;
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(500, 1);
            this.TopPanel.TabIndex = 0;
            // 
            // LeftPanel
            // 
            this.LeftPanel.BackColor = System.Drawing.Color.DodgerBlue;
            this.LeftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.LeftPanel.Location = new System.Drawing.Point(0, 1);
            this.LeftPanel.Name = "LeftPanel";
            this.LeftPanel.Size = new System.Drawing.Size(1, 209);
            this.LeftPanel.TabIndex = 4;
            // 
            // ButtonContainer2
            // 
            this.ButtonContainer2.Controls.Add(this.Button_2_1);
            this.ButtonContainer2.Controls.Add(this.Button_2_2);
            this.ButtonContainer2.Location = new System.Drawing.Point(1, 174);
            this.ButtonContainer2.Name = "ButtonContainer2";
            this.ButtonContainer2.Size = new System.Drawing.Size(498, 35);
            this.ButtonContainer2.TabIndex = 9;
            this.ButtonContainer2.Visible = false;
            // 
            // Button_2_1
            // 
            this.Button_2_1.BackColor = System.Drawing.Color.AliceBlue;
            this.Button_2_1.Dock = System.Windows.Forms.DockStyle.Left;
            this.Button_2_1.ForeColor = System.Drawing.Color.SteelBlue;
            this.Button_2_1.Location = new System.Drawing.Point(0, 0);
            this.Button_2_1.Name = "Button_2_1";
            this.Button_2_1.Size = new System.Drawing.Size(249, 35);
            this.Button_2_1.TabIndex = 8;
            this.Button_2_1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Button_2_1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.Button_2_1.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.Button_2_1.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            this.Button_2_1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // Button_2_2
            // 
            this.Button_2_2.BackColor = System.Drawing.Color.AliceBlue;
            this.Button_2_2.Dock = System.Windows.Forms.DockStyle.Right;
            this.Button_2_2.Font = new System.Drawing.Font("Malgun Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Button_2_2.ForeColor = System.Drawing.Color.SteelBlue;
            this.Button_2_2.Location = new System.Drawing.Point(249, 0);
            this.Button_2_2.Name = "Button_2_2";
            this.Button_2_2.Size = new System.Drawing.Size(249, 35);
            this.Button_2_2.TabIndex = 9;
            this.Button_2_2.Text = "종료";
            this.Button_2_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Button_2_2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.Button_2_2.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.Button_2_2.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            this.Button_2_2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // RightPanel
            // 
            this.RightPanel.BackColor = System.Drawing.Color.DodgerBlue;
            this.RightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.RightPanel.Location = new System.Drawing.Point(499, 1);
            this.RightPanel.Name = "RightPanel";
            this.RightPanel.Size = new System.Drawing.Size(1, 209);
            this.RightPanel.TabIndex = 0;
            // 
            // BottomPanel
            // 
            this.BottomPanel.BackColor = System.Drawing.Color.DodgerBlue;
            this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomPanel.Location = new System.Drawing.Point(1, 209);
            this.BottomPanel.Name = "BottomPanel";
            this.BottomPanel.Size = new System.Drawing.Size(498, 1);
            this.BottomPanel.TabIndex = 5;
            // 
            // HeadLabel
            // 
            this.HeadLabel.BackColor = System.Drawing.Color.Transparent;
            this.HeadLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HeadLabel.Font = new System.Drawing.Font("Malgun Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.HeadLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.HeadLabel.Location = new System.Drawing.Point(1, 1);
            this.HeadLabel.Name = "HeadLabel";
            this.HeadLabel.Size = new System.Drawing.Size(498, 75);
            this.HeadLabel.TabIndex = 6;
            this.HeadLabel.Text = "    Los Angeles Role Play";
            this.HeadLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HeadLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HeadCaption_MouseDown);
            this.HeadLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HeadCaption_MouseMove);
            // 
            // GameStart
            // 
            this.GameStart.Tick += new System.EventHandler(this.GameStart_Tick);
            // 
            // GameExit
            // 
            this.GameExit.Interval = 1000;
            this.GameExit.Tick += new System.EventHandler(this.GameExit_Tick);
            // 
            // Button_1_1
            // 
            this.Button_1_1.BackColor = System.Drawing.Color.AliceBlue;
            this.Button_1_1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Button_1_1.Font = new System.Drawing.Font("Malgun Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Button_1_1.ForeColor = System.Drawing.Color.SteelBlue;
            this.Button_1_1.Location = new System.Drawing.Point(0, 0);
            this.Button_1_1.Name = "Button_1_1";
            this.Button_1_1.Size = new System.Drawing.Size(498, 35);
            this.Button_1_1.TabIndex = 7;
            this.Button_1_1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Button_1_1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.Button_1_1.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.Button_1_1.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            this.Button_1_1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // ButtonContainer1
            // 
            this.ButtonContainer1.Controls.Add(this.Button_1_1);
            this.ButtonContainer1.Location = new System.Drawing.Point(1, 174);
            this.ButtonContainer1.Name = "ButtonContainer1";
            this.ButtonContainer1.Size = new System.Drawing.Size(498, 35);
            this.ButtonContainer1.TabIndex = 8;
            this.ButtonContainer1.Visible = false;
            // 
            // ButtonContainer3
            // 
            this.ButtonContainer3.Controls.Add(this.Button_3_2);
            this.ButtonContainer3.Controls.Add(this.Button_3_1);
            this.ButtonContainer3.Controls.Add(this.Button_3_3);
            this.ButtonContainer3.Location = new System.Drawing.Point(1, 174);
            this.ButtonContainer3.Name = "ButtonContainer3";
            this.ButtonContainer3.Size = new System.Drawing.Size(498, 35);
            this.ButtonContainer3.TabIndex = 10;
            this.ButtonContainer3.Visible = false;
            // 
            // Button_3_2
            // 
            this.Button_3_2.BackColor = System.Drawing.Color.AliceBlue;
            this.Button_3_2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Button_3_2.ForeColor = System.Drawing.Color.SteelBlue;
            this.Button_3_2.Location = new System.Drawing.Point(166, 0);
            this.Button_3_2.Name = "Button_3_2";
            this.Button_3_2.Size = new System.Drawing.Size(166, 35);
            this.Button_3_2.TabIndex = 9;
            this.Button_3_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Button_3_2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.Button_3_2.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.Button_3_2.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            this.Button_3_2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // Button_3_1
            // 
            this.Button_3_1.BackColor = System.Drawing.Color.AliceBlue;
            this.Button_3_1.Dock = System.Windows.Forms.DockStyle.Left;
            this.Button_3_1.ForeColor = System.Drawing.Color.SteelBlue;
            this.Button_3_1.Location = new System.Drawing.Point(0, 0);
            this.Button_3_1.Name = "Button_3_1";
            this.Button_3_1.Size = new System.Drawing.Size(166, 35);
            this.Button_3_1.TabIndex = 8;
            this.Button_3_1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Button_3_1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.Button_3_1.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.Button_3_1.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            this.Button_3_1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // Button_3_3
            // 
            this.Button_3_3.BackColor = System.Drawing.Color.AliceBlue;
            this.Button_3_3.Dock = System.Windows.Forms.DockStyle.Right;
            this.Button_3_3.Font = new System.Drawing.Font("Malgun Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Button_3_3.ForeColor = System.Drawing.Color.SteelBlue;
            this.Button_3_3.Location = new System.Drawing.Point(332, 0);
            this.Button_3_3.Name = "Button_3_3";
            this.Button_3_3.Size = new System.Drawing.Size(166, 35);
            this.Button_3_3.TabIndex = 10;
            this.Button_3_3.Text = "종료";
            this.Button_3_3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Button_3_3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this.Button_3_3.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.Button_3_3.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            this.Button_3_3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // frmMain
            // 
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(500, 210);
            this.Controls.Add(this.ButtonContainer1);
            this.Controls.Add(this.ButtonContainer2);
            this.Controls.Add(this.ButtonContainer3);
            this.Controls.Add(this.HeadLabel);
            this.Controls.Add(this.BottomPanel);
            this.Controls.Add(this.RightPanel);
            this.Controls.Add(this.LeftPanel);
            this.Controls.Add(this.TotalProgressBar_Background);
            this.Controls.Add(this.PercentageLabel);
            this.Controls.Add(this.UnitProgressBar_Background);
            this.Controls.Add(this.TopPanel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Malgun Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Los Angeles Role Play";
            this.UnitProgressBar_Background.ResumeLayout(false);
            this.TotalProgressBar_Background.ResumeLayout(false);
            this.ButtonContainer2.ResumeLayout(false);
            this.ButtonContainer1.ResumeLayout(false);
            this.ButtonContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel UnitProgressBar_Background;
        private System.Windows.Forms.Panel UnitProgressBar;
        private System.Windows.Forms.Label PercentageLabel;
        private System.Windows.Forms.Panel TotalProgressBar_Background;
        private System.Windows.Forms.Panel TopPanel;
        private System.Windows.Forms.Panel LeftPanel;
        private System.Windows.Forms.Panel RightPanel;
        private System.Windows.Forms.Panel BottomPanel;
        private System.Windows.Forms.Label HeadLabel;
        private System.Windows.Forms.Panel TotalProgressBar;
        private System.ComponentModel.BackgroundWorker AntiCleoWorker;
        private System.ComponentModel.BackgroundWorker DownloadWorker;
        private System.Windows.Forms.Timer GameStart;
        private System.Windows.Forms.Timer GameExit;
        private System.Windows.Forms.Label Button_1_1;
        private System.Windows.Forms.Label Button_2_1;
        private System.Windows.Forms.Label Button_2_2;
        private System.Windows.Forms.Panel ButtonContainer1;
        private System.Windows.Forms.Panel ButtonContainer2;
        private System.Windows.Forms.Panel ButtonContainer3;
        private System.Windows.Forms.Label Button_3_1;
        private System.Windows.Forms.Label Button_3_3;
        private System.Windows.Forms.Label Button_3_2;
    }
}

