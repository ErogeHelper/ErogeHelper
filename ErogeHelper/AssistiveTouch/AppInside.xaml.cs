﻿using System.Reflection;
using System.Windows;
using System.Windows.Input;
using ErogeHelper.AssistiveTouch.Helper;
using ErogeHelper.AssistiveTouch.NativeMethods;
using WindowsInput.Events;

namespace ErogeHelper.AssistiveTouch;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class AppInside : Application
{
    public static IntPtr GameWindowHandle { get; private set; }

    /// <summary>
    /// Constructor only execute once
    /// </summary>
    public AppInside()
    {
        InitializeComponent();
        DisableWPFTabletSupport();

        Config.Load();

        if (Config.EnterKeyMapping)
        {
            GlobalKeyHook();
        }
    }

    private static void GlobalKeyHook()
    {
        const int debounce = 40;
        var throttle = new Throttle(debounce, () =>
            WindowsInput.Simulate.Events()
                .Click(KeyCode.Enter)
                .Invoke());
        // Thread name MessagePumpingObject
        var keyboard = WindowsInput.Capture.Global.KeyboardAsync();
        keyboard.KeyDown += (_, e) =>
        {
            if (e.Data.Key == KeyCode.Z && 
                User32.GetForegroundWindow() == GameWindowHandle)
            {
                e.Next_Hook_Enabled = false;
                throttle.Signal();
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
