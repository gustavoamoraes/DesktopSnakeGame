using System;
using System.Runtime.InteropServices;

namespace DesktopSnakeGame
{
    public static class Win32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flAllocationType, uint flProtect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetKeyState(int nVirtKey);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, out uint lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] IntPtr buffer, int size, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint dwFreeType);

        public static IntPtr MakeLParam(int wLow, int wHigh)
        {
            return (IntPtr)((wHigh << 16) | (wLow & 0xFFFF));
        }

        public const uint LVM_FIRST = 4096;
        public const uint LVM_GETITEMCOUNT = 4100;
        public const uint LVM_GETITEMPOSITION = 4112;
        public const uint LVM_SETITEMPOSITION = 0x1000 + 15;
        public const uint LVM_ARRANGE = 4118;
        public const uint LVM_ENABLEGROUPVIEW = 4253;
        public const uint LVM_SETGROUPINFO = 4243;
        public const uint LVM_GETGROUPCOUNT = LVM_FIRST + 152;
        public const uint LVM_SETICONSPACING = 4149;
        public const uint LVM_SETITEMCOUNT = 4143;

        public const int LVA_SNAPTOGRID = 0x0005;
        public const int LVA_DEFAULT = 0x0000;
        public const uint LVA_ALIGNTOP = 0x0002;
        public const uint LVA_ALIGNLEFT = 0x0001;

        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int WM_COMMAND = 0x111;

        // used for memory allocation
        public const uint MEM_COMMIT = 0x00001000;
        public const uint MEM_RESERVE = 0x00002000;
        public const uint PAGE_READWRITE = 4;

        //free mem
        public const uint RELEASE = 0x8000;

        // privileges
        public const int PROCESS_VM_OPERATION = 0x0008;
        public const int PROCESS_VM_WRITE = 0x0020;
        public const int PROCESS_VM_READ = 0x0010;

    }
}

