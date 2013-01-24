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
using System.Windows.Shapes;
using System.Configuration;
using System.Windows.Forms;

namespace ShowRoomSys.Client.Play
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : Window
    {
        public Setting()
        {
            InitializeComponent();
            this.ServerPath.Text = ConfigurationManager.AppSettings["serverpath"];
            this.LocalPath.Text = ConfigurationManager.AppSettings["localpath"];
            this.FullScreen.IsChecked = bool.Parse(ConfigurationManager.AppSettings["isfullscreen"]);
            this.tb_BackGround.Text = ConfigurationManager.AppSettings["background"];
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["serverpath"].Value = this.ServerPath.Text;
            configuration.AppSettings.Settings["localpath"].Value = this.LocalPath.Text;
            configuration.AppSettings.Settings["isfullscreen"].Value = this.FullScreen.IsChecked.ToString();
            configuration.AppSettings.Settings["background"].Value = this.tb_BackGround.Text;
            configuration.Save(); 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            DialogResult dr = fd.ShowDialog();
            fd.ShowNewFolderButton = true;
            if (dr == System.Windows.Forms.DialogResult.OK && fd.SelectedPath != string.Empty)
            {
                this.LocalPath.Text = fd.SelectedPath;
            }
        }

        private void BGChoose_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "图片文件(*.jpg,*.gif,*.bmp)|*.jpg;*.gif;*.bmp";
            DialogResult dr = of.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK && of.FileName != string.Empty)
            {
                this.tb_BackGround.Text = of.FileName;
            }
        }
    }
}
