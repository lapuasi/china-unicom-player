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
    /// Interaction logic for Media.xaml
    /// </summary>
    public partial class MediaPage : Page
    {
        public MediaPage()
        {
            InitializeComponent();
        }
        public MediaPage(string filePath)
            : this()
        {
            this.wpfMediaPlayer.URL = filePath;
        }
        public void AdjustView(bool isFull)
        {
            this.wpfMediaPlayer.fullScreen = isFull;
        }
        public void Pause()
        {
            this.wpfMediaPlayer.Ctlcontrols.pause();
        }
        public void Play()
        {
            this.wpfMediaPlayer.Ctlcontrols.play();
        }
        public void Stop()
        {
            this.wpfMediaPlayer.Ctlcontrols.stop();
        }
        public void Dispose()
        {
            this.wpfMediaPlayer.Dispose();
        }
    }
}
