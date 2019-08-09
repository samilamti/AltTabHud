using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AltTabHud
{
    public  class WinApi
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetCaretPos(out Point lpPoint);

        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(HandleRef hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WNDCLASSEX
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public int style;
            public IntPtr lpfnWndProc; // not WndProc
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;

            //Use this function to make a new one with cbSize already filled in.
            //For example:
            //var WndClss = WNDCLASSEX.Build()
            public static WNDCLASSEX Build()
            {
                var nw = new WNDCLASSEX();
                nw.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
                return nw;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean GetClassInfoEx(HandleRef hInstance, String lpClassName, ref WNDCLASSEX lpWndClass);

        const int IDC_IBEAM = 32513;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref POINT lpPoint);

        public struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(HandleRef hwnd, String lpString);
    }

    internal static class CapsLockControl
    {
        [DllImport("user32.dll")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "The WinAPI is what it is")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        const byte CapsLockVirtualKey = 0x14;
        const int ExtendedKey = 0x1;
        const int KeyUp = 0x2;
        static readonly UIntPtr ExtraInfo = (UIntPtr)0;

        unsafe public static void PushButton()
        {
            keybd_event(CapsLockVirtualKey, 0x45, ExtendedKey, ExtraInfo);
            keybd_event(CapsLockVirtualKey, 0x45, ExtendedKey | KeyUp, ExtraInfo);
        }
    }
}
