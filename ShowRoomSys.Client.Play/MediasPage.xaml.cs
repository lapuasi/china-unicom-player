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
    /// Interaction logic for MediasPage.xaml
    /// </summary>
    public partial class MediasPage : Page
    {
        public MediasPage()
        {
            InitializeComponent();
        }
        public MediasPage(string filePath)
            : this()
        {
            this.myVideo.Source = new Uri(filePath);
            myVideo.Stretch = Stretch.Fill;
            myVideo.LoadedBehavior = MediaState.Manual;
            myVideo.Play();
        }
        public void Play()
        {
            try
            {
                if (myVideo.Source != null)
                {
                    myVideo.Play();
                }
            }
            catch { }
        }
        public void Pause()
        {
            try
            {
                if (myVideo.Source != null)
                {
                    myVideo.Pause();
                }
            }
            catch { }
        }
        public void Stop()
        {
            try
            {
                if (myVideo.Source != null)
                {
                    myVideo.Stop();
                }
            }
            catch { }
        }
        public void AdjustView(bool isFull)
        {
        }
        public void FullScreen()
        {
        }
        public void Dispose()
        {
        }
    }
}
