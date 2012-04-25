using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Draft.DraftSites;
using Draft.Decks;
using System.Threading;
using Helpers.Pictures;
using CardRatings;
using Helpers.Forms;

namespace Draft
{
    public partial class DeckEditorForm : Form
    {
        private Point coordinates;
        private readonly IDraftSite draftSite;
        private readonly Dictionary<string, CardRatingsItem> ratings;
        private readonly string title;

        public DeckEditorForm(IDraftSite draftSite, Dictionary<string, CardRatingsItem> ratings)
        {
            InitializeComponent();

            this.ratings = ratings;
            this.draftSite = draftSite;
            this.draftSite.GetPickedCardsError += draftSite_GetPickedCardsError;
            this.draftSite.GetPickedCardsFinished += draftSite_GetPickedCardsFinished;
            this.draftSite.GetPickedCardsStarted += draftSite_GetPickedCardsStarted;
            this.draftSite.PickedCardReceived += draftSite_PickedCardReceived;

            title = Text;
        }

        public void AddCard(Card picture)
        {
            CardUserControl cardUserControl = DraftListForm.CreateCardUserControl(picture, DraftListForm.GetCardRatingsItem(ratings, picture));
            AddCardUserControlEvents(cardUserControl);

            cardsPanel.Controls.Add(cardUserControl);

            cardUserControl.BringToFront();

            orderToolStripMenuItem_Click(null, null);
        }
        public void MassAdd(string name, int count)
        {
            Bitmap pic = DownloadPicture(name);
            for (int i = 0; i < count; i++)
                AddCard(new Card { Picture = pic, Name = name });
        }
        private void AddCardUserControlEvents(CardUserControl cardUserControl)
        {
            cardUserControl.Click += movableControl_Click;
            cardUserControl.MouseDown += movableControl_MouseDown;
            cardUserControl.MouseMove += movableControl_MouseMove;
            cardUserControl.MouseUp += movableControl_MouseUp;
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
        private void SetStatistics()
        {
            Text = "Cards in deck: " + GetCardsInDeck().Count();
        }
        private static void NormalizePosition(Control c)
        {
            c.Left = c.Left - (c.Left % c.Width);
        }
        private int GetSplitLineY()
        {
            return Convert.ToInt32(cardsPanel.Height * 0.4);
        }
        private int GetSplitLineY2()
        {
            return Convert.ToInt32(cardsPanel.Height * 0.8);
        }
        private IEnumerable<Control> GetCardsNotInDeck()
        {
            return CardsPanelControlsToArray().Where(i => !CardInDeck(i));
        }
        private bool CardInDeck(Control i)
        {
            return i.Top > GetSplitLineY();
        }
        private Control[] CardsPanelControlsToArray()
        {
            Control[] controlsArray = new Control[cardsPanel.Controls.Count];
            cardsPanel.Controls.CopyTo(controlsArray, 0);
            return controlsArray;
        }
        private IEnumerable<Control> GetCardsInDeck()
        {
            return CardsPanelControlsToArray().Where(i => CardInDeck(i));
        }
        private static void OrderCards(IEnumerable<Control> controls, int top)
        {
            foreach (IGrouping<int, Control> grouping in controls.GroupBy(i => i.Left))
            {
                List<Control> groupingList = grouping.OrderBy(i => i.Top).ToList();
                for (int i = 0; i < groupingList.Count; i++)
                {
                    Control control = groupingList[i];
                    control.Top = top + i * 25;
                    control.BringToFront();
                }
            }
        }

        private void movableControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                NormalizePosition((Control)sender);

                SetStatistics();

                orderToolStripMenuItem_Click(null, null);
            }
        }
        private void cardsPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = cardsPanel.CreateGraphics();
            graphics.Clear(BackColor);

            int y = GetSplitLineY();
            graphics.DrawLine(new Pen(Color.Black), new Point(0, y), new Point(cardsPanel.Width, y));

            int y2 = GetSplitLineY2();
            graphics.DrawLine(new Pen(Color.Black), new Point(0, y2), new Point(cardsPanel.Width, y2));
        }
        private void DeckEditorForm_Resize(object sender, EventArgs e)
        {
            cardsPanel_Paint(null, null);
        }
        private void movableControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                coordinates = new Point(-e.X, -e.Y);
                cardsPanel.Controls.SetChildIndex((Control)sender, 0);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                ((Control)sender).SendToBack();
        }
        private void movableControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pointToClient = PointToClient(MousePosition);
                pointToClient.Offset(coordinates);
                ((Control)sender).Location = pointToClient;
            }
        }
        private void movableControl_Click(object sender, EventArgs e)
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
                FormsHelper.ShowExceptionInfo("Unable to get picked cards!", ex);
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
            try
            {
                MassAdd("Forest", 5);
            }
            catch (Exception ex)
            {
                FormsHelper.ShowExceptionInfo("Unable to get forests!", ex);
            }
        }
        private void plainsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MassAdd("Plains", 5);
            }
            catch (Exception ex)
            {
                FormsHelper.ShowExceptionInfo("Unable to get plains!", ex);
            }
        }
        private void swampToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MassAdd("Swamp", 5);
            }
            catch (Exception ex)
            {
                FormsHelper.ShowExceptionInfo("Unable to get swamps!", ex);
            }
        }
        private void mountainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MassAdd("Mountain", 5);
            }
            catch (Exception ex)
            {
                FormsHelper.ShowExceptionInfo("Unable to get mountains!", ex);
            }
        }
        private void islandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MassAdd("Island", 5);
            }
            catch (Exception ex)
            {
                FormsHelper.ShowExceptionInfo("Unable to get islands!", ex);    
            }
        }
        private void saveDeckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Deck deck = new Deck();

                deck.Cards.AddRange(GetCardsInDeck().Select(i => ((Card)i.Tag).Name));
                deck.SideboardCards.AddRange(GetCardsNotInDeck().Select(i => ((Card)i.Tag).Name));

                string deckText = deck.Generate();
                if (!string.IsNullOrEmpty(deckText))
                    Clipboard.SetText(deckText);
            }
            catch (Exception ex)
            {
                FormsHelper.ShowExceptionInfo("Unable to save deck!", ex);
            }
        }
        private void orderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OrderCards(GetCardsNotInDeck(), 15);
                OrderCards(GetCardsInDeck().Where(i => i.Top < GetSplitLineY2()), GetSplitLineY() + 5);
                OrderCards(GetCardsInDeck().Where(i => i.Top >= GetSplitLineY2()), GetSplitLineY2() + 5);
            }
            catch (Exception ex)
            {
                FormsHelper.ShowExceptionInfo("Unable to order cards!", ex);
            }
        }
        private void DeckEditorForm_Activated(object sender, EventArgs e)
        {
            cardsPanel_Paint(null, null);
        }       
    }
}
