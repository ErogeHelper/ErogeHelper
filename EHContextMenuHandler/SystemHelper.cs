﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace EHContextMenuHandler;

internal class SystemHelper
{
    public static bool Is4KDisplay()
    {
        var g = Graphics.FromHwnd(IntPtr.Zero);
        var desktop = g.GetHdc();

        // 10 = VERTRES
        // 90 = LOGPIXELSY
        // 117 = DESKTOPVERTRES
        var logicDpi = GetDeviceCaps(desktop, 10);
        var logY = GetDeviceCaps(desktop, 90);
        var realDpi = GetDeviceCaps(desktop, 117);

        g.ReleaseHdc();

        return (realDpi / logicDpi == 2) || (logY / 96 == 2);
    }

    [DllImport("gdi32.dll")]
    private static extern int GetDeviceCaps(IntPtr hDc, int nIndex);

    public static bool Is64BitOS()
    {
        //The code below is from http://1code.codeplex.com/SourceControl/changeset/view/39074#842775
        //which is under the Microsoft Public License: http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.

        if (IntPtr.Size == 8) // 64-bit programs run only on Win64
        {
            return true;
        }
        // Detect whether the current process is a 32-bit process 
        // running on a 64-bit system.
        bool flag;
        return DoesWin32MethodExist("kernel32.dll", "IsWow64Process") &&
               IsWow64Process(GetCurrentProcess(), out flag) && flag;
    }

    private static bool DoesWin32MethodExist(string moduleName, string methodName)
    {
        var moduleHandle = GetModuleHandle(moduleName);
        if (moduleHandle == IntPtr.Zero)
        {
            return false;
        }
        return GetProcAddress(moduleHandle, methodName) != IntPtr.Zero;
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr GetModuleHandle(string moduleName);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string procName);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);


    public static string RedirectToWow64(string path)
    {
        var system = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\").ToLower();
        var systemWow64 = Environment.ExpandEnvironmentVariables("%SystemRoot%\\SysWOW64\\").ToLower();

        path = Environment.ExpandEnvironmentVariables(path).ToLower();

        return path.Replace(system, systemWow64);
    }
}
