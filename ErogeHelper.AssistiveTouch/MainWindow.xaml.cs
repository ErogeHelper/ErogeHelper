﻿using ErogeHelper.AssistiveTouch.Helper;
using ErogeHelper.Share;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ErogeHelper.AssistiveTouch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static IntPtr Handle { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Handle = new WindowInteropHelper(this).EnsureHandle();

            ContentRendered += (_, _) => IpcRenderer.Send(IpcTypes.Loaded);

            HwndTools.RemovePopupAddChildStyle(Handle);
            User32.SetParent(Handle, App.GameWindowHandle);
            User32.GetClientRect(App.GameWindowHandle, out var rectClient);
            User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, rectClient.Width, rectClient.Height, User32.SetWindowPosFlags.SWP_NOZORDER);

            var hooker = new GameWindowHooker();
        }


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
