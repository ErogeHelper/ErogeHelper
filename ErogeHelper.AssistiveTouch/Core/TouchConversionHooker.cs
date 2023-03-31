﻿using ErogeHelper.AssistiveTouch.NativeMethods;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ErogeHelper.AssistiveTouch.Core
{
    public static class TouchConversionHooker
    {
        // User32.MOUSEEVENTF.MOUSEEVENTF_FROMTOUCH
        private const uint MOUSEEVENTF_FROMTOUCH = 0xFF515700;

        private static IntPtr _hookId;

        public static void Install()
        {
            var moduleHandle = Kernel32.GetModuleHandle(); // get current exe instant handle

            _hookId = User32.SetWindowsHookEx(User32.HookType.WH_MOUSE_LL, Hook, moduleHandle, 0); // tid 0 set global hook
            if (_hookId == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public static void UnInstall() => User32.UnhookWindowsHookEx(_hookId);

        private static IntPtr Hook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
                return User32.CallNextHookEx(_hookId!, nCode, wParam, lParam);

            var obj = Marshal.PtrToStructure(lParam, typeof(User32.MSLLHOOKSTRUCT));
            if (obj is not User32.MSLLHOOKSTRUCT info)
                return User32.CallNextHookEx(_hookId!, nCode, wParam, lParam);

            var extraInfo = (uint)info.dwExtraInfo;
            if ((extraInfo & MOUSEEVENTF_FROMTOUCH) == MOUSEEVENTF_FROMTOUCH)
            {
                var isGameWindow = User32.GetForegroundWindow() == App.GameWindowHandle;
                if (!isGameWindow)
                    return User32.CallNextHookEx(_hookId!, nCode, wParam, lParam);

                switch ((int)wParam)
                {
                    case 0x202:
                        Task.Run(() =>
                        {
                            var (x, y) = GetCursorPosition();
                            User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_LEFTDOWN, x, y, 0, IntPtr.Zero);
                            Thread.Sleep(UserTimerMinimum);
                            User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_LEFTUP, x, y, 0, IntPtr.Zero);
                        });
                        break;
                    case 0x205:
                        Task.Run(() =>
                        {
                            var (x, y) = GetCursorPosition();
                            User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_RIGHTDOWN, x, y, 0, IntPtr.Zero);
                            Thread.Sleep(UserTimerMinimum);
                            User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_RIGHTUP, x, y, 0, IntPtr.Zero);
                        });
                        break;
                }
            }

            return User32.CallNextHookEx(_hookId!, nCode, wParam, lParam);
        }

        private const int UserTimerMinimum = 0x0000000A;

        private static (int X, int Y) GetCursorPosition()
        {
            var gotPoint = User32.GetCursorPos(out var currentMousePoint);
            if (!gotPoint)
                currentMousePoint = new System.Drawing.Point(0, 0);
            return (currentMousePoint.X, currentMousePoint.Y);
        }
    }
}
