using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Terminal.Gui;

namespace RedditBrowserTextUI
{
    interface IDisplayable
    {
        void Display();
    }

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

        public void Display()
        {
            for (int i = 0; i < displayFrame.Rows.Count; i++)
            {
                displayFrame.Rows[i].Text = Contents[i];
            }
        }
    }

    class ASCIIGIF : IDisplayable
    {
        private DisplayFrame displayFrame;
                
        // List<ASCIIFrame> Contents

        public ASCIIGIF(DisplayFrame frame)
        {
            displayFrame = frame;
            // TODO Create Contents
        }

        public void Display()
        {
            throw new NotImplementedException();
        }
    }
}
