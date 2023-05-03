﻿using ErogeHelper.AssistiveTouch.Core;
using ErogeHelper.AssistiveTouch.Helper;
using ErogeHelper.AssistiveTouch.NativeMethods;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace ErogeHelper.AssistiveTouch;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    // Thread RootHwndWatch, Stylus Input are from Wpf 
    public static IntPtr Handle { get; private set; }

    public double Dpi { get; }

    public MainWindow()
    {
        InitializeComponent();
        Handle = new WindowInteropHelper(this).EnsureHandle();
        Dpi = VisualTreeHelper.GetDpi(this).DpiScaleX;

        ContentRendered += (_, _) => IpcRenderer.Send("Loaded");

        HwndTools.RemovePopupAddChildStyle(Handle);
        User32.SetParent(Handle, App.GameWindowHandle);
        User32.GetClientRect(App.GameWindowHandle, out var rectClient);
        User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, rectClient.Width, rectClient.Height, User32.SetWindowPosFlags.SWP_NOZORDER);

        var hooker = new GameWindowHooker(Handle);
        hooker.SizeChanged += (_, _) => Fullscreen.UpdateFullscreenStatus();

        if (Config.UseEdgeTouchMask)
        {
            Fullscreen.MaskForScreen(this);
        }
    }
}
