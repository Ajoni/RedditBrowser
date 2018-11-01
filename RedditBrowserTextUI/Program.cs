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
using System.Net;
using System.IO;

namespace RedditBrowserTextUI
{
    class Program
    {
        private static SharedResources sharedResources = new SharedResources();        
        
        static void Main(string[] args)
        {
            string[] formats = { ".png", ".jpg" };
            sharedResources.Manager = new Manager(formats);

            Console.WindowHeight = Console.LargestWindowHeight - 2;
            Console.WindowWidth = Console.LargestWindowWidth - 2;

            Application.Init();
            SetupGUI();
            Application.Run();
        }

        private static void SetupGUI()
        {
            // Display Frame's height and width is constant, for now.
            sharedResources.DisplayFrame = new DisplayFrame(new Rect(20, 0, Console.WindowWidth - 22, Console.WindowHeight - 4));

            CreateTop();            
            
            AddControls();
        }

        private static void CreateTop()
        {
            var top = Application.Top;

            // Creates the top-level window to show
            sharedResources.MainWindow = new Window(new Rect(0, 1, top.Frame.Width, top.Frame.Height - 1), "Reddit Browser");
            top.Add(sharedResources.MainWindow);

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
            sharedResources.MainWindow.Add(
                                new Button(2, 2, "Load Sub") { Clicked = LoadSub_Clicked },
                                new Button(2, 4, "Next") { Clicked = Next_Clicked },
                                new Button(2, 6, "Previous") { Clicked = Previous_Clicked },
                                new Button(2, 8, "Save") { Clicked = Save_Clicked },
                                new Button(2, 10, "Copy Link") { Clicked = CopyLink_Clicked },
                                new Button(2, 12, "View Comments") { Clicked = ViewComments_Clicked }
                                );
        }
                
        private static List<string> getFragmentedString(string comment, int width)
        {
            List<string> fragments = new List<string>();
            for (int j=0; j<comment.Length; j++)
            {
                int charsToAdd = Math.Min(width, comment.Length - j);
                if (j < comment.IndexOf('\n', j) && comment.IndexOf('\n', j) < j + charsToAdd)
                {
                    charsToAdd = comment.IndexOf('\n', j) - j;
                }
                fragments.Add(comment.Substring(j, charsToAdd));
                j += charsToAdd;
                if (j < comment.Length - 1)
                {
                    while (comment[j + 1] == '\n')
                    {
                        j++;
                    }
                }
            }
            return fragments;
        }

        private static void Back_Clicked()
        {
            Application.RequestStop();
        }

        private static void Save_Clicked()
        {
            if (sharedResources.Manager.SubredditIsOpen())
            {
                SaveDialog dialog = new SaveDialog("Save", "Please choose a directory");
                dialog.DirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                Application.Run(dialog);

                if (dialog.FileName != null)
                {
                    sharedResources.CanSaveFlag = true;

                    string folderName = dialog.DirectoryPath.ToString();
                    string saveName = folderName + @"\" + dialog.FileName.ToString();                    
                    if (File.Exists(saveName))
                    {
                        // Depending on the button pressed, CanSaveFlag will be modified
                        Dialog overwriteDialog = new Dialog("Do you want to overwrite existing file?", 46, 6,
                            new Button(10, 8,"OK", true) { Clicked = OverwriteOK_Clicked },
                            new Button(20, 8, "Cancel", false) { Clicked = OverwriteCancel_Clicked }
                            );
                        Application.Run(overwriteDialog);
                    }
                    if (sharedResources.CanSaveFlag)
                    {
                        sharedResources.Manager.GetCurrentMedia().Save(saveName);
                        Dialog saveSuccessful = new Dialog("Save successful", 21, 6, new Button("OK", true) { Clicked = DialogOK_Clicked });
                        Application.Run(saveSuccessful);
                    }
                }
            }
        }

        private static void OverwriteCancel_Clicked()
        {
            sharedResources.CanSaveFlag = false;
            Application.RequestStop();
        }

        private static void OverwriteOK_Clicked()
        {
            sharedResources.CanSaveFlag = true;
            Application.RequestStop();
        }

        private static void LoadSub_Clicked()
        {
            sharedResources.MainWindow.Remove(sharedResources.DisplayFrame.ContentFrame);
            Application.Refresh();

            sharedResources.PopupWindow = new Window(new Rect(22, 3, 50, 11), null);
            sharedResources.TargetSubTextView = new TextField(0, 3, 48, sharedResources.SubredditName);
            sharedResources.SubBeingLoaded = new Label(0, 5, "                             ");
            sharedResources.PopupWindow.Add(
                    new Label(0, 1, "Please input a sub"),
                    sharedResources.TargetSubTextView,
                    sharedResources.SubBeingLoaded,
                    new Button(12, 7, "OK", true) { Clicked = LoadSubOK_Clicked },
                    new Button(21, 7, "Cancel", false) { Clicked = LoadSubCancel_Clicked }
            );
            Application.Run(sharedResources.PopupWindow);
        }

        private static void LoadSubOK_Clicked()
        {
            sharedResources.SubBeingLoaded.Text = "Subreddit is being loaded...";
            Application.Refresh();
            // Get name from popupWindow.
            string subName = sharedResources.TargetSubTextView.Text.ToString();
            // Pass sub name to manager.
            // If a sub loads successfully.
            sharedResources.SubredditName = subName;
            if (sharedResources.Manager.SetSubreddit(subName))
            {
                // Draw the frame.
                sharedResources.MainWindow.Add(sharedResources.DisplayFrame.ContentFrame);
                sharedResources.Manager.Next();
                LoadItem();
                sharedResources.ItemToDisplay.Display();
            }
            // Else if a sub fails to load.
            else
            {
                Dialog loadingSubFailed = new Dialog("Given subreddit failed to load!", 50, 6, new Button("Back", true) { Clicked = DialogOK_Clicked });
                Application.Run(loadingSubFailed);
            }

            Application.RequestStop();
        }

        private static void DialogOK_Clicked()
        {
            Application.RequestStop();
        }

        private static void LoadSubCancel_Clicked()
        {
            // If a manager has a sub loaded and open.
            if (sharedResources.Manager.SubredditIsOpen())
            {
                // Draw the frame.
                sharedResources.MainWindow.Add(sharedResources.DisplayFrame.ContentFrame);
            }

            sharedResources.PopupWindow.Running = false;
        }

        private static void Next_Clicked()
        {
            if (sharedResources.Manager.SubredditIsOpen())
            {
                if (sharedResources.Manager.CanGetNext())
                {
                    DisplayLoadingMessage();
                    sharedResources.Manager.Next();
                    LoadItem();
                    sharedResources.ItemToDisplay.Display();
                }
            }
        }

        private static void DisplayLoadingMessage()
        {
            foreach(Label label in sharedResources.DisplayFrame.Rows)
            {
                label.Text = new string(' ', sharedResources.DisplayFrame.ContentWidth);
            }

            string loading = "Loading image...";
            sharedResources.DisplayFrame.Rows[0].Text = loading + (new string(' ', sharedResources.DisplayFrame.ContentWidth - loading.Length));
            Application.Refresh();
        }

        private static void Previous_Clicked()
        {
            if (sharedResources.Manager.SubredditIsOpen())
            {
                if (sharedResources.Manager.CanGetPrevious())
                {
                    DisplayLoadingMessage();
                    sharedResources.Manager.Previous();
                    LoadItem();
                    sharedResources.ItemToDisplay.Display();
                }
            }
        }

        private static void CopyLink_Clicked()
        {
            if (sharedResources.Manager.SubredditIsOpen())
            {
                Thread copyThread = new Thread(CopyLink);
                copyThread.IsBackground = true;
                copyThread.SetApartmentState(ApartmentState.STA);
                copyThread.Start();

                Dialog linkCopySuccessfulDialog = new Dialog("Link copied", 16, 6, new Button("OK", true) { Clicked = DialogOK_Clicked });
                Application.Run(linkCopySuccessfulDialog);
            }
        }

        private static void CopyLink()
        {
            System.Windows.Clipboard.SetText(sharedResources.Manager.GetUri());
        }
        
        private static void LoadItem()
        {
            Media media = sharedResources.Manager.GetCurrentMedia();
            if (media.GetType() == typeof(MediaBitmapImage))
            {
                MediaBitmapImage image = (MediaBitmapImage)media;
                sharedResources.ItemToDisplay = new ASCIIImage(sharedResources.DisplayFrame, image.Image);
            }
        }

        private static void ViewComments_Clicked()
        {
            if (sharedResources.Manager.SubredditIsOpen())
            {
                Window win = new Window("Top comments");
                win.Add(new Button(2, 3, "Back", false) { Clicked = Back_Clicked });
                const int itemsCount = 3;
                List<Comment> comments = sharedResources.Manager.GetTopComments(itemsCount);

                for (int i = 0; i < itemsCount; i++)
                {
                    Comment comment = comments[i];

                    string commentContent = WebUtility.HtmlDecode(comment.Body);
                    List<string> fragmentedComment = getFragmentedString(commentContent, Console.WindowWidth - 24 - 1);

                    FrameView frame = new FrameView(new Rect(20, i * ((Console.WindowHeight - 2) / 3), Console.WindowWidth - 22, ((Console.WindowHeight - 2) / 3) - 1), comment.AuthorName);

                    for (int j = 0; j < Math.Min(((Console.WindowHeight - 2) / 3) - 2, fragmentedComment.Count); j++)
                    {
                        frame.Add(
                            new Label(0, j, fragmentedComment[j])
                        );
                    }

                    win.Add(frame);
                }

                Application.Run(win);
            }
        }

    }
}
