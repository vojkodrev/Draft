using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Draft
{
    public partial class CardUserControl : UserControl
    {
        public CardUserControl()
        {
            InitializeComponent();
        }

        public Image Image
        {
            get
            {
                return picture.Image;
            }

            set
            {
                picture.Image = value;
            }
        }
        public string Rating
        {
            get
            {
                return rating.Text;
            }

            set
            {
                rating.Text = value;
            }
        }

        public void SetToolTip(string tooltip)
        {
            string twn = "";
            int counter = 0;
            foreach (char item in tooltip)
            {
                if (counter != 0 && counter % 50 == 0)
                    twn += Environment.NewLine;

                twn += item;

                counter++;
            }

            new ToolTip() { ShowAlways = true }.SetToolTip(picture, twn);
        }

        private void CardUserControl_Load(object sender, EventArgs e)
        {
            Height = rating.Top + rating.Height;
            Width = picture.Width;

            foreach (Control control in Controls)
            {
                control.Click += (cs, ce) => OnClick(ce);
                control.MouseMove += (cs, ce) => OnMouseMove(ce);
                control.MouseDown += (cs, ce) => OnMouseDown(ce);
                control.MouseUp += (cs, ce) => OnMouseUp(ce);                
            }
        }
    }
}
