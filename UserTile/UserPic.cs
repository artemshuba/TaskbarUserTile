using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace UserTile
{
    public partial class UserPic : UserControl
    {
        private readonly Timer t;

        public UserPic()
        {
            InitializeComponent();

            this.picture.Width = this.Width - 2;
            this.picture.Height = this.Height - 2;
            this.picture.Left = 1;
            this.picture.Top = 1;

            if (string.IsNullOrEmpty(Program.AvatarPath))
            {
                if (File.Exists(Path.GetTempPath() + "\\" + Environment.UserName + ".bmp"))
                    this.picture.Load(Path.GetTempPath() + "\\" + Environment.UserName + ".bmp");
                else
                    this.picture.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\userpic.png");
            }
            else if (Program.AvatarPath.EndsWith(".wmv") && File.Exists(Program.AvatarPath))
            {
                //this.player.Width = this.Width - 2;
                //this.player.Height = this.Height - 2;
                //this.player.Left = 1;
                //this.player.Top = 1;
                //this.player.uiMode = "none";
                //this.player.enableContextMenu = false;
                //this.player.URL = Program.AvatarPath;
                //this.player.Visible = true;
                //this.player.ClickEvent += new AxWMPLib._WMPOCXEvents_ClickEventHandler(this.PlayerClickEvent);
                this.t = new Timer();
                this.t.Interval = 1000;
                this.t.Tick += new EventHandler(this.Tick);
                this.t.Start();
            }
            else
            {
                this.picture.Load(Program.AvatarPath);
            }
            this.picture.MouseClick += new MouseEventHandler(this.PictureMouseClick);
        }

        private void Tick(object sender, EventArgs e)
        {
            //TODO
            //if (this.player == null || this.player.currentMedia == null)
            //    return;
            //if (this.player.Ctlcontrols.currentPosition >= this.player.currentMedia.duration - 0.5)
            //    this.player.Ctlcontrols.currentPosition = 0.0;
            //if (this.player.playState != WMPPlayState.wmppsMediaEnded)
            //{
            //    this.player.Width = this.Width - 2;
            //    this.player.Height = this.Height - 2;
            //    this.player.Ctlcontrols.play();
            //}
        }

        private void PictureMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            new PopupWindow().Show();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 123)
            {
                this.contextMenu.Left = Control.MousePosition.X;
                this.contextMenu.Show((Control)this, Control.MousePosition);
            }
            else
                base.WndProc(ref m);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //this.player.close();
            //this.player.Dispose();
            if (this.t != null)
            {
                this.t.Stop();
                this.t.Dispose();
            }
            Program.taskbarManager.Dispose();
            Application.Exit();
        }

        private void UserPicSizeChanged(object sender, EventArgs e)
        {
            this.picture.Width = this.Width - 2;
            this.picture.Height = this.Height - 2;
            //this.player.Width = this.Width - 2;
            //this.player.Height = this.Height - 2;
        }

        //private void PlayerClickEvent(object sender, _WMPOCXEvents_ClickEvent e)
        //{
        //    if ((int)e.nButton == 1)
        //    {
        //        new PopupWindow().Show();
        //    }
        //    else
        //    {
        //        this.contextMenu.Left = Control.MousePosition.X;
        //        this.contextMenu.Show((Control)this, Control.MousePosition);
        //    }
        //}
    }
}