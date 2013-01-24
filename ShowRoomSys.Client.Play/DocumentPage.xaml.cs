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
using System.Configuration;
using System.Windows.Xps.Packaging;
using System.IO;

namespace ShowRoomSys.Client.Play
{
    /// <summary>
    /// Interaction logic for DocumentPage.xaml
    /// </summary>
    public partial class DocumentPage : Page
    {

        public DocumentPage()
        {
            InitializeComponent();
        }
        public DocumentPage(string filePath)
            : this()
        {
            LoadXPSFile(filePath);
        }
        private void LoadXPSFile(string _xpsfile)
        {
            using (XpsDocument xpsDoc = new XpsDocument(_xpsfile, FileAccess.Read))
            {
                var fs = xpsDoc.GetFixedDocumentSequence();
                this.xpsDoc.Document = fs;
                this.AdjustView();
            }
        }
        public void AdjustView()
        {
            this.xpsDoc.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
            this.xpsDoc.FitToMaxPagesAcross(1);
            DocumentViewer.FitToMaxPagesAcrossCommand.Execute("1", this.xpsDoc);
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri(ConfigurationManager.AppSettings["background"]));
            b.Stretch = Stretch.Fill;
            this.Background = b;
            this.Resources["bgImage"] = b;
        }
        public void MoveUp()
        {
            if (this.xpsDoc.CanMoveUp)
                this.xpsDoc.MoveUp();
        }
        public void MoveDown()
        {
            if (this.xpsDoc.CanMoveDown)
                this.xpsDoc.MoveDown();
        }
        public void MoveLeft()
        {
            if (this.xpsDoc.CanMoveLeft)
                this.xpsDoc.MoveLeft();
        }
        public void MoveRight()
        {
            if (this.xpsDoc.CanMoveRight)
                this.xpsDoc.MoveRight();
        }
        public void IncreaseZoom()
        {
            if (this.xpsDoc.CanIncreaseZoom)
                this.xpsDoc.IncreaseZoom();
        }
        public void DecreaseZoom()
        {
            if (this.xpsDoc.CanDecreaseZoom)
                this.xpsDoc.DecreaseZoom();
        }
        public void GONextPage()
        {
            this.xpsDoc.NextPage();
        }
        public void GOPreviousPage()
        {
            this.xpsDoc.PreviousPage();
        }
        
    }
}
