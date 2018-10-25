using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;
using Logic;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Threading;
using RedditSharp.Things;

namespace RedditBrowserTextUI
{
    class Program
    {
        private static SharedResources sharedResources = new SharedResources();        
        
        static void Main(string[] args)
        {
            string[] formats = { ".png", ".jpg" };
            sharedResources.manager = new Manager(formats);

            Console.WindowHeight = Console.LargestWindowHeight - 2;
            Console.WindowWidth = Console.LargestWindowWidth - 2;

            Application.Init();
            SetupGUI();
            Application.Run();
        }

        private static void SetupGUI()
        {
            // Display Frame's height and width is constant, for now.
            sharedResources.displayFrame = new DisplayFrame(new Rect(20, 0, Console.WindowWidth - 22, Console.WindowHeight - 4));

            CreateTop();            
            
            AddControls();

            Application.Iteration += Iteration_listener;
        }

        private static void Iteration_listener(object sender, EventArgs e)
        {
            if (sharedResources.itemToDisplay != null)
            {
                sharedResources.itemToDisplay.Display();
                Application.Refresh();
            }
        }        

        private static void CreateTop()
        {
            var top = Application.Top;

            // Creates the top-level window to show
            sharedResources.mainWindow = new Window(new Rect(0, 1, top.Frame.Width, top.Frame.Height - 1), "Reddit Browser");
            top.Add(sharedResources.mainWindow);

            // Creates a menubar, the item "New" has a help menu.
            var menu = new MenuBar(new MenuBarItem[] {
            new MenuBarItem ("_File", new MenuItem [] {
                new MenuItem ("_New", "Creates new file", null),
                new MenuItem ("_Close", "", null),
                new MenuItem ("_Quit", "", () => { top.Running = false; })
            }),
            new MenuBarItem ("_Edit", new MenuItem [] {
                new MenuItem ("_Copy", "", null),
                new MenuItem ("_Cut", "", null),
                new MenuItem ("_Paste", "", null)
                })
            });
            top.Add(menu);
        }

        private static void AddControls()
        {
            // TODO: Add viewing comments
            sharedResources.mainWindow.Add(
                                new Button(3, 2, "Load Sub") { Clicked = LoadSub_Clicked },
                                new Button(3, 4, "Next") { Clicked = Next_Clicked },
                                new Button(3, 6, "Previous") { Clicked = Previous_Clicked },
                                new Button(3, 8, "Save") { Clicked = Save_Clicked },
                                new Button(3, 10, "Copy Link") { Clicked = CopyLink_Clicked },
                                new Button(3, 12, "View Comments") { Clicked = ViewComments_Clicked }
                                );
        }

        private static void ViewComments_Clicked()
        {
            Window win = new Window("Top comments");
            win.Add(new Button(1, 1, "Back", false) { Clicked = Back_Clicked });
            const int itemsCount = 3;
            List<Comment> comments = sharedResources.manager.GetTopComments(itemsCount);

            for (int i = 0; i < itemsCount; i++)
            {
                Comment comment = comments[i];

                string commentContent = comment.Body;
                List<string> fragmentedComment = getFragmentedString(commentContent, Console.WindowWidth - 24 - 1);

                FrameView frame = new FrameView(new Rect(20, i * ((Console.WindowHeight - 2) / 3), Console.WindowWidth - 22, ((Console.WindowHeight - 2) / 3) - 1), comment.AuthorName);
                
                for (int j = 0; j < Math.Min(((Console.WindowHeight - 2) / 3) - 2, fragmentedComment.Count ); j++)
                {
                    frame.Add(
                        new Label(0, j, fragmentedComment[j])
                    );
                }

                win.Add(frame);
            }

            Application.Run(win);
        }

        private static List<string> getFragmentedString(string comment, int width)
        {
            List<string> fragments = new List<string>();
            for (int j=0; j<comment.Length; j++)
            {
                int charsToAdd = Math.Min(width, comment.Length - j);
                fragments.Add(comment.Substring(j, charsToAdd));
                j += charsToAdd;
            }
            return fragments;
        }

        private static void Back_Clicked()
        {
            Application.RequestStop();
        }

        private static void Save_Clicked()
        {
            SaveDialog dialog = new SaveDialog("Save", "Please choose a directory");
            dialog.DirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Application.Run(dialog);

            if (dialog.FileName != null)
            {
                string saveName = dialog.FileName.ToString();
                // TODO: save from the source.
            }
        }

        private static void LoadSub_Clicked()
        {
            sharedResources.mainWindow.Remove(sharedResources.displayFrame.ContentFrame);

            sharedResources.popupWindow = new Window(new Rect(22, 3, 50, 7), "Open Subreddit");
            sharedResources.targetSubTextView = new TextField(0, 2, 48, "ProgrammerHumor");
            sharedResources.popupWindow.Add(
                    new Label(0, 0, "Please input a sub"),
                    sharedResources.targetSubTextView,
                    new Button(5, 4, "OK", false) { Clicked = LoadSubOK_Clicked },
                    new Button(12, 4, "Cancel", true) { Clicked = LoadSubCancel_Clicked }
            );
            Application.Refresh();
            Application.Run(sharedResources.popupWindow);
        }

        private static void LoadSubOK_Clicked()
        {
            // Get name from popupWindow.
            string subName = sharedResources.targetSubTextView.Text.ToString();
            // Pass sub name to manager.
            

            // If a sub loads successfully.
            if (sharedResources.manager.SetSubreddit(subName))
            {
                // Draw the frame.
                sharedResources.mainWindow.Add(sharedResources.displayFrame.ContentFrame);
                sharedResources.manager.Next();
                LoadItem();
                sharedResources.itemToDisplay.Display();
            }
            // Else if a sub fails to load.
            else
            {
                // Return an error message to user interface,
                // as a dialog, perhaps?
            }

            sharedResources.popupWindow.Running = false;
        }

        private static void LoadSubCancel_Clicked()
        {
            // If a manager has a sub loaded and open.
            if (true)
            {
                // Draw the frame.
                sharedResources.mainWindow.Add(sharedResources.displayFrame.ContentFrame);
            }

            sharedResources.popupWindow.Running = false;
        }

        private static void Next_Clicked()
        {
            sharedResources.manager.Next();
            LoadItem();
            sharedResources.itemToDisplay.Display();
        }

        private static void Previous_Clicked()
        {
            sharedResources.manager.Previous();
            LoadItem();
            sharedResources.itemToDisplay.Display();
        }

        private static void CopyLink_Clicked()
        {
            Thread copyThread = new Thread(CopyLink);
            copyThread.IsBackground = true;
            copyThread.SetApartmentState(ApartmentState.STA);
            copyThread.Start();            
        }

        private static void CopyLink()
        {
            // Ask the manager for a link here
            System.Windows.Clipboard.SetText("Hello, clipboard");
        }
        
        private static void LoadItem()
        {
            Media media = sharedResources.manager.GetCurrentMedia();
            if (media.GetType() == typeof(MediaBitmapImage))
            {
                MediaBitmapImage image = (MediaBitmapImage)media;
                sharedResources.itemToDisplay = new ASCIIImage(sharedResources.displayFrame, image.Image);
            }            
            else if (media.GetType() == typeof(MediaGIF))
            {
                MediaGIF gif = (MediaGIF)media;
                sharedResources.itemToDisplay = new ASCIIGIF(sharedResources.displayFrame, gif.image);
            }
        }
    }
}
