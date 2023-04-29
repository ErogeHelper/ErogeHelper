﻿using ErogeHelper.AssistiveTouch.Core;
using ErogeHelper.AssistiveTouch.NativeMethods;
using System.IO.Pipes;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace ErogeHelper.AssistiveTouch;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IntPtr GameWindowHandle { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        var _pipeClient = new AnonymousPipeClientStream(PipeDirection.Out, e.Args[0]);
        _ = new IpcRenderer(_pipeClient);

        GameWindowHandle = (IntPtr)int.Parse(e.Args[1]);

        DisableWPFTabletSupport();

        Config.Load();

        if (Config.UseEnterKeyMapping)
        {
            GlobalKeyHook();
        }

        if (Config.EnableMagpieTouchMapping)
        {
            new MagpieTouchHooker();
        }
    }

    private static void GlobalKeyHook()
    {
        const byte VK_RETURN = 0x0D;
        const uint KEYEVENTF_KEYUP = 0x0002;
        // Thread name MessagePumpingObject
        var keyboard = WindowsInput.Capture.Global.KeyboardAsync();
        keyboard.KeyDown += (_, e) =>
        {
            if (e.Data.Key == Config.MappingKey && User32.GetForegroundWindow() == GameWindowHandle)
            {
                e.Next_Hook_Enabled = false;

                User32.keybd_event(VK_RETURN, 0, 0, 0);
                User32.keybd_event(VK_RETURN, 0, KEYEVENTF_KEYUP, 0);
            }
        };
        Current.Exit += (_, _) => keyboard.Dispose();
    }

    private static void DisableWPFTabletSupport()
    {
        // Get a collection of the tablet devices for this window.
        TabletDeviceCollection devices = Tablet.TabletDevices;

        if (devices.Count > 0)
        {
            // Get the Type of InputManager.  
            Type inputManagerType = typeof(InputManager);

            // Call the StylusLogic method on the InputManager.Current instance.  
            object stylusLogic = inputManagerType.InvokeMember("StylusLogic",
                        BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                        null, InputManager.Current, null)!;

            if (stylusLogic != null)
            {
                //  Get the type of the stylusLogic returned from the call to StylusLogic.  
                Type stylusLogicType = stylusLogic.GetType();

                // Loop until there are no more devices to remove.  
                while (devices.Count > 0)
                {
                    // Remove the first tablet device in the devices collection.  
                    stylusLogicType.InvokeMember("OnTabletRemoved",
                            BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, stylusLogic, new object[] { (uint)0 });
                }
            }

        }
    }
}
