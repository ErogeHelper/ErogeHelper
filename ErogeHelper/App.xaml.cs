﻿using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Windows.Threading;

namespace ErogeHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private App()
        {
            // UNDONE: Check singleton app
            // see Windows.UI.Notifications; or ToastNotifications
            // Toast user ErogeHelper is running, or you can turn ErogeHelper down immediately

            // Enable Pointer for touch device
            //AppContext.SetSwitch("Switch.System.Windows.Input.Stylus.EnablePointerSupport", true);

            // Set environment to app directory
            var currentDirectory = Path.GetDirectoryName(GetType().Assembly.Location);
            Directory.SetCurrentDirectory(currentDirectory ?? 
                                          throw new ArgumentNullException(nameof(currentDirectory), 
                                              @"Could not located Eroge Helper's directory"));

            // Set logger
            Serilog.Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
                // VS Output
                .WriteTo.Debug(outputTemplate:
                    "[{Timestamp:MM-dd-yyyy HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
#else
            .MinimumLevel.Information()
#endif
                .CreateLogger();

            // Set i18n
            SetLanguageDictionary();

            // Set thread error handle
            AppDomain.CurrentDomain.UnhandledException += (_, unhandledExceptionArgs) =>
            {
                if (Dispatcher.FromThread(Thread.CurrentThread) is null || 
                    Dispatcher.CurrentDispatcher.Thread == Thread.CurrentThread)
                    return;

                var ex = unhandledExceptionArgs.ExceptionObject as Exception ?? new Exception("???");

                // UNDONE: Use https://github.com/ookii-dialogs/ookii-dialogs-wpf TaskDialog
                ModernWpf.MessageBox.Show(ex.ToString());

                Log.Fatal(ex);
            };
            DispatcherUnhandledException += (_, dispatcherUnhandledExceptionEventArgs) =>
            {
                // More friendly
                //dispatcherUnhandledExceptionEventArgs.Handled = true;

                var ex = dispatcherUnhandledExceptionEventArgs.Exception;

                ModernWpf.MessageBox.Show(ex.ToString(), "UI");

                Log.Error(ex);
            };
        }

        private static void SetLanguageDictionary()
        {
            Language.Strings.Culture = Thread.CurrentThread.CurrentCulture.ToString() switch
            {
                "zh-CN" => new System.Globalization.CultureInfo("zh-Hans"),
                "zh-Hans" => new System.Globalization.CultureInfo("zh-Hans"),
                // Default english because there can be so many different system language, we rather fallback on
                // english in this case.
                _ => new System.Globalization.CultureInfo(string.Empty),
            };
            // UNDONE: Delete those directories which not defined in the ErogeHelper.Language project after build
            // But define all the the name in this dictionary switcher
            // 真的没有和他文件夹一样多的国家与地区，上一个版本就很好。选择性的增加语言还是明智之举
        }
    }
}
