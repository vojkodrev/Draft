namespace Draft
{
    partial class SiteSelectorForm
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
            this.selectCCGDecks = new System.Windows.Forms.Button();
            this.selectTappedout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // selectCCGDecks
            // 
            this.selectCCGDecks.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.selectCCGDecks.Location = new System.Drawing.Point(12, 12);
            this.selectCCGDecks.Name = "selectCCGDecks";
            this.selectCCGDecks.Size = new System.Drawing.Size(302, 63);
            this.selectCCGDecks.TabIndex = 0;
            this.selectCCGDecks.Text = "CCGDecks";
            this.selectCCGDecks.UseVisualStyleBackColor = true;
            this.selectCCGDecks.Click += new System.EventHandler(this.selectCCGDecks_Click);
            // 
            // selectTappedout
            // 
            this.selectTappedout.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.selectTappedout.Location = new System.Drawing.Point(12, 81);
            this.selectTappedout.Name = "selectTappedout";
            this.selectTappedout.Size = new System.Drawing.Size(302, 63);
            this.selectTappedout.TabIndex = 1;
            this.selectTappedout.Text = "Tappedout";
            this.selectTappedout.UseVisualStyleBackColor = true;
            this.selectTappedout.Click += new System.EventHandler(this.selectTappedout_Click);
            // 
            // SiteSelectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 164);
            this.Controls.Add(this.selectTappedout);
            this.Controls.Add(this.selectCCGDecks);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SiteSelectorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Draft Site";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button selectCCGDecks;
        private System.Windows.Forms.Button selectTappedout;
    }
}