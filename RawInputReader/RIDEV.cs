using System;

namespace RawInputReader
{
    [Flags]
    public enum RIDEV
    {
        RIDEV_NONE = 0,
        RIDEV_REMOVE = 0x00000001,
        RIDEV_EXCLUDE = 0x00000010,
        RIDEV_PAGEONLY = 0x00000020,
        RIDEV_NOLEGACY = 0x00000030,
        RIDEV_INPUTSINK = 0x00000100,
        RIDEV_CAPTUREMOUSE = 0x00000200,
        RIDEV_NOHOTKEYS = 0x00000200,
        RIDEV_APPKEYS = 0x00000400,
        RIDEV_EXINPUTSINK = 0x00001000,
        RIDEV_DEVNOTIFY = 0x00002000,
        RIDEV_EXMODEMASK = 0x000000F0,
    }
}
