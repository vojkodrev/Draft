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

        //public static PictureBox CreatePictureBox(Card pickPicture)
        //{
        //    PictureBox pictureBox = new PictureBox()
        //    {
        //        Image = pickPicture.Picture,
        //        Height = Convert.ToInt32(pickPicture.Picture.Height * 0.8),
        //        Width = Convert.ToInt32(pickPicture.Picture.Width * 0.8),
        //        SizeMode = PictureBoxSizeMode.Zoom,
        //        Tag = pickPicture,
        //    };

        //    return pictureBox;
        //}

        public static CardUserControl CreateCardUserControl(Card pickPicture, CardRatingsItem ratings)
        {
            CardUserControl cardUserControl = new CardUserControl();
            cardUserControl.Image = pickPicture.Picture;
            cardUserControl.Tag = pickPicture;

            if (ratings != null)
            {
                cardUserControl.Rating = ratings.Rating.ToString();
                cardUserControl.SetToolTip(ratings.RatingDescription);
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ChangeText(string text)
        {
            Text = String.Format("{0} - {1}", title, text);
        }

        private void draftSite_TimeLeftReceived(object sender, TimeLeftEventArgs e)
        {
            Invoke(new Action(() =>
            {
                timeLeft.Text = e.TimeLeft + "";
            }));
        }
        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Invoke(new Action(() =>
            {
                MessageBox.Show(e.Exception + Environment.NewLine + e.Exception.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }
        private void DraftListForm_Load(object sender, EventArgs e)
        {
            Application.ThreadException += Application_ThreadException;

            try
            {
                ratings = CardRatingsReader.Read("ratings");
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Unable to read ratings - {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
                        
            SiteSelectorForm siteSelectorForm = new SiteSelectorForm();
            if (siteSelectorForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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

            draftSite.CurrentPickReceived += draftSite_CurrentPickReceived;
            draftSite.GetCurrentPicksError += draftSite_GetCurrentPicksError;
            draftSite.GetCurrentPicksFinished += draftSite_GetCurrentPicksFinished;
            draftSite.GetCurrentPicksStarted += draftSite_GetCurrentPicksStarted;
            draftSite.TimeLeftReceived += draftSite_TimeLeftReceived;

            deckEditorForm = new DeckEditorForm(draftSite, ratings);
            deckEditorForm.Show();

            title = Text;

            Show();
            loginToolStripMenuItem_Click(sender, e);
        }
        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            if (loginForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                try
                {
                    draftSite.Login(loginForm.Username.Text, loginForm.Password.Text);
                    FillCurrentPicks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }
        private void draftPickPicture_Click(object sender, EventArgs e)
        {
            try
            {
                CardUserControl cardUserControl = (CardUserControl)sender;
                Card pickPicture = (Card)cardUserControl.Tag;

                if (MessageBox.Show("Do you realy want to pick this picture?", "Pick?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    draftSite.PickCard(pickPicture.Id);
                    deckEditorForm.AddCard(pickPicture);
                    FillCurrentPicks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        public static CardRatingsItem GetCardRatingsItem(Dictionary<string, CardRatingsItem> ratings, Card c)
        {
            CardRatingsItem cardRatingsItem;
            if (ratings.ContainsKey(c.Name))
                cardRatingsItem = ratings[c.Name];
            else
            {
                TH.Message(String.Format("Unable to find ratings for card {0}.", c.Name));
                cardRatingsItem = null;
            }

            return cardRatingsItem;
        }

        private void draftSite_CurrentPickReceived(object sender, CardEventArgs e)
        {
            Invoke(new Action(() =>
            {
                CardUserControl cardUserControl = CreateCardUserControl(e.Card, GetCardRatingsItem(ratings, e.Card));

                cardUserControl.Click += draftPickPicture_Click;

                draftPickPicturesPanel.Controls.Add(cardUserControl);
            }));
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
