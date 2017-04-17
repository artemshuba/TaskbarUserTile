using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UserTile
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        public PopupWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.Top = (double)SystemInformation.WorkingArea.Height - this.Height - 5.0;
            this.Left = (double)SystemInformation.WorkingArea.Width - this.Width - 10.0;

            if (!string.IsNullOrEmpty(Program.AvatarPath) && File.Exists(Program.AvatarPath))
            {
                if (Program.AvatarPath.EndsWith(".wmv"))
                {
                    //this.Player.Source = new Uri(Program.AvatarPath, UriKind.Relative);
                    //this.Player.Play();
                }
                else
                    this.Avatar.Source = new BitmapImage(new Uri(Program.AvatarPath, UriKind.RelativeOrAbsolute));
            }
            else
                this.Avatar.Source = !File.Exists(Path.GetTempPath() + "\\" + Environment.UserName + ".bmp") ? (ImageSource)new BitmapImage(new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\userpic.png")) : (ImageSource)new BitmapImage(new Uri(Path.GetTempPath() + "\\" + Environment.UserName + ".bmp"));
            this.Username.Text = Environment.UserName;
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SwitchUser_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Process.Start("tsdiscon.exe");
            }
            catch (Exception ex)
            {
                Program.Log(ex);
            }
        }

        private void LogOff_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("shutdown.exe", "/l");
        }

        private void LockPC_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("C:\\WINDOWS\\system32\\rundll32.exe", "user32.dll,LockWorkStation");
        }

        private void MySettings_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("control.exe");
        }

        private void MyLook_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("control.exe", "userpasswords");
        }
    }
}