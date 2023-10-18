using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RawInputReader.Utilities
{
    public static class WinformsUtilities
    {
        public static string ApplicationName => AssemblyUtilities.GetTitle()!;
        public static string ApplicationVersion => AssemblyUtilities.GetFileVersion()!;
        public static string ApplicationTitle => ApplicationName + " V" + ApplicationVersion;

        public static void ShowMessage(this IWin32Window owner, string text) => MessageBox.Show(owner, text, ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        public static DialogResult ShowConfirm(this IWin32Window owner, string text) => MessageBox.Show(owner, text, ApplicationTitle + " - " + "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
        public static DialogResult ShowQuestion(this IWin32Window owner, string text) => MessageBox.Show(owner, text, ApplicationTitle + " - " + "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        public static void ShowError(this IWin32Window owner, string text) => MessageBox.Show(owner, text, ApplicationTitle + " - " + "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        public static void ShowWarning(this IWin32Window owner, string text) => MessageBox.Show(owner, text, ApplicationTitle + " - " + "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public static string GetWindowText(IntPtr handle)
        {
            var len = GetWindowTextLengthW(handle);
            var sb = new StringBuilder(len + 2);
            _ = GetWindowText(handle, sb, sb.Capacity);
            return sb.ToString();
        }

        public static void ReplaceWindowText(IntPtr handle, string text, bool canBeUndone = false)
        {
            SetLastError(0);
            var len = GetWindowTextLengthW(handle);
            if (len == 0)
            {
                var gle = Marshal.GetLastWin32Error();
                if (gle != 0)
                    return;
            }

            _ = SendMessage(handle, EM_SETSEL, len, len);
            _ = SendMessage(handle, EM_REPLACESEL, canBeUndone ? 1 : 0, text ?? string.Empty);
            len = GetWindowTextLengthW(handle);
            if (len > 0)
            {
                _ = SendMessage(handle, EM_SETSEL, len - 1, len);
            }
        }

        private const int EM_SETSEL = 0xB1;
        private const int EM_REPLACESEL = 0xC2;

        [DllImport("user32", SetLastError = true)]
        private static extern int GetWindowTextLengthW(IntPtr handle);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr handle, StringBuilder lpString, int nMaxCount);

        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr handle, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr handle, int msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("kernel32")]
        private static extern bool SetLastError(int error);
    }
}
