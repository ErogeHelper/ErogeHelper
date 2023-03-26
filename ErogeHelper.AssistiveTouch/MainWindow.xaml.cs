﻿using ErogeHelper.AssistiveTouch.Core;
using ErogeHelper.AssistiveTouch.Helper;
using ErogeHelper.AssistiveTouch.NativeMethods;
using ErogeHelper.IpcChannel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace ErogeHelper.AssistiveTouch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Thread RootHwndWatche, Stylus Input are from Wpf 
        public static IntPtr Handle { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Handle = new WindowInteropHelper(this).EnsureHandle();

            ContentRendered += (_, _) => IpcRenderer.Send(IpcTypes.Loaded);

            HwndTools.RemovePopupAddChildStyle(Handle);
            User32.SetParent(Handle, App.GameWindowHandle);
            User32.GetClientRect(App.GameWindowHandle, out var rectClient);
            User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, (int)rectClient.Width, (int)rectClient.Height, User32.SetWindowPosFlags.SWP_NOZORDER);

            var hooker = new GameWindowHooker();
            hooker.SizeChanged += (_, _) => Fullscreen.UpdateFullscreenStatus();
        }

        // TODO: Try https://www.top-password.com/blog/enable-or-disable-touch-feedback-in-windows-10/
        // Use EnableTouchPointer instead but wait until DisableTouchFeedback get fixed
        #region Disable Touch White Point 

        protected override void OnPreviewTouchDown(TouchEventArgs e)
        {
            base.OnPreviewTouchDown(e);
            Cursor = Cursors.None;
        }

        protected override void OnPreviewTouchMove(TouchEventArgs e)
        {
            base.OnPreviewTouchMove(e);
            Cursor = Cursors.None;
        }

        protected override void OnGotMouseCapture(MouseEventArgs e)
        {
            base.OnGotMouseCapture(e);
            Cursor = Cursors.Arrow;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (e.StylusDevice == null)
                Cursor = Cursors.Arrow;
        }

        #endregion
    }
}
