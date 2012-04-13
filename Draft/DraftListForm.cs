using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Draft.DraftSites;
using System.Linq;
using Draft.DraftSites.TappedOut;
using System.Threading;
using Draft.DraftSites.CCGDecks;
using CardRatings;
using Helpers.Diagnostics;
using Helpers.Forms;

namespace Draft
{
    public partial class DraftListForm : Form
    {
        private DeckEditorForm deckEditorForm;
        private IDraftSite draftSite;
        private string title;
        Dictionary<string, CardRatingsItem> ratings = new Dictionary<string, CardRatingsItem>();

        public DraftListForm()
        {
            InitializeComponent();
        }

        public static CardUserControl CreateCardUserControl(Card pickPicture, CardRatingsItem ratings)
        {
            CardUserControl cardUserControl = new CardUserControl();
            cardUserControl.Image = pickPicture.Picture;
            cardUserControl.Tag = pickPicture;

            if (ratings != null)
            {
                cardUserControl.Rating = ratings.Rating.ToString();
                cardUserControl.SetToolTip(String.Format("{0} {1}", ratings.Rating, ratings.RatingDescription));
            }

            return cardUserControl;
        }
        private void FillCurrentPicks()
        {
            try
            {
                draftPickPicturesPanel.Controls.Clear();

                new Thread(draftSite.GetCurrentPicks) { IsBackground = true }.Start();
            }
            catch (Exception ex)
            {
                FormsHelper.ShowExceptionInfo("Unable to get current picks!", ex);
            }
        }
        private void ChangeText(string text)
        {
            Text = String.Format("{0} - {1}", title, text);
        }
        public static CardRatingsItem GetCardRatingsItem(Dictionary<string, CardRatingsItem> ratings, Card c)
        {
            CardRatingsItem cardRatingsItem;
            if (ratings.ContainsKey(c.Name))
                cardRatingsItem = ratings[c.Name];
            else
            {
                TH.Warning(String.Format("Unable to find ratings for card {0}.", c.Name));
                cardRatingsItem = null;
            }

            return cardRatingsItem;
        }

        private void draftSite_TimeLeftReceived(object sender, TimeLeftEventArgs e)
        {
            Invoke(new Action(() =>
            {
                timeLeft.Text = e.TimeLeft + "";
            }));
        }
        private void DraftListForm_Load(object sender, EventArgs e)
        {
            try
            {
                Show();

                try
                {
                    ratings = CardRatingsReader.Read("ratings");
                }
                catch (Exception ex)
                {
                    FormsHelper.ShowExceptionInfo("Unable to read ratings!", ex);
                }

                using (SiteSelectorForm siteSelectorForm = new SiteSelectorForm())
                {
                    siteSelectorForm.ShowDialog();
                    switch (siteSelectorForm.DraftSite)
                    {
                        case SiteSelectorForm.SelectedSite.CCGDecks:
                            draftSite = new CCGDecksDraftSite();
                            break;
                        case SiteSelectorForm.SelectedSite.Tappedout:
                            draftSite = new TappedOutDraftSite();
                            break;
                        default:
                            throw new NotSupportedException("Draft List - Unsupported draft site");
                    }
                }

                draftSite.CurrentPickReceived += draftSite_CurrentPickReceived;
                draftSite.GetCurrentPicksError += draftSite_GetCurrentPicksError;
                draftSite.GetCurrentPicksFinished += draftSite_GetCurrentPicksFinished;
                draftSite.GetCurrentPicksStarted += draftSite_GetCurrentPicksStarted;
                draftSite.TimeLeftReceived += draftSite_TimeLeftReceived;

                deckEditorForm = new DeckEditorForm(draftSite, ratings);
                deckEditorForm.Show();

                title = Text;

                loginToolStripMenuItem_Click(sender, e);
            }
            catch (Exception ex)
            {
                FormsHelper.ShowExceptionInfo("Exception in form load!", ex);
                throw;
            }
        }
        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (LoginForm loginForm = new LoginForm())
                    if (loginForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        draftSite.Login(loginForm.Username.Text, loginForm.Password.Text);
                        FillCurrentPicks();
                    }
            }
            catch (Exception ex)
            {
                FormsHelper.ShowExceptionInfo("Unable to login!", ex);
            }
        }
        private void refreshDraftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FillCurrentPicks();
        }
        private void draftSite_GetCurrentPicksStarted(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                refreshDraftToolStripMenuItem.Enabled = false;
                ChangeText("Loading picks");
            }));
        }
        private void draftSite_GetCurrentPicksFinished(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                refreshDraftToolStripMenuItem.Enabled = true;

                ChangeText("Finished loading picks");
            }));
        }
        private void draftSite_GetCurrentPicksError(object sender, ErrorEventArgs e)
        {
            Invoke(new Action(() =>
            {
                MessageBox.Show(e.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }        
        private void draftSite_CurrentPickReceived(object sender, CardEventArgs e)
        {
            Invoke(new Action(() =>
            {
                CardUserControl cardUserControl = CreateCardUserControl(e.Card, GetCardRatingsItem(ratings, e.Card));

                cardUserControl.MouseUp += cardUserControl_MouseUp;

                draftPickPicturesPanel.Controls.Add(cardUserControl);
            }));
        }
        void cardUserControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Control control = (Control)sender;
                control.Parent.Controls.SetChildIndex(control, 0);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                try
                {
                    CardUserControl cardUserControl = (CardUserControl)sender;
                    Card pickPicture = (Card)cardUserControl.Tag;

                    if (MessageBox.Show("Do you realy want to pick this card?", "Pick?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        draftSite.PickCard(pickPicture.Id);
                        deckEditorForm.AddCard(pickPicture);
                        FillCurrentPicks();
                    }
                }
                catch (Exception ex)
                {
                    FormsHelper.ShowExceptionInfo("Unable to pick the card!", ex);
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                int tl = Convert.ToInt32(timeLeft.Text);
                if (tl > 0)
                    timeLeft.Text = tl - 1 + "";
            }
            catch (Exception)
            {

            }
        }
    }
}
