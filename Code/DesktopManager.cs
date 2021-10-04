using System;
using System.Runtime.InteropServices;

namespace Program
{
    public class DesktopManager
    {
        private Point[] initialIconsPosition;
        public IntPtr desktopHandler;
        //public Vector2 screenResolution;
        public int iconsCount;

        public DesktopManager()
        {
            desktopHandler = GetDesktopWindow();

            //Get amount of icons on desktop
            iconsCount = GetIconsCount();

            //Set icons spacing to custom grid size for a better moving experience
            SetIconsSpacing(GameManager.tileSize);

            //Initializing the array that will save the icons positions
            initialIconsPosition = new Point[iconsCount];

            //Saving icons positions to initialized array
            SaveAllIconsPositions(iconsCount);
        }

        public Vector2 ScreenResolution()
        {
            Win32.DEVMODE devMode = default;
            devMode.dmSize = (short)Marshal.SizeOf(devMode);
            Win32.EnumDisplaySettings(null, Win32.ENUM_CURRENT_SETTINGS, ref devMode);

            return new Vector2(devMode.dmPelsWidth,devMode.dmPelsHeight);
        }

        public IntPtr GetDesktopWindow()
        {
            IntPtr _ProgMan = Win32.GetShellWindow();
            IntPtr _SHELLDLL_DefView = Win32.FindWindowEx(_ProgMan, IntPtr.Zero, "SHELLDLL_DefView", null);
            IntPtr _SysListView32 = Win32.FindWindowEx(_SHELLDLL_DefView, IntPtr.Zero, "SysListView32", null);
            return _SysListView32;
        }

        public int GetIconsCount()
        {
            return (int)Win32.SendMessage(desktopHandler, Win32.LVM_GETITEMCOUNT, 0, IntPtr.Zero);
        }

        private void SaveAllIconsPositions(int iconsCount)
        {
            int pointSize = Marshal.SizeOf(typeof(Point));

            uint desktopProcessID;
            Win32.GetWindowThreadProcessId(desktopHandler, out desktopProcessID);
            IntPtr desktopProcessHandle = Win32.OpenProcess(Win32.PROCESS_VM_OPERATION | Win32.PROCESS_VM_WRITE | Win32.PROCESS_VM_READ, false, desktopProcessID);

            IntPtr allocMemAddress = Win32.VirtualAllocEx(desktopProcessHandle, IntPtr.Zero, pointSize, Win32.MEM_COMMIT | Win32.MEM_RESERVE, Win32.PAGE_READWRITE);

            Point[] points = new Point[1];
            IntPtr pointPtr = Marshal.UnsafeAddrOfPinnedArrayElement(points, 0);

            try
            {
                for (int i = 0; i < iconsCount; i++)
                {
                    uint bytesWritten = 0;
                    Win32.WriteProcessMemory(desktopProcessHandle, allocMemAddress, pointPtr, pointSize, out bytesWritten);

                    Win32.SendMessage(GetDesktopWindow(), Win32.LVM_GETITEMPOSITION, i, allocMemAddress);

                    int bytesRead = 0;
                    Win32.ReadProcessMemory(desktopProcessHandle, allocMemAddress, pointPtr, pointSize, out bytesRead);

                    initialIconsPosition[i] = (Point)Marshal.PtrToStructure(pointPtr, typeof(Point));
                }
            }
            finally
            {

                Win32.VirtualFreeEx(desktopProcessHandle, allocMemAddress, 0, Win32.RELEASE);
            }
        }

        public void SetIconsPositions(Vector2[] iconPositions)
        {
            for(int i = 0;i < iconPositions.Length;i++)
            {
                Win32.SendMessage(desktopHandler, Win32.LVM_SETITEMPOSITION, i, Win32.MakeLParam((int)iconPositions[i].x, (int)iconPositions[i].y));
            }
        }

        //Set the amount of icons to be draw on screen
        public void SetIconsOnScreen(int icons)
        {
            Win32.SendMessage(desktopHandler, Win32.LVM_SETITEMCOUNT, icons, (IntPtr)1);
        }

        private void SetIconsSpacing(int square_size)
        {
            Win32.SendMessage(desktopHandler, Win32.LVM_SETICONSPACING, 0,Win32.MakeLParam(square_size, square_size));
        }
    }
}