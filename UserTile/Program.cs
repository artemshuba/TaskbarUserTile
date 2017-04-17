using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using UserTileLib;

namespace UserTile
{
    static class Program
    {
        public static IntPtr taskbarHwnd;
        public static IntPtr trayHwnd;
        private static IntPtr langbarHwnd;
        private static IntPtr showDesktopButtonHwnd;
        private static UserPic userPic;
        private static System.Windows.Forms.Timer timer;
        public static WinAPI.RECT defaultTrayRect;
        public static WinAPI.WINDOWPLACEMENT showDesktopDefaultPlacement;
        public static TaskbarManager taskbarManager;
        //private static IConfigSource config;
        public static string AvatarPath;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            //if (File.Exists("config.ini"))
            //{
            //    Program.config = (IConfigSource)new IniConfigSource("config.ini");
            //    Program.AvatarPath = Program.config.Configs["Main"].GetString("AvatarPath");
            //}

            taskbarManager = new TaskbarManager();
            userPic = new UserPic();
            if (!Program.taskbarManager.IsTaskbarSmall())
            {
                userPic.Width = 37;
                userPic.Height = 36;
                taskbarManager.ReserveSpace(37);
            }
            else
            {
                userPic.Width = 27;
                userPic.Height = 26;
                taskbarManager.ReserveSpace(27);
            }
            userPic.Top = 3;
            taskbarManager.AddControl((UserControl)Program.userPic);
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 10;
            timer.Tick += new EventHandler(Program.TimerTick);
            timer.Enabled = true;
            Application.EnableVisualStyles();
            Application.ThreadException += new ThreadExceptionEventHandler(Program.Application_ThreadException);
            Application.Run();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Program.Log(e.Exception);
        }

        public static void Log(Exception e)
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!File.Exists(directoryName + "\\log.txt"))
                File.WriteAllText(directoryName + "\\log.txt", string.Empty);
            try
            {
                File.AppendAllText(directoryName + "\\log.txt", DateTime.Now.ToString() + " -------------- " + (object)'\r' + (object)'\n' + "OS: " + Environment.OSVersion.VersionString + (object)'\r' + (object)'\n' + e.ToString() + (object)'\r' + (object)'\n');
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show("Can't write log. " + ex.Message);
            }
        }

        private static void TimerTick(object sender, EventArgs e)
        {
            taskbarManager.CheckTaskbar();
        }
    }
}