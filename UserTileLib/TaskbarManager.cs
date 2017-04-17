using System;
using System.Windows.Forms;
using static UserTileLib.WinAPI;

namespace UserTileLib
{
    public class TaskbarManager
    {
        private int reservedWidth = 32;
        private readonly IntPtr taskbarHwnd;
        private readonly IntPtr trayHwnd;
        private readonly IntPtr rebarHwnd;
        private readonly IntPtr minimizeHwnd;
        private TaskbarPosition currentTaskbarPos;
        private bool spaceReserved;
        private Control control;

        private event EventHandler TrayResized;

        private event EventHandler RebarResized;

        public TaskbarManager()
        {
            this.taskbarHwnd = WinAPI.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", (string)null);
            this.rebarHwnd = WinAPI.FindWindowEx(this.taskbarHwnd, IntPtr.Zero, "ReBarWindow32", (string)null);
            this.trayHwnd = WinAPI.FindWindowEx(this.taskbarHwnd, IntPtr.Zero, "TrayNotifyWnd", (string)null);
            this.minimizeHwnd = WinAPI.FindWindowEx(this.trayHwnd, IntPtr.Zero, "TrayShowDesktopButtonWClass", (string)null);
            this.currentTaskbarPos = this.DetectTaskbarPos();
            this.TrayResized += new EventHandler(this.TaskbarManagerTrayResized);
            this.RebarResized += new EventHandler(this.TaskbarManagerRebarResized);
        }

        private void TaskbarManagerRebarResized(object sender, EventArgs e)
        {
            this.ReduceRebarWidth();
        }

        private void TaskbarManagerTrayResized(object sender, EventArgs e)
        {
            this.MoveTrayToLeft();
            this.ReduceRebarWidth();
        }

        public TaskbarPosition DetectTaskbarPos()
        {
            WinAPI.RECT lpRect;
            WinAPI.GetWindowRect(this.taskbarHwnd, out lpRect);
            if (lpRect.Left == 0 && lpRect.Bottom == SystemInformation.VirtualScreen.Bottom && lpRect.Top == SystemInformation.WorkingArea.Top)
                return TaskbarPosition.Left;
            if (lpRect.Left == 0 && lpRect.Top == 0 && lpRect.Bottom != SystemInformation.PrimaryMonitorSize.Height)
                return TaskbarPosition.Top;
            if (lpRect.Left != 0 && lpRect.Top == 0 && lpRect.Bottom == SystemInformation.PrimaryMonitorSize.Height)
                return TaskbarPosition.Right;
            return lpRect.Left == 0 && lpRect.Top != 0 && lpRect.Bottom == SystemInformation.PrimaryMonitorSize.Height ? TaskbarPosition.Bottom : TaskbarPosition.Unknown;
        }

        public bool IsTaskbarSmall()
        {
            WinAPI.RECT lpRect;
            WinAPI.GetWindowRect(this.taskbarHwnd, out lpRect);
            return lpRect.Bottom - lpRect.Top < 35;
        }

        public WinAPI.RECT GetMinimizeRect()
        {
            WinAPI.RECT lpRect;
            WinAPI.GetWindowRect(this.minimizeHwnd, out lpRect);
            return lpRect;
        }

        public WinAPI.RECT GetTrayRect()
        {
            WinAPI.RECT lpRect;
            WinAPI.GetWindowRect(this.trayHwnd, out lpRect);
            return lpRect;
        }

        public WinAPI.RECT GetRebarRect()
        {
            WinAPI.RECT lpRect;
            WinAPI.GetWindowRect(this.rebarHwnd, out lpRect);
            return lpRect;
        }

        public void MoveTrayToLeft()
        {
            RECT trayRect = GetTrayRect(); //rect of tray area
            RECT minimizeRect = GetMinimizeRect(); //rect of Show desktop button

            int minimizeWidth = minimizeRect.Right - minimizeRect.Left; //width of Show desktop button

            RECT rebarRect = GetRebarRect();

            RECT trayClockRect;

            GetWindowRect(FindWindowEx(this.trayHwnd, IntPtr.Zero, "TrayClockWClass", null), out trayClockRect);

            int trayClockWidth = trayClockRect.Right - trayClockRect.Left;

            RECT sysPagerRect;
            GetWindowRect(FindWindowEx(this.trayHwnd, IntPtr.Zero, "SysPager", null), out sysPagerRect);

            int sysPagerWidth = sysPagerRect.Right - sysPagerRect.Left;

            RECT buttonRect;
            GetWindowRect(FindWindowEx(this.trayHwnd, IntPtr.Zero, "Button", null), out buttonRect);

            int buttonWidth = buttonRect.Right - buttonRect.Left;

            RECT langIndicatorRect;
            GetWindowRect(FindWindowEx(this.trayHwnd, IntPtr.Zero, "TrayInputIndicatorWClass", null), out langIndicatorRect);

            int langIndicatorWidth = langIndicatorRect.Right - langIndicatorRect.Left;

            RECT actionCenterButtonRect;
            GetWindowRect(FindWindowEx(this.trayHwnd, IntPtr.Zero, "TrayButton", null), out actionCenterButtonRect);

            int actionCenterButtonWidth = actionCenterButtonRect.Right - actionCenterButtonRect.Left;

            SetWindowPos(this.trayHwnd, IntPtr.Zero,
                SystemInformation.PrimaryMonitorSize.Width - minimizeWidth - actionCenterButtonWidth - reservedWidth - trayClockWidth - langIndicatorWidth - sysPagerWidth - buttonWidth, 0,
                minimizeWidth + reservedWidth + actionCenterButtonWidth + trayClockWidth + langIndicatorWidth + sysPagerWidth + buttonWidth, trayRect.Bottom - trayRect.Top, (SetWindowPosFlags)0);

            RECT trayRectNew;
            GetWindowRect(this.trayHwnd, out trayRectNew);

            trayRect = this.GetTrayRect();
            int trayWidth = trayRect.Right - trayRect.Left;

            RECT minimizeRectNew;

            GetWindowRect(minimizeHwnd, out minimizeRectNew);

            SetWindowPos(this.minimizeHwnd, IntPtr.Zero, trayWidth - minimizeWidth, 0, minimizeWidth, trayRect.Bottom - trayRect.Top, SetWindowPosFlags.SWP_NOSIZE);

            if (minimizeWidth == 0)
                minimizeWidth = 15;

            control.Left = trayWidth - this.control.Width - minimizeWidth - 4;
        }

        public void PlaceMinimizeOnTaskbar()
        {
            WinAPI.SetParent(this.minimizeHwnd, this.taskbarHwnd);
            WinAPI.RECT minimizeRect = this.GetMinimizeRect();
            int cx = minimizeRect.Right - minimizeRect.Left;
            WinAPI.SetWindowPos(this.minimizeHwnd, IntPtr.Zero, SystemInformation.WorkingArea.Right - cx, 0, cx, minimizeRect.Bottom - minimizeRect.Top, WinAPI.SetWindowPosFlags.SWP_NOSIZE);
            WinAPI.RECT trayRect = this.GetTrayRect();
            int num = trayRect.Right - trayRect.Left;
            WinAPI.SetWindowPos(this.trayHwnd, IntPtr.Zero, SystemInformation.WorkingArea.Right - this.reservedWidth - num - cx, 0, num - cx, trayRect.Bottom - trayRect.Top, (WinAPI.SetWindowPosFlags)0);
        }

        public void PlaceMinimizeOnTray()
        {
            WinAPI.SetParent(this.minimizeHwnd, this.trayHwnd);
        }

        public bool CheckTrayWidth()
        {
            WinAPI.RECT trayRect = this.GetTrayRect();
            int num1 = trayRect.Right - trayRect.Left;
            WinAPI.RECT minimizeRect = this.GetMinimizeRect();
            int num2 = minimizeRect.Right - minimizeRect.Left;
            WinAPI.RECT lpRect1;
            WinAPI.GetWindowRect(WinAPI.FindWindowEx(this.trayHwnd, IntPtr.Zero, "TrayClockWClass", (string)null), out lpRect1);
            int num3 = lpRect1.Right - lpRect1.Left;
            WinAPI.RECT lpRect2;
            WinAPI.GetWindowRect(WinAPI.FindWindowEx(this.trayHwnd, IntPtr.Zero, "SysPager", (string)null), out lpRect2);
            int num4 = lpRect2.Right - lpRect2.Left;
            WinAPI.RECT lpRect3;
            WinAPI.GetWindowRect(WinAPI.FindWindowEx(this.trayHwnd, IntPtr.Zero, "Button", (string)null), out lpRect3);
            int num5 = lpRect3.Right - lpRect3.Left;
            return num1 == num2 + num3 + num4 + num5 + this.reservedWidth;
        }

        public void ReduceRebarWidth()
        {
            RECT rebarRect = this.GetRebarRect();
            int rebarWidth = rebarRect.Right - rebarRect.Left;

            RECT trayRect = this.GetTrayRect();
            int trayWidth = trayRect.Right - trayRect.Left;

            RECT minimizeRect = this.GetMinimizeRect();
            int minimizeWidth = minimizeRect.Right - minimizeRect.Left;

            SetWindowPos(rebarHwnd, IntPtr.Zero, rebarRect.Left, 0, SystemInformation.PrimaryMonitorSize.Width - rebarRect.Left - trayWidth, rebarRect.Bottom - rebarRect.Top, WinAPI.SetWindowPosFlags.SWP_NOMOVE);
        }

        public void ReserveSpace(int width)
        {
            if (this.spaceReserved)
                return;
            this.reservedWidth = width;
            this.spaceReserved = true;
        }

        public void FreeSpace()
        {
            this.reservedWidth = 0;
            WinAPI.RECT trayRect = this.GetTrayRect();
            WinAPI.RECT rebarRect = this.GetRebarRect();
            WinAPI.RECT minimizeRect = this.GetMinimizeRect();
            int cx1 = minimizeRect.Right - minimizeRect.Left;
            WinAPI.RECT lpRect1;
            WinAPI.GetWindowRect(WinAPI.FindWindowEx(this.trayHwnd, IntPtr.Zero, "TrayClockWClass", (string)null), out lpRect1);
            int num1 = lpRect1.Right - lpRect1.Left;
            WinAPI.RECT lpRect2;
            WinAPI.GetWindowRect(WinAPI.FindWindowEx(this.trayHwnd, IntPtr.Zero, "SysPager", (string)null), out lpRect2);
            int num2 = lpRect2.Right - lpRect2.Left;
            WinAPI.RECT lpRect3;
            WinAPI.GetWindowRect(WinAPI.FindWindowEx(this.trayHwnd, IntPtr.Zero, "Button", (string)null), out lpRect3);
            int num3 = lpRect3.Right - lpRect3.Left;
            int cx2 = cx1 + num1 + num2 + num3;
            WinAPI.SetWindowPos(this.trayHwnd, IntPtr.Zero, SystemInformation.WorkingArea.Right - cx2, 0, cx2, trayRect.Bottom - trayRect.Top, (WinAPI.SetWindowPosFlags)0);
            WinAPI.SetWindowPos(this.rebarHwnd, IntPtr.Zero, rebarRect.Left, 0, SystemInformation.WorkingArea.Right - cx2 - rebarRect.Left, trayRect.Bottom - trayRect.Top, WinAPI.SetWindowPosFlags.SWP_NOMOVE);
            WinAPI.SetWindowPos(this.minimizeHwnd, IntPtr.Zero, cx2 - cx1, 0, cx1, minimizeRect.Bottom - minimizeRect.Top, WinAPI.SetWindowPosFlags.SWP_NOSIZE);
        }

        public void CheckTaskbar()
        {
            if (this.spaceReserved)
            {
                if (this.CheckTrayWidth())
                    return;
                this.currentTaskbarPos = this.DetectTaskbarPos();
                if (this.currentTaskbarPos == TaskbarPosition.Bottom)
                    this.TrayResized((object)null, EventArgs.Empty);
            }
            else
                this.ReserveSpace(36);
        }

        public void AddControl(UserControl control)
        {
            WinAPI.RECT trayRect = this.GetTrayRect();
            int num1 = trayRect.Right - trayRect.Left;
            WinAPI.RECT minimizeRect = this.GetMinimizeRect();
            int num2 = minimizeRect.Right - minimizeRect.Left;
            this.reservedWidth = control.Width + 5;
            control.Left = num1 - num2;
            WinAPI.SetParent(control.Handle, this.trayHwnd);
            this.control = (Control)control;
        }

        public void Dispose()
        {
            if (!this.spaceReserved)
                return;
            this.FreeSpace();
        }
    }
}
