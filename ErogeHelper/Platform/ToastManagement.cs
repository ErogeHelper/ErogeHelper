﻿using System.Diagnostics;
using System.Security.Principal;
using CommunityToolkit.WinUI.Notifications;
using ErogeHelper.Shared;
using Splat;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace ErogeHelper.Platform;

public static class ToastManagement
{
    private const int ToastDurationTime = 5000;

    // Tip: CustomNotification
    // https://github.com/rafallopatka/ToastNotifications/blob/master-v2/Docs/CustomNotificatios.md
    private static readonly Notifier DesktopNotifier = new(cfg =>
    {
        cfg.PositionProvider =
            new PrimaryScreenPositionProvider(
                corner: Corner.BottomRight,
                offsetX: 16,
                offsetY: 12);

        cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
            notificationLifetime: TimeSpan.FromMilliseconds(ToastDurationTime),
            maximumNotificationCount: MaximumNotificationCount.UnlimitedNotifications());

        cfg.DisplayOptions.TopMost = true;
    });

    // TODO: Refactor to service register pattern
    public static void Register() =>
        ToastNotificationManagerCompat.OnActivated += toastArgs =>
        {
            if (toastArgs.Argument.Length == 0)
            {
                LogHost.Default.Debug("Toast Clicked");
                return;
            }
            var toastArguments = ToastArguments.Parse(toastArgs.Argument);
            LogHost.Default.Debug(toastArguments.ToString());
        };

    public static void Show(string mainText)
    {
        if (Utils.HasWinRT)
        {
            new ToastContentBuilder()
                .AddText(mainText)
                .Show(toast =>
                {
                    toast.Group = "eh";
                    toast.Tag = "eh";
                    // ExpirationTime bugged with InvalidCastException in .Net5
                    // ExpirationTime can not work and bugged with using
                    // ToastNotificationManagerCompat.History.Clear() in .Net6
                    //toast.ExpirationTime = DateTime.Now.AddSeconds(5);
                });

            Thread.Sleep(ToastDurationTime);
            ToastNotificationManagerCompat.History.Clear();
        }
        else
        {
            DesktopNotifier.ShowInformation(
                mainText,
                new MessageOptions { ShowCloseButton = false, FreezeOnMouseEnter = false });
        }
    }

    public static async Task ShowAsync(string mainText, Stopwatch toastLifetimeTimer)
    {
        if (Utils.HasWinRT)
        {
            new ToastContentBuilder()
                .AddText(mainText)
                .Show(toast =>
                {
                    toast.Group = "eh";
                    toast.Tag = "eh";
                    // ExpirationTime bugged with InvalidCastException in .Net5
                    // ExpirationTime can not work and bugged with using
                    // ToastNotificationManagerCompat.History.Clear() in .Net6
                    //toast.ExpirationTime = DateTime.Now.AddSeconds(5);
                });

            toastLifetimeTimer.Restart();
            await Task.Delay(ToastDurationTime).ConfigureAwait(false);
            if (toastLifetimeTimer.ElapsedMilliseconds >= ToastDurationTime)
            {
                ToastNotificationManagerCompat.History.Clear();
                toastLifetimeTimer.Stop();
            }
        }
        else
        {
            DesktopNotifier.ShowInformation(
                mainText,
                new MessageOptions { ShowCloseButton = false, FreezeOnMouseEnter = false });
        }
    }

    public static void IfAdminThenToast()
    {
        var current = WindowsIdentity.GetCurrent();
        var windowsPrincipal = new WindowsPrincipal(current);
        if (!windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
            return;

        Show("ErogeHelper is running in Admin");
    }
}
