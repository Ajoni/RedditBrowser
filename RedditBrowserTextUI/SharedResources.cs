using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RedditBrowserTextUI
{
    // A class for resources shared between Tasks
    class SharedResources
    {
        public DisplayFrame displayFrame;
        public Window mainWindow;
        public Window popupWindow;
        public TextField targetSubTextView;

        public Task displayTask = null;

        public Manager manager;

        public IDisplayable itemToDisplay = null;
    }
}
