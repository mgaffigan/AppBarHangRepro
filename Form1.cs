using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WinFormsSystemEventsHangRepro
{
    public partial class Form1 : Form
    {
        private static readonly int AppBarMessageId = RegisterWindowMessage(typeof(Form1).FullName!);

        public Form1()
        {
            InitializeComponent();
        }

        #region P/Invoke

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int RegisterWindowMessage(string msg);

        [DllImport("shell32.dll", ExactSpelling = true)]
        public static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        #endregion

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            var abd = GetAppBarData();
            SHAppBarMessage(0 /* ABM_NEW */, ref abd);

            abd = GetAppBarData();
            abd.rc = new RECT { left = 0, top = 0, right = 100, bottom = 1080 };
            SHAppBarMessage(3 /* ABM_SETPOS */, ref abd);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            var abd = GetAppBarData();
            SHAppBarMessage(6 /* ABM_ACTIVATE */, ref abd);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            var abd = GetAppBarData();
            SHAppBarMessage(1 /* ABM_REMOVE */, ref abd);
        }

        private APPBARDATA GetAppBarData() => new APPBARDATA()
        {
            cbSize = Marshal.SizeOf(typeof(APPBARDATA)),
            hWnd = Handle,
            uCallbackMessage = AppBarMessageId,
            uEdge = 0,
        };
    }
}