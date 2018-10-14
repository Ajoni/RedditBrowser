using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RedditBrowserTextUI
{
    class DisplayFrame
    {
        public FrameView ContentFrame;
        public List<Label> Rows;
        public int ContentHeight;
        public int ContentWidth;

        public DisplayFrame(Rect FrameViewInfo)
        {
            ContentFrame = new FrameView(FrameViewInfo, null);

            // Content has 2 characters less to fit in, due to the frame being drawn.
            ContentHeight = FrameViewInfo.Height - 2;
            ContentWidth = FrameViewInfo.Width - 2;

            string WhiteSpacedString = new string('-', ContentWidth);

            Rows = new List<Label>();
            for (int i=0; i < FrameViewInfo.Height - 2; i++)
            {
                Label label = new Label(0, i, String.Copy(WhiteSpacedString));
                Rows.Add(label);
                ContentFrame.Add(label);
            }            
        }
    }
}
