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
    /// <summary>
    /// Interaction logic for WebBrowserPage.xaml
    /// </summary>
    public partial class WebBrowserPage : Page
    {
        public WebBrowserPage()
        {
            InitializeComponent();
        }
        public WebBrowserPage(string url)
            : this()
        {
            this.m_WebBrowser.Source = (new Uri(url, UriKind.RelativeOrAbsolute));
        }
    }
}
