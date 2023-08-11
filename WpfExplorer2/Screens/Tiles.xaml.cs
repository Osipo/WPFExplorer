using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfExplorer.ViewModels;
using WpfExplorerControl.Extensions;

namespace WpfExplorer.Screens
{
    /// <summary>
    /// Логика взаимодействия для Tiles.xaml
    /// </summary>
    public partial class Tiles : Page
    {
        public Tiles()
        {
            DataContext = (App.Current.Resources["servicelocator"] as ViewModelServiceLocator).TilesViewModel;
            InitializeComponent();
        }

        private Canvas _activeLine;

        private void line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas c = sender as Canvas;
            if(c == null)
            {
                e.Handled = true;
                return;
            }

            
            var pos = e.GetPosition(c);
            _activeLine = c;

            //add rect
            Rectangle rect = new Rectangle();
            rect.Width = 20;
            rect.Height = c.ActualHeight;
            rect.Fill = Brushes.DarkGreen;
            rect.Focusable = true;
            rect.MouseMove += rect_Moved;
            rect.MouseLeave += rect_Moved;
            rect.MouseLeftButtonDown += rect_Clicked;

            Binding b = new Binding("ActualHeight");
            b.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Canvas), 1);
            rect.SetBinding(Rectangle.HeightProperty, b);

            //attach focus event handlers through focusManager for shapes.
            FocusManager.AddLostFocusHandler(rect, rect_LostFocus);
            FocusManager.AddGotFocusHandler(rect, rect_Clicked); 

            Canvas.SetLeft(rect, pos.X);
            Canvas.SetTop(rect, 0);
            c.Children.Add(rect);
            e.Handled = true;
            rect.RaiseEvent(new RoutedEventArgs(routedEvent: Rectangle.GotFocusEvent));
        }

        private void rect_Clicked(object sender, RoutedEventArgs e)
        {
            Rectangle r = sender as Rectangle;
            if (r == null)
            {
                e.Handled = true;
                return;
            }
            r.Focus();
            r.Fill = Brushes.Blue;
            e.Handled = true;
        }

        private void rect_LostFocus(object sender, RoutedEventArgs e)
        {
            Rectangle r = sender as Rectangle;
            if (r == null)
            {
                e.Handled = true;
                return;
            }
            r.Fill = Brushes.DarkGreen;
            e.Handled = true;
        }

        private void rect_Moved(object sender, MouseEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if (rect == null || _activeLine == null || e.LeftButton == MouseButtonState.Released)
            {
                e.Handled = true;
                return;
            }
            var pos = e.GetPosition(_activeLine);
            var x = pos.X < 0.0 ? 0.0 : pos.X;


            //Right boundary is out.
            if(x + rect.ActualWidth > _activeLine.ActualWidth)
            {
                linesContainer.FindVisualChildren<Canvas>().ForEach(c_i => c_i.Width = c_i.ActualWidth * 1.5);
            }
            Canvas.SetLeft(rect, x);

            e.Handled = true;
        }
    }
}
