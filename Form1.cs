using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace WinFormsSystemEventsHangRepro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region P/Invoke

        [DllImport("USER32.dll", ExactSpelling = true, EntryPoint = "SendMessageTimeoutW", SetLastError = true)]
        internal static extern nint SendMessageTimeout(nint hWnd, uint Msg, nint wParam, nint lParam, uint fuFlags, uint uTimeout, IntPtr lpdwResult);

        #endregion

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            var lp = Marshal.StringToHGlobalUni("TapsEnabled");
            SendMessageTimeout(0x0000FFFF /* HWND_BROADCAST */, 26 /* WM_SETTINGCHANGE */, 0, lp, 0, 1000, 0);
        }
    }
}