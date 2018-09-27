using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RedditBrowser
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand OpenSub = new RoutedUICommand
            (
                "OpenSub",
                "OpenSub",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.O, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand Download = new RoutedUICommand
        (
            "Download",
            "Download",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                        new KeyGesture(Key.S, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand Prev = new RoutedUICommand
        (
            "Prev",
            "Prev",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                        new KeyGesture(Key.Left)
            }
        );

        public static readonly RoutedUICommand Next = new RoutedUICommand
        (
            "Next",
            "Next",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                        new KeyGesture(Key.Right)
            }
        );


        public static readonly RoutedUICommand ImgLink = new RoutedUICommand
        (
            "ImgLink",
            "ImgLink",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                        new KeyGesture(Key.C, ModifierKeys.Control)
            }
        );


            public static readonly RoutedUICommand ShowButtons = new RoutedUICommand
        (
            "ShowButtons",
            "ShowButtons",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                                new KeyGesture(Key.S, ModifierKeys.Alt)
            }
        );

    }
}
