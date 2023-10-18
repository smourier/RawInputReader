using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using RawInputReader.Utilities;

namespace RawInputReader
{
    public class RawInput : INotifyPropertyChanged
    {
        private HID_USAGE_PAGE _page = (HID_USAGE_PAGE)ushort.MaxValue;
        private ushort _usage = ushort.MaxValue;

        public event PropertyChangedEventHandler? PropertyChanged;

        public RawInput()
        {
            Page = HID_USAGE_PAGE.HID_USAGE_PAGE_UNDEFINED;
            Usage = 0;
        }

        public HID_USAGE_PAGE Page
        {
            get => _page;
            set
            {
                if (_page == value)
                    return;

                _page = value;
                PageHex = "0x" + ((ushort)Page).ToString("X4");
                OnPropertyChanged();
                OnPropertyChanged(nameof(PageHex));
            }
        }

        [Editor(typeof(EnumEditor), typeof(UITypeEditor))]
        public RIDEV Flags { get; set; } = RIDEV.RIDEV_INPUTSINK | RIDEV.RIDEV_DEVNOTIFY;

        [TypeConverter(typeof(UsageConverter))]
        public ushort Usage
        {
            get => _usage;
            set
            {
                if (_usage == value)
                    return;

                _usage = value;
                UsageHex = "0x" + Usage.ToString("X4");
                OnPropertyChanged();
                OnPropertyChanged(nameof(UsageHex));
            }
        }

        [DisplayName("Usage (Hex)")]
        public string? UsageHex { get; private set; }

        [DisplayName("Page (Hex)")]
        public string? PageHex { get; private set; }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public override string ToString()
        {
            var str = $"Page {Page} ({PageHex}) Usage {UsageHex} Flags {Flags}";
            return str;
        }
    }
}
