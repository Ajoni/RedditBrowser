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
        public DisplayFrame DisplayFrame;
        public Window MainWindow;
        public Window PopupWindow;
        public TextField TargetSubTextView;
        public Label SubBeingLoaded;
        public String SubredditName = "ProgrammerHumor";

        public Manager Manager;

        public IDisplayable ItemToDisplay = null;
    }
}
