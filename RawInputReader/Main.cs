using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RawInputReader.Utilities;

namespace RawInputReader
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            Icon = Resources.RawInputReader_icon;
            textBoxLog.MaxLength = int.MaxValue;
        }

        private void ClearLogToolStripMenuItem_Click(object sender, EventArgs e) => ClearLog();
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Close();
        private void AboutRawInputReaderToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var asm = Assembly.GetEntryAssembly()!;
            MessageBox.Show(
                asm.GetCustomAttribute<AssemblyTitleAttribute>()!.Title + " - " + (IntPtr.Size == 4 ? "32" : "64") + "-bit" + Environment.NewLine + asm.GetCustomAttribute<AssemblyCopyrightAttribute>()!.Copyright,
                asm.GetCustomAttribute<AssemblyTitleAttribute>()!.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ReadInputsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new InputForm();
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            var device = new RAWINPUTDEVICE
            {
                dwFlags = dlg.Input.Flags,
                hwndTarget = Handle,
                usUsagePage = dlg.Input.Page,
                usUsage = dlg.Input.Usage
            };

            if (!RegisterRawInputDevices(new[] { device }, 1, Marshal.SizeOf<RAWINPUTDEVICE>()))
            {
                this.ShowError(new Win32Exception(Marshal.GetLastWin32Error()).Message);
                return;
            }

            AppendLog("Registered raw input: " + dlg.Input);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_INPUT:
                    OnInput(ref m);
                    break;

                case WM_INPUT_DEVICE_CHANGE:
                    OnInputDeviceChange(ref m);
                    break;

            }
            base.WndProc(ref m);
        }

        private void OnInput(ref Message m)
        {
            var str = nameof(WM_INPUT);
            if (m.WParam == RIM_INPUT)
            {
                str += " " + nameof(RIM_INPUT);
            }
            else if (m.WParam == RIM_INPUTSINK)
            {
                str += " " + nameof(RIM_INPUTSINK);
            }

            var header = GetHeader(m.LParam);
            if (header != null)
            {
                str += " type:" + header.Value.dwType + " size:" + header.Value.dwSize;
                var name = GetDeviceName(header.Value.hDevice);
                if (name != null)
                {
                    str += " '" + name + "'";
                }
            }

            AppendLog(str);
        }

        private void OnInputDeviceChange(ref Message m)
        {
            var str = nameof(WM_INPUT_DEVICE_CHANGE);
            if (m.WParam == GIDC_ARRIVAL)
            {
                str += " " + nameof(GIDC_ARRIVAL);
            }
            else if (m.WParam == GIDC_REMOVAL)
            {
                str += " " + nameof(GIDC_REMOVAL);
            }

            var name = GetDeviceName(m.LParam);
            if (name != null)
            {
                str += " '" + name + "'";
            }

            var info = GetDeviceInfo(m.LParam);
            if (info != null)
            {
                str += " " + info.ToString();
            }

            AppendLog(str);
        }

        private void ClearLog()
        {
            try
            {
                textBoxLog.Clear();
            }
            catch (Exception)
            {
                // race condition on disposed textBoxLog, do nothing
            }
        }

        private void AppendLog(string? text = null)
        {
            var handle = (textBoxLog?.Handle).GetValueOrDefault();
            if (handle == 0)
                return;

            if (text != null)
            {
                text = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " [" + Environment.CurrentManagedThreadId + "]: " + text;
            }

            // we use this because system doesn't work good under heavy logging
            WinformsUtilities.ReplaceWindowText(handle, text + Environment.NewLine);
        }

        private static RAWINPUTHEADER? GetHeader(IntPtr handle)
        {
            var size = 0;
            var sizef = Marshal.SizeOf<RAWINPUTHEADER>();
            _ = GetRawInputData(handle, RID.RID_HEADER, 0, ref size, sizef);
            if (size == 0)
                return null;

            var ptr = Marshal.AllocHGlobal((size + 1) * 2);
            try
            {
                _ = GetRawInputData(handle, RID.RID_HEADER, ptr, ref size, sizef);
                return Marshal.PtrToStructure<RAWINPUTHEADER>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        private static string? GetDeviceName(IntPtr handle)
        {
            var size = 0;
            _ = GetRawInputDeviceInfo(handle, RIDI.RIDI_DEVICENAME, 0, ref size);
            if (size == 0)
                return null;

            var ptr = Marshal.AllocHGlobal((size + 1) * 2);
            try
            {
                _ = GetRawInputDeviceInfo(handle, RIDI.RIDI_DEVICENAME, ptr, ref size);
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        private static RID_DEVICE_INFO? GetDeviceInfo(IntPtr handle)
        {
            var size = 0;
            _ = GetRawInputDeviceInfo(handle, RIDI.RIDI_DEVICEINFO, 0, ref size);
            if (size == 0)
                return null;

            var ptr = Marshal.AllocHGlobal((size + 1) * 2);
            try
            {
                _ = GetRawInputDeviceInfo(handle, RIDI.RIDI_DEVICEINFO, ptr, ref size);
                return Marshal.PtrToStructure<RID_DEVICE_INFO>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        private const int WM_INPUT_DEVICE_CHANGE = 0x00FE;
        private const int WM_INPUT = 0x00FF;
        private const int GIDC_ARRIVAL = 1;
        private const int GIDC_REMOVAL = 2;
        private const int RIM_INPUT = 0;
        private const int RIM_INPUTSINK = 1;

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint GetRawInputData(IntPtr hRawInput, RID uiCommand, IntPtr pData, ref int pcbSize, int cbSizeHeader);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint GetRawInputDeviceInfo(IntPtr hDevice, RIDI uiCommand, IntPtr pData, ref int pcbSize);

        [DllImport("user32", SetLastError = true)]
        private static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, int uiNumDevices, int cbSize);

        private enum RIM
        {
            RIM_TYPEMOUSE = 0,
            RIM_TYPEKEYBOARD = 1,
            RIM_TYPEHID = 2,
        }

        private enum RID
        {
            RID_HEADER = 0x10000005,
            RID_INPUT = 0x10000003,
        }

        private enum RIDI
        {
            RIDI_PREPARSEDDATA = 0x20000005,
            RIDI_DEVICENAME = 0x20000007,
            RIDI_DEVICEINFO = 0x2000000B,
        }

        [Flags]
        private enum MOUSE_FLAGS : ushort
        {
            MOUSE_MOVE_RELATIVE = 0x0,
            MOUSE_MOVE_ABSOLUTE = 0x1,
            MOUSE_VIRTUAL_DESKTOP = 0x2,
            MOUSE_ATTRIBUTES_CHANGED = 0x4,
            MOUSE_MOVE_NOCOALESCE = 0x8,
        }

        [Flags]
        private enum KEYBOARD_FLAGS : ushort
        {
            RI_KEY_MAKE = 0x0,
            RI_KEY_BREAK = 0x1,
            RI_KEY_E0 = 0x2,
            RI_KEY_E1 = 0x4,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTDEVICE
        {
            public HID_USAGE_PAGE usUsagePage;
            public ushort usUsage;
            public RIDEV dwFlags;
            public IntPtr hwndTarget;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTHEADER
        {
            public RIM dwType;
            public int dwSize;
            public IntPtr hDevice;
            public IntPtr wParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWMOUSE
        {
            public MOUSE_FLAGS usFlags;
            public uint ulButtons;
            public uint ulRawButtons;
            public int lLastX;
            public int lLastY;
            public uint ulExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWKEYBOARD
        {
            public ushort MakeCode;
            public KEYBOARD_FLAGS Flags;
            public ushort Reserved;
            public ushort VKey;
            public uint Message;
            public uint ExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RID_DEVICE_INFO_HID
        {
            public int dwVendorId;
            public int dwProductId;
            public int dwVersionNumber;
            public HID_USAGE_PAGE usUsagePage;
            public ushort usUsage;

            public override string ToString() => $"Vendor 0x{dwVendorId.ToString("X8")} Product 0x{dwProductId.ToString("X8")} Page {usUsagePage} Usage 0x{usUsage.ToString("X4")}";
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RID_DEVICE_INFO_KEYBOARD
        {
            public int dwType;
            public int dwSubType;
            public int dwKeyboardMode;
            public int dwNumberOfFunctionKeys;
            public int dwNumberOfIndicators;
            public int dwNumberOfKeysTotal;

            public override string ToString() => $"Type 0x{dwType.ToString("X8")} Keys {dwNumberOfKeysTotal}";
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RID_DEVICE_INFO_MOUSE
        {
            public int dwId;
            public int dwNumberOfButtons;
            public int dwSampleRate;
            public bool fHasHorizontalWheel;

            public override string ToString() => $"Id 0x{dwId.ToString("X8")} Buttons {dwNumberOfButtons} SampleRate {dwSampleRate}";
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct RID_DEVICE_INFO
        {
            [FieldOffset(0)]
            public int cbSize;

            [FieldOffset(4)]
            public RIM dwType;

            [FieldOffset(8)]
            public RID_DEVICE_INFO_MOUSE mouse;

            [FieldOffset(8)]
            public RID_DEVICE_INFO_KEYBOARD keyboard;

            [FieldOffset(8)]
            public RID_DEVICE_INFO_HID hid;

            public override string ToString()
            {
                var str = "Size " + cbSize + " ";
                switch (dwType)
                {
                    case RIM.RIM_TYPEMOUSE:
                        str += "Mouse " + mouse;
                        break;

                    case RIM.RIM_TYPEKEYBOARD:
                        str += "Keyboard " + keyboard;
                        break;

                    case RIM.RIM_TYPEHID:
                        str += "Hid " + hid;
                        break;
                }
                return str;
            }
        }
    }
}