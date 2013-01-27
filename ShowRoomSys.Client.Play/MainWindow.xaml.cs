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
using System.ServiceModel;
using System.Windows.Xps.Packaging;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Web;

namespace ShowRoomSys.Client.Play
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ControlService.IMainControlCallback
    {
        private NotifyIcon notifyIcon = null;
        System.Windows.WindowState currentState;
        System.Windows.WindowStyle currentStyle;
        double? currentwidth;
        double? currentheight;
        double? currentleft;
        double? currenttop;
        int CurrentFile = 0;
        int TotalFiles = 0;
        List<string> files;
        System.Configuration.AppSettingsReader config = new AppSettingsReader();
        Client client = new Client();
        InstanceContext ctx;
        ControlService.MainControlClient svc;
        DocumentPage dp = null;
        //MediaPage mp = null;
        MediasPage mp = null;
        WebBrowserPage wbp = null;
        PDFPage pp = null;
        ImagePage ip = null;
        public int? PID;
        public enum PlayType
        {
            Document,
            Media,
            Url,
            PDF,
            EXE,
            Img,
            Swf,
            Null
        }
        PlayType now = PlayType.Null;
        public MainWindow()
        {
            InitializeComponent();
            InitialTray();
            if (bool.Parse(ConfigurationManager.AppSettings["isfullscreen"]))
                this.FullScreen();
        }

        #region about notifyIcon
        private void InitialTray()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.BalloonTipText = "Welcome";
            notifyIcon.Icon = new System.Drawing.Icon(System.Windows.Forms.Application.StartupPath.Replace(@"bin\Debug","") + @"/Icon/play.ico");
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(2000);
            notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);

            //menu options
            //System.Windows.Forms.MenuItem setting1 = new System.Windows.Forms.MenuItem("服务器地址");
            //System.Windows.Forms.MenuItem setting2 = new System.Windows.Forms.MenuItem("本地文件存储路径");
            //setting2.Click += new EventHandler(SetStorePath_Click);
            //System.Windows.Forms.MenuItem setting3 = new System.Windows.Forms.MenuItem("选择背景图片");
            //setting3.Click += new EventHandler(ChooseBGImage_Click);
            //System.Windows.Forms.MenuItem setting4 = new System.Windows.Forms.MenuItem("全屏");
            System.Windows.Forms.MenuItem setting = new System.Windows.Forms.MenuItem("设置");
            setting.Click+=new EventHandler(setting_Click);

            //show mainwindow
            System.Windows.Forms.MenuItem help = new System.Windows.Forms.MenuItem("显示主窗口");

            //about
            System.Windows.Forms.MenuItem about = new System.Windows.Forms.MenuItem("关于");

            //exit
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += new EventHandler(exit_Click);

            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { setting, help, about, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);
        }
        private void setting_Click(object sender, EventArgs e)
        {
            Setting st = new Setting();
            st.Owner = this;
            st.Show();
        }
        private void ChooseBGImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "图片文件(*.jpg,*.gif,*.bmp)|*.jpg;*.gif;*.bmp";
            DialogResult dr = of.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK && of.FileName != string.Empty)
            {
                ImageBrush b = new ImageBrush();
                b.ImageSource = new BitmapImage(new Uri(of.FileName));//pack://application:,,,/images/SSL21037.JPG
                b.Stretch = Stretch.Fill;
                this.Background = b;
            }
        }       
        private void SysTray_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Visibility = Visibility.Hidden;
            }
        }
        private void exit_Click(object sender, EventArgs e)
        {
            if (System.Windows.MessageBox.Show("sure to exit?",
                                               "application",
                                                MessageBoxButton.YesNo,
                                                MessageBoxImage.Question,
                                                MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }
        private void SetStorePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            DialogResult dr = fd.ShowDialog();
            fd.ShowNewFolderButton = true;
            if (dr == System.Windows.Forms.DialogResult.OK && fd.SelectedPath != string.Empty)
            {
                //save path
            }
        }
        private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Hidden;
                    notifyIcon.BalloonTipText = "播放器在后台运行...";
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    this.Activate();
                    this.Show();
                    WindowState = currentState;
                    WindowStyle = currentStyle;
                }
            }
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ConfigurationManager.AppSettings["background"] != "")
            {
                ImageBrush b = new ImageBrush();
                b.ImageSource = new BitmapImage(new Uri(ConfigurationManager.AppSettings["background"]));
                b.Stretch = Stretch.Fill;
                this.Background = b;
            }
            else
            {
                SolidColorBrush b = new SolidColorBrush();
                b.Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("Black");
                this.Background = b;
            }
            if (ConfigurationManager.AppSettings["backgroundflash"] != "")
            {
                this.LoadFlash(ConfigurationManager.AppSettings["backgroundflash"]);
            }
            this.currentheight = this.Height;
            this.currentwidth = this.Width;
            this.currentleft = this.Left;
            this.currenttop = this.Top;
            RegistToServer();
            StartTimer();
        }
        private void RegistToServer()
        {
            try
            {
                ctx = new InstanceContext(this);
                svc = new ControlService.MainControlClient(ctx);
                svc.ClientCredentials.Windows.ClientCredential.UserName = config.GetValue("user", typeof(string)).ToString();
                svc.ClientCredentials.Windows.ClientCredential.Password = config.GetValue("pwd", typeof(string)).ToString();
                svc.RegisterClient(ShowRoomSys.CommonLib.Common.MAC);
            }
            catch { System.Windows.MessageBox.Show("Service does not running!"); }
        }

        #region accept order from service
        public void SendMessage(ControlService.OptionType type)
        {
            Option(type);
        }
        public void SendMessages(ControlService.OptionType type, string fileName)
        {
            files = new List<string>() { fileName };
            TotalFiles = files.Count;
            Option(type);
        }
        #endregion

        #region player options
        private void Option(ControlService.OptionType type)
        {
            switch (type)
            {
                case ControlService.OptionType.Start:
                    Start();
                    break;
                case ControlService.OptionType.NextDoc:
                    NextFile();
                    break;
                case ControlService.OptionType.LastDoc:
                    LastFile();
                    break;
                case ControlService.OptionType.FullScreen:
                    FullScreen();
                    break;
                case ControlService.OptionType.Next:
                    GONextPage();
                    break;
                case ControlService.OptionType.Previous:
                    GOPreviousPage();
                    break;
                case ControlService.OptionType.IncreaseZoom:
                    IncreaseZoom();
                    break;
                case ControlService.OptionType.DecreaseZoom:
                    DecreaseZoom();
                    break;
                case ControlService.OptionType.Pause:
                    this.Pause();
                    break;
                case ControlService.OptionType.Play:
                    this.Play();
                    break;
                case ControlService.OptionType.Stop:
                    this.Stop();
                    break;
                case ControlService.OptionType.MoveUp:
                    this.MoveUp();
                    break;
                case ControlService.OptionType.MoveDown:
                    this.MoveDown();
                    break;
                case ControlService.OptionType.MoveLeft:
                    this.MoveLeft();
                    break;
                case ControlService.OptionType.MoveRight:
                    this.MoveRight();
                    break;
            }
        }
        private void Pause()
        {
            if (mp != null)
                this.mp.Pause();
        }
        private void Play()
        {
            if (mp != null)
                this.mp.Play();
        }
        private void Stop()
        {
            if (mp != null)
                this.mp.Stop();
        }
        private void MoveUp()
        {
            if (dp!=null)
                this.dp.MoveUp();
        }
        private void MoveDown()
        {
            if (this.dp!=null)
                this.dp.MoveDown();
        }
        private void MoveLeft()
        {
            if (this.dp!=null)
                this.dp.MoveLeft();
        }
        private void MoveRight()
        {
            if (this.dp!=null)
                this.dp.MoveRight();
        }
        private void IncreaseZoom()
        {
            if (this.dp != null && now == PlayType.Document)
                this.dp.IncreaseZoom();
            if (this.pp != null && now == PlayType.PDF)
                this.pp.IncreaseZoom();
            if (this.ip != null && now == PlayType.Img)
                this.ip.IncreaseZoom();
        }
        private void DecreaseZoom()
        {
            if (this.dp != null && now == PlayType.Document)
                this.dp.DecreaseZoom();
            if (this.pp != null && now == PlayType.PDF)
                this.pp.DecreaseZoom();
            if (this.ip != null && now == PlayType.Img)
                this.ip.DecreaseZoom();
        }
        private void GONextPage()
        {
            if (this.dp != null && now == PlayType.Document)
                this.dp.GONextPage();
            if (this.pp != null && now == PlayType.PDF)
                this.pp.GONextPage();
        }
        private void GOPreviousPage()
        {
            if (this.dp != null && now == PlayType.Document)
                this.dp.GOPreviousPage();
            if (this.pp != null && now == PlayType.PDF)
                this.pp.GOPreviousPage();
        }
        private void FullScreen()
        {
            if (this.WindowState != System.Windows.WindowState.Maximized)
            {
                currentState = this.WindowState;
                currentStyle = this.WindowStyle;
                this.WindowState = System.Windows.WindowState.Maximized;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
                this.Topmost = false;
                this.WindowStyle = System.Windows.WindowStyle.None;
                this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
                this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
                this.Hide();
                this.Show();
            }
            else
            {
                System.Windows.WindowState temp = this.WindowState;
                System.Windows.WindowStyle style = this.WindowStyle;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
                this.WindowState = currentState;
                this.WindowStyle = currentStyle;
                currentState = temp;
                currentStyle = style;
                this.Height = this.currentheight.HasValue == true ? 350 : this.currentheight.Value;
                this.Width = this.currentwidth.HasValue == true ? 525 : this.currentwidth.Value;
                this.Left = this.currentwidth.HasValue == true ? 0 : this.currentleft.Value; ;
                this.Top = this.currentwidth.HasValue == true ? 0 : this.currenttop.Value; ;
            }
            SubPageFullScreen();
        }
        private void SubPageFullScreen()
        {
            switch (now)
            {
                case PlayType.Document:
                    if (dp != null)
                        dp.AdjustView();
                    break;
                case PlayType.Media:
                    if (mp != null)
                        mp.AdjustView(this.WindowState == System.Windows.WindowState.Maximized);
                    break;
                case PlayType.Url:
                    break;
                case PlayType.PDF:
                    if (pp != null)
                        pp.AdjustView();
                    break;
                case PlayType.Img:
                    if (ip != null)
                        ip.AdjustView();
                    break;
            }
        }
        private void Start()
        {
            GetFile(null);
        }
        private void NextFile()
        {
            GetFile(true);
        }
        private void LastFile()
        {
            GetFile(false);
        }
        #endregion

        #region get file
        private void GetFile(bool? isNext)
        {
            if (isNext.HasValue)
            {
                if (isNext.Value)
                {
                    if (CurrentFile == TotalFiles - 1)
                        CurrentFile = 0;
                    else
                        CurrentFile++;
                }
                else
                {
                    if (CurrentFile == 0)
                        CurrentFile = TotalFiles - 1;
                    else
                        CurrentFile--;
                }
            }
            string ext;
            if (CheckIsUrlFormat(files[CurrentFile]))
                ext = "url";
            else
                ext = System.IO.Path.GetExtension(files[CurrentFile]).ToLower().TrimStart('.');
            switch (ext)
            {
                case "ppt":
                case "pptx":
                case "doc":
                case "docx":
                case "xls":
                case "xlsx":
                    now = PlayType.Document;
                    this.LoadDocument();
                    break;
                case "avi":
                case "wmv":
                    now = PlayType.Media;
                    this.LoadMedia();
                    break;
                case "exe":
                    now = PlayType.EXE;
                    this.LoadEXE(files[CurrentFile]);
                    break;
                case "pdf":
                    now = PlayType.PDF;
                    this.LoadPDF();
                    break;
                case "url":
                    now = PlayType.Url;
                    this.LoadUrl(files[CurrentFile]);
                    break;
                case "swf":
                    now = PlayType.Swf;
                    this.LoadFlash();
                    break;
                case "jpg":
                case "png":
                case "bmp":
                    now = PlayType.Img;
                    this.LoadImage();
                    break;
                default:
                    break;
            }

        }
        private void LoadFlash()
        {
            if (this.DealFile(System.IO.Path.GetFileName(files[CurrentFile])))
            {
                this.LoadFlash(config.GetValue("localpath", typeof(string)).ToString() + System.IO.Path.GetFileName(files[CurrentFile]));
            }
            else
            {
                System.Windows.MessageBox.Show("加载文件失败！");
            }
        }
        private void LoadFlash(string path)
        {
            Destroy();
            wbp = new WebBrowserPage(path);
            this.PageContext.Content = wbp;
        }
        private void LoadMedia()
        {
            Destroy();
            if (this.DealFile(System.IO.Path.GetFileName(files[CurrentFile])))
            {
                mp = new MediasPage(config.GetValue("localpath", typeof(string)).ToString() + System.IO.Path.GetFileName(files[CurrentFile]));
                mp.AdjustView(this.WindowState == System.Windows.WindowState.Maximized);
                this.PageContext.Content = mp;
            } 
            else
            {
                System.Windows.MessageBox.Show("加载文件失败！");
            }
        }
        private void LoadDocument()
        {
            Destroy();
            if (this.DealFile(System.IO.Path.GetFileNameWithoutExtension(files[CurrentFile]) + ".xps"))
            {
                dp = new DocumentPage(config.GetValue("localpath", typeof(string)).ToString() + System.IO.Path.GetFileNameWithoutExtension(files[CurrentFile]) + ".xps");
                dp.AdjustView();
                this.PageContext.Content = dp;
            }
            else
            {
                System.Windows.MessageBox.Show("加载文件失败！");
            }
        }
        private void LoadPDF()
        {
            if (this.DealFile(System.IO.Path.GetFileName(files[CurrentFile])))
            {
                Destroy();
                pp = new PDFPage(config.GetValue("localpath", typeof(string)).ToString() + System.IO.Path.GetFileName(files[CurrentFile]));
                pp.AdjustView();
                this.PageContext.Content = pp;
            }
        }
        private void LoadUrl(string url)
        {
            Destroy();
            wbp = new WebBrowserPage(url);
            this.PageContext.Content = wbp;
        }
        private void LoadEXE(string path)
        {
            Destroy();
            //System.Windows.MessageBox.Show(path);
            Process p = new Process();
            string str = path.Replace("//", @"\");
            p.StartInfo.FileName = str;
            try
            {
                p.Start();
                PID = p.Id;
            }
            catch (Win32Exception err)
            {
                System.Windows.MessageBox.Show(err.Message);
            }
        }
        private void LoadImage()
        {
            if (this.DealFile(System.IO.Path.GetFileName(files[CurrentFile])))
            {
                Destroy();
                ip = new ImagePage(config.GetValue("localpath", typeof(string)).ToString() + System.IO.Path.GetFileName(files[CurrentFile]));
                this.PageContext.Content = ip;
            }

        }
        public bool DealFile(string path)
        {
            string localpath = config.GetValue("localpath", typeof(string)).ToString();
            string serverpath = config.GetValue("serverpath", typeof(string)).ToString()+path;
            if (!Directory.Exists(localpath))
                Directory.CreateDirectory(localpath);
            localpath += path;
            if (File.Exists(localpath))
                return true;
            else
            {
                PleaseWait pw = new PleaseWait();
                pw.Owner = this;
                pw.Show();
                try
                {
                    if (File.Exists(serverpath))
                    {
                        File.Copy(serverpath, localpath);
                    }
                }
                catch { return false; }
                pw.Close();
                return true;
            }
        }

        public bool CheckIsUrlFormat(string strValue)
        {
            return this.CheckIsFormat(@"^(http|HTTP|https|HTTPS)://([\w-]+\.)*[\w-]+(:\d+)?(/[\u4e00-\u9fa5\w- ./?%&=]*)?$", strValue);
        }
        public bool CheckIsFormat(string strRegex, string strValue)
        {
            if (strValue != null && strValue.Trim() != "")
            {
                Regex re = new Regex(strRegex);
                if (re.IsMatch(strValue))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public void Destroy()
        {
            if (mp != null)
                mp.Dispose();
            if (pp != null)
                pp.Dispose();
            if (PID.HasValue)
            {
                try
                {
                    var ps = System.Diagnostics.Process.GetProcessById(PID.Value);
                    //ps.Dispose();
                    ps.Kill();
                    PID = null;
                }
                catch { }
            }
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (svc.State != CommunicationState.Faulted)
                    svc.ChannelFactory.Close();
            }catch{}
        }
        private void StartTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromHours(1);
            timer.Tick += dispatcherTimer_Tick;  
            timer.Start();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (svc.State != CommunicationState.Opened)
                this.RegistToServer();
        }
    }
}
