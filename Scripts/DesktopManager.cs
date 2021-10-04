using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DesktopSnakeGame
{
    public class DesktopManager
    {
        private DesktopPoint[] initialIconsPosition;
        private IntPtr desktopHandler;
        public int iconsCount;

        #region Singleton
        private DesktopManager()
        {
            desktopHandler = GetDesktopWindow();
        }

        static DesktopManager instance;

        public static DesktopManager Instance
        {
            get
            {
                return instance ?? (instance = new DesktopManager());
            }
        }

        #endregion

        public void ResetDesktop()
        {
            if (initialIconsPosition == null)
                return;

            SetIconsPositions(initialIconsPosition);
            SetIconsOnScreen(iconsCount);
        }

        public static Vector2 GetTileSize()
        {
            int iconSize = DesktopIconSize();
            int horizontalSpacing = 75;
            int verticalSpacing = iconSize + 52;

            if (iconSize == 96)
            {
                horizontalSpacing = iconSize + 10;
            }

            return new Vector2(horizontalSpacing, verticalSpacing);
        }

        public void Update()
        {
            //Get amount of icons on desktop
            iconsCount = GetIconsCount();
            //Initializing the array that will save the icons positions
            initialIconsPosition = new DesktopPoint[iconsCount];
            //Saving icons positions to initialized array
            SaveAllIconsPositions(iconsCount);
        }

        static public Vector2 ScreenResolution()
        {
            return new Vector2(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);          
        }

        static public int DesktopIconSize()
        {
            int IconSize = -1;

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\Shell\\Bags\\1\\Desktop")) 
                {
                    if (key != null)
                    {
                        IconSize = (int)key.GetValue("IconSize");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            return IconSize;
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
            int pointSize = Marshal.SizeOf(typeof(DesktopPoint));

            uint desktopProcessID;
            Win32.GetWindowThreadProcessId(desktopHandler, out desktopProcessID);
            IntPtr desktopProcessHandle = Win32.OpenProcess(Win32.PROCESS_VM_OPERATION | Win32.PROCESS_VM_WRITE | Win32.PROCESS_VM_READ, false, desktopProcessID);

            IntPtr allocMemAddress = Win32.VirtualAllocEx(desktopProcessHandle, IntPtr.Zero, pointSize, Win32.MEM_COMMIT | Win32.MEM_RESERVE, Win32.PAGE_READWRITE);

            DesktopPoint[] points = new DesktopPoint[1];
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

                    initialIconsPosition[i] = (DesktopPoint)Marshal.PtrToStructure(pointPtr, typeof(DesktopPoint));
                }
            }
            finally
            {
                Win32.VirtualFreeEx(desktopProcessHandle, allocMemAddress, 0, Win32.RELEASE);
            }
        }

        public void SetIconsPositions(DesktopPoint[] iconPositions)
        {
            for(int i = 0; i < iconPositions.Length; i++)
            {
                Win32.SendMessage(desktopHandler, Win32.LVM_SETITEMPOSITION, i, Win32.MakeLParam(iconPositions[i].X, iconPositions[i].Y));
            }
        }

        //Set the amount of icons to be draw on screen
        public void SetIconsOnScreen(int icons)
        {
            Win32.SendMessage(desktopHandler, Win32.LVM_SETITEMCOUNT, icons, (IntPtr)1);
        }

        public void SetIconsSpacing(int cx,int cy)
        {
            Win32.SendMessage(desktopHandler, Win32.LVM_SETICONSPACING, 0, Win32.MakeLParam(cx, cy));
        }
    }
}