﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        void DisplayNext();
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

        public void DisplayNext()
        {

        }
    }

    class ASCIIGIF : IDisplayable
    {
        private DisplayFrame displayFrame;
        private int delay;
        private int currentFrame;

        List<ASCIIImage> Contents = new List<ASCIIImage>();

        public ASCIIGIF(DisplayFrame frame, Image gif)
        {
            displayFrame = frame;

            FrameDimension dimension = new FrameDimension(gif.FrameDimensionsList[0]);
            int frameCount = gif.GetFrameCount(dimension);
            PropertyItem item = gif.GetPropertyItem(0x5100); // FrameDelay in libgdiplus
            delay = (item.Value[0] + item.Value[1] * 256) * 10;  // Time is in milliseconds

            currentFrame = -1;

            for (int index = 0; index < frameCount; index++)
            {
                gif.SelectActiveFrame(dimension, index);
                ASCIIImage gifFrame = new ASCIIImage(displayFrame, gif);
                Contents.Add(gifFrame);
            }
        }

        public void Display()
        {
            currentFrame = (currentFrame + 1) % Contents.Count;
            ASCIIImage frame = Contents[currentFrame];
            frame.Display();
            Task.Delay(delay).Wait();
        }

        public void DisplayNext()
        {
            currentFrame = (currentFrame + 1) % Contents.Count;
            ASCIIImage frame = Contents[currentFrame];
            frame.Display();
            Task.Delay(delay).Wait();
        }
    }
}
