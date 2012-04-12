namespace Draft
{
    partial class DraftListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.cCGDecksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshDraftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.draftPickPicturesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.timeLeft = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cCGDecksToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1060, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // cCGDecksToolStripMenuItem
            // 
            this.cCGDecksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginToolStripMenuItem,
            this.refreshDraftToolStripMenuItem});
            this.cCGDecksToolStripMenuItem.Name = "cCGDecksToolStripMenuItem";
            this.cCGDecksToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.cCGDecksToolStripMenuItem.Text = "Draft Site";
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.loginToolStripMenuItem.Text = "Login";
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.loginToolStripMenuItem_Click);
            // 
            // refreshDraftToolStripMenuItem
            // 
            this.refreshDraftToolStripMenuItem.Name = "refreshDraftToolStripMenuItem";
            this.refreshDraftToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshDraftToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.refreshDraftToolStripMenuItem.Text = "Refresh Draft";
            this.refreshDraftToolStripMenuItem.Click += new System.EventHandler(this.refreshDraftToolStripMenuItem_Click);
            // 
            // draftPickPicturesPanel
            // 
            this.draftPickPicturesPanel.AutoScroll = true;
            this.draftPickPicturesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.draftPickPicturesPanel.Location = new System.Drawing.Point(3, 23);
            this.draftPickPicturesPanel.Name = "draftPickPicturesPanel";
            this.draftPickPicturesPanel.Size = new System.Drawing.Size(1054, 513);
            this.draftPickPicturesPanel.TabIndex = 2;
            // 
            // timeLeft
            // 
            this.timeLeft.AutoSize = true;
            this.timeLeft.Location = new System.Drawing.Point(3, 0);
            this.timeLeft.Name = "timeLeft";
            this.timeLeft.Size = new System.Drawing.Size(0, 13);
            this.timeLeft.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.timeLeft, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.draftPickPicturesPanel, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1060, 539);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // DraftListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 563);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DraftListForm";
            this.Text = "DraftList";
            this.Load += new System.EventHandler(this.DraftListForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cCGDecksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel draftPickPicturesPanel;
        private System.Windows.Forms.ToolStripMenuItem refreshDraftToolStripMenuItem;
        private System.Windows.Forms.Label timeLeft;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Timer timer1;
    }
}