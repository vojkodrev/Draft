namespace Draft
{
    partial class DeckEditorForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.cCGDecksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshPicksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plainsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.swampToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mountainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.islandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.orderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cardsPanel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cCGDecksToolStripMenuItem,
            this.addToolStripMenuItem,
            this.deckToolStripMenuItem,
            this.orderToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(919, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // cCGDecksToolStripMenuItem
            // 
            this.cCGDecksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshPicksToolStripMenuItem});
            this.cCGDecksToolStripMenuItem.Name = "cCGDecksToolStripMenuItem";
            this.cCGDecksToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.cCGDecksToolStripMenuItem.Text = "Draft Site";
            // 
            // refreshPicksToolStripMenuItem
            // 
            this.refreshPicksToolStripMenuItem.Name = "refreshPicksToolStripMenuItem";
            this.refreshPicksToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshPicksToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.refreshPicksToolStripMenuItem.Text = "Refresh picks";
            this.refreshPicksToolStripMenuItem.Click += new System.EventHandler(this.refreshPicksToolStripMenuItem_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.forestToolStripMenuItem,
            this.plainsToolStripMenuItem,
            this.swampToolStripMenuItem,
            this.mountainToolStripMenuItem,
            this.islandToolStripMenuItem});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // forestToolStripMenuItem
            // 
            this.forestToolStripMenuItem.Name = "forestToolStripMenuItem";
            this.forestToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.forestToolStripMenuItem.Text = "Forest";
            this.forestToolStripMenuItem.Click += new System.EventHandler(this.forestToolStripMenuItem_Click);
            // 
            // plainsToolStripMenuItem
            // 
            this.plainsToolStripMenuItem.Name = "plainsToolStripMenuItem";
            this.plainsToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.plainsToolStripMenuItem.Text = "Plains";
            this.plainsToolStripMenuItem.Click += new System.EventHandler(this.plainsToolStripMenuItem_Click);
            // 
            // swampToolStripMenuItem
            // 
            this.swampToolStripMenuItem.Name = "swampToolStripMenuItem";
            this.swampToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.swampToolStripMenuItem.Text = "Swamp";
            this.swampToolStripMenuItem.Click += new System.EventHandler(this.swampToolStripMenuItem_Click);
            // 
            // mountainToolStripMenuItem
            // 
            this.mountainToolStripMenuItem.Name = "mountainToolStripMenuItem";
            this.mountainToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.mountainToolStripMenuItem.Text = "Mountain";
            this.mountainToolStripMenuItem.Click += new System.EventHandler(this.mountainToolStripMenuItem_Click);
            // 
            // islandToolStripMenuItem
            // 
            this.islandToolStripMenuItem.Name = "islandToolStripMenuItem";
            this.islandToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.islandToolStripMenuItem.Text = "Island";
            this.islandToolStripMenuItem.Click += new System.EventHandler(this.islandToolStripMenuItem_Click);
            // 
            // deckToolStripMenuItem
            // 
            this.deckToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.deckToolStripMenuItem.Name = "deckToolStripMenuItem";
            this.deckToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.deckToolStripMenuItem.Text = "Deck";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveDeckToolStripMenuItem_Click);
            // 
            // orderToolStripMenuItem
            // 
            this.orderToolStripMenuItem.Name = "orderToolStripMenuItem";
            this.orderToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.orderToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.orderToolStripMenuItem.Text = "Order";
            this.orderToolStripMenuItem.Click += new System.EventHandler(this.orderToolStripMenuItem_Click);
            // 
            // cardsPanel
            // 
            this.cardsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardsPanel.Location = new System.Drawing.Point(0, 24);
            this.cardsPanel.Name = "cardsPanel";
            this.cardsPanel.Size = new System.Drawing.Size(919, 504);
            this.cardsPanel.TabIndex = 2;
            this.cardsPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.cardsPanel_Paint);
            // 
            // DeckEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 528);
            this.Controls.Add(this.cardsPanel);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DeckEditorForm";
            this.Text = "DeckEditor";
            this.Activated += new System.EventHandler(this.DeckEditorForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DeckEditorForm_FormClosing);
            this.Resize += new System.EventHandler(this.DeckEditorForm_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cCGDecksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshPicksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plainsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem swampToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mountainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem islandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.Panel cardsPanel;
        private System.Windows.Forms.ToolStripMenuItem orderToolStripMenuItem;
    }
}