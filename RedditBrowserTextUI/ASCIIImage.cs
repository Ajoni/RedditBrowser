using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Terminal.Gui;

namespace RedditBrowserTextUI
{
    class ASCIIImage : IDisplayable
    {
        private DisplayFrame displayFrame;

        private List<string> Contents;

        public ASCIIImage(DisplayFrame frame, BitmapImage image)
        {
            displayFrame = frame;
            // Frame size dictates size to which an image is shrunk or stretched to.
            int height = displayFrame.ContentHeight;
            int width = displayFrame.ContentWidth;

            Bitmap bitmap = Helpers.BitmapImage2Bitmap(image);
            Bitmap resizedBitmap = new Bitmap(bitmap, new System.Drawing.Size(width, height));
            Contents = Helpers.ConvertToAscii(resizedBitmap);
        }

        public ASCIIImage(DisplayFrame frame, Image image)
        {
            displayFrame = frame;

            int height = displayFrame.ContentHeight;
            int width = displayFrame.ContentWidth;

            Bitmap bitmap = new Bitmap(image);
            Bitmap resizedBitmap = new Bitmap(bitmap, new System.Drawing.Size(width, height));
            Contents = Helpers.ConvertToAscii(resizedBitmap);
        }

        public void Display()
        {
            for (int i = 0; i < displayFrame.Rows.Count; i++)
            {
                displayFrame.Rows[i].Text = Contents[i];
            }
        }
    }
}
