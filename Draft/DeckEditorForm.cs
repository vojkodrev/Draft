using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Draft.DraftSites;
using Draft.Http;
using Draft.Decks;
using System.Threading;
using Draft.Pictures;

namespace Draft
{
    public partial class DeckEditorForm : Form
    {
        private Point coordinates;
        private readonly IDraftSite draftSite;
        private readonly string title;

        public DeckEditorForm(IDraftSite draftSite)
        {
            this.draftSite = draftSite;
            this.draftSite.GetPickedCardsError += draftSite_GetPickedCardsError;
            this.draftSite.GetPickedCardsFinished += draftSite_GetPickedCardsFinished;
            this.draftSite.GetPickedCardsStarted += draftSite_GetPickedCardsStarted;
            this.draftSite.PickedCardReceived += draftSite_PickedCardReceived;

            InitializeComponent();

            title = Text;
        }

        public void AddCard(Card pickPicture)
        {
            PictureBox pictureBox = DraftListForm.CreatePictureBox(pickPicture);
            AddPictureBoxEvents(pictureBox);


            cardsPanel.Controls.Add(pictureBox);

            pictureBox.BringToFront();
        }
        public void MassAdd(string name, int count)
        {
            Bitmap pic = DownloadPicture(name);
            for (int i = 0; i < count; i++)
                AddCard(new Card { Picture = pic, Name = name });
        }

        private void AddPictureBoxEvents(PictureBox pictureBox)
        {
            pictureBox.Click += pictureBox_Click;
            pictureBox.MouseDown += pictureBox_MouseDown;
            pictureBox.MouseMove += pictureBox_MouseMove;
            pictureBox.MouseUp += pictureBox_MouseUp;
        }
        private static Bitmap DownloadPicture(string cardName)
        {
            //Bitmap picture = HttpHelper.DownloadPicture(String.Format("http://www.wizards.com/global/images/magic/general/{0}.jpg", cardName.Replace(" ", "_")));
            //return picture;

            return PictureCache.GetPicture(cardName, String.Format("http://www.wizards.com/global/images/magic/general/{0}.jpg", cardName.Replace(" ", "_")));
        }
        private void ChangeText(string text)
        {
            Text = String.Format("{0} - {1}", title, text);
        }
        
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            SetStatistics();
        }
        private void SetStatistics()
        {
            Control.ControlCollection controls = cardsPanel.Controls;
            int counter = 0;
            foreach (Control control in controls)
                if (control.Top > (Height / 2))
                    counter++;

            Text = "Cards in deck: " + counter;
        }
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            coordinates = new Point(-e.X, -e.Y);
            cardsPanel.Controls.SetChildIndex((Control)sender, 0);
        }
        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pointToClient = PointToClient(MousePosition);
                pointToClient.Offset(coordinates);
                ((Control)sender).Location = pointToClient;
            }
        }
        private void pictureBox_Click(object sender, EventArgs e)
        {
            cardsPanel.Controls.SetChildIndex((Control)sender, 0);
        }
        private void DeckEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }        
        private void refreshPicksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                cardsPanel.Controls.Clear();

                new Thread(draftSite.GetPickedCards) { IsBackground = true }.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void draftSite_PickedCardReceived(object sender, CardEventArgs e)
        {
            Invoke(new Action(() =>
            {
                AddCard(e.Card);
            }));
        }
        private void draftSite_GetPickedCardsStarted(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                refreshPicksToolStripMenuItem.Enabled = false;
                ChangeText("Fetching picked cards");
            }));
        }
        private void draftSite_GetPickedCardsFinished(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                refreshPicksToolStripMenuItem.Enabled = true;
                ChangeText("Finished fetching picked cards");
            }));
        }
        private void draftSite_GetPickedCardsError(object sender, ErrorEventArgs e)
        {
            Invoke(new Action(() =>
            {
                MessageBox.Show(e.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }        
        private void forestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MassAdd("Forest", 5);
        }
        private void plainsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MassAdd("Plains", 5);
        }
        private void swampToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MassAdd("Swamp", 5);
        }
        private void mountainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MassAdd("Mountain", 5);
        }
        private void islandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MassAdd("Island", 5);
        }
        private void saveDeckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control.ControlCollection controls = cardsPanel.Controls;
            Deck deck = new Deck();

            foreach (Control control in controls)
                if (control.GetType() == typeof(PictureBox))
                {
                    Card pickPicture = (Card)((PictureBox)control).Tag;

                    if (control.Top > (Height / 2))
                        deck.Cards.Add(pickPicture.Name);
                    else
                        deck.SideboardCards.Add(pickPicture.Name);
                }

            Clipboard.SetText(deck.Generate());
        }
    }
}
