using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Draft
{
    public partial class SiteSelectorForm : Form
    {
        public enum SelectedSite
        {
            CCGDecks,
            Tappedout,           
        }

        public SiteSelectorForm()
        {
            InitializeComponent();

            DraftSite = SiteSelectorForm.SelectedSite.CCGDecks;
        }

        public SelectedSite DraftSite { get; private set; }

        private void selectCCGDecks_Click(object sender, EventArgs e)
        {
            DraftSite = SelectedSite.CCGDecks;
            Close();
        }
        private void selectTappedout_Click(object sender, EventArgs e)
        {
            DraftSite = SelectedSite.Tappedout;
            Close();
        }
    }
}
