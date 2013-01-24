using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShowRoomSys.Client.Play
{
     //<summary>
     //Interaction logic for ImagePage.xaml
     //</summary>
    public partial class ImagePage : Page
    {
        private Uri path = new Uri(System.Configuration.ConfigurationManager.AppSettings["background"]);
        Point previousMousePoint = new Point(0, 0);
        public ImagePage()
        {
            InitializeComponent();
            this.img.Stretch = Stretch.Fill;
            this.img.Source = new BitmapImage(path);
        }
        public ImagePage(string filePath)
            : this()
        {
            this.img.Stretch = Stretch.Fill;
            this.img.Source = new BitmapImage(new Uri(filePath));
        }
        double w = 0;
        double h = 0;
        public void AdjustView(double sx,double xy)
        {
            w = this.ActualWidth;
            h = this.ActualHeight;
            this.img.Width = w;
            this.img.Height = h;
            Canvas.SetLeft(img, (w - (w * sx)) / 2);
            Canvas.SetTop(img, (h - (h * xy)) / 2);
        }
        public void AdjustView()
        {
            w = this.ActualWidth;
            h = this.ActualHeight;
            this.img.Width = w;
            this.img.Height = h;
            Canvas.SetTop(img, 0);
            Canvas.SetLeft(img, 0);
        }
        public void IncreaseZoom()
        {
            sfr.ScaleX += 0.05;
            sfr.ScaleY += 0.05;
            AdjustView(sfr.ScaleX, sfr.ScaleY);
        }
        public void DecreaseZoom()
        {
            sfr.ScaleX -= 0.05;
            sfr.ScaleY -= 0.05;
            AdjustView(sfr.ScaleX, sfr.ScaleY);
            AdjustView(sfr.ScaleX, sfr.ScaleY);
        }
        private void img_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                IncreaseZoom();
            else
                DecreaseZoom();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustView();
        }
    }
}
