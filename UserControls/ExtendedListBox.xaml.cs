using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UserControls
{
    /// <summary>
    /// In essence it is the same as a ListBox, but it does have more events.
    /// </summary>
    public partial class ExtendedListBox : ListBox
    {
        #region Events

        static readonly RoutedEvent ScrollReachedBottomEvent =
            EventManager.RegisterRoutedEvent("ScrollReachedBottom",
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(ExtendedListBox));

        static readonly RoutedEvent ScrollReachedTopEvent =
            EventManager.RegisterRoutedEvent("ScrollReachedTop",
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(ExtendedListBox));

        static readonly RoutedEvent NeedsMoreItemsEvent =
            EventManager.RegisterRoutedEvent("NeedsMoreItems",
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(ExtendedListBox));

        #endregion // Events

        #region Handlers

        public event RoutedEventHandler ScrollReachedBottom
        {
            add     { AddHandler    (ScrollReachedBottomEvent,  value); }
            remove  { RemoveHandler (ScrollReachedBottomEvent,  value); }
        }

        public event RoutedEventHandler ScrollReachedTop
        {
            add     { AddHandler    (ScrollReachedTopEvent,     value); }
            remove  { RemoveHandler (ScrollReachedTopEvent,     value); }
        }

        public event RoutedEventHandler NeedsMoreItems
        {
             add    { AddHandler    (NeedsMoreItemsEvent,     value); }
            remove  { RemoveHandler (NeedsMoreItemsEvent,     value); }
        }

        #endregion // Handlers
        
        private double PreviousHeight = 0.0d;
        
        public ExtendedListBox()
        {
            InitializeComponent();
        }

        private void ListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = GetVisualChildCollection<ScrollViewer>(this).FirstOrDefault();

            if (scrollViewer.ScrollableHeight == 0.0d)
            {
                RaiseNeedsMoreItems();
            }

            else if (e.VerticalOffset == scrollViewer.ScrollableHeight && PreviousHeight != e.VerticalOffset)
            {
                RaiseScrollReachedBottom();
            }

            else if (e.VerticalOffset == 0.0d && PreviousHeight != 0.0d)
            {
                RaiseScrollReachedTop();
            }

            PreviousHeight = e.VerticalOffset;

            e.Handled = false;
        }

        private void RaiseScrollReachedBottom()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ExtendedListBox.ScrollReachedBottomEvent);
            RaiseEvent(newEventArgs);
        }

        private void RaiseScrollReachedTop()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ExtendedListBox.ScrollReachedTopEvent);
            RaiseEvent(newEventArgs);
        }

        private void RaiseNeedsMoreItems()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ExtendedListBox.NeedsMoreItemsEvent);
            RaiseEvent(newEventArgs);
        }

        private static List<T> GetVisualChildCollection<T>(object parent) where T : Visual
        {
            List<T> visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        private static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    visualCollection.Add(child as T);
                }
                else if (child != null)
                {
                    GetVisualChildCollection(child, visualCollection);
                }
            }
        }
    }
}
