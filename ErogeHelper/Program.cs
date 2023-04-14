﻿using System.Diagnostics;
using ErogeHelper;
using SplashScreenGdip;

#region Arguments Check
if (args.Length == 0)
{
    MessageBox.Show($"{Strings.App_StartNoParameter}({(DateTime.Now - Process.GetCurrentProcess().StartTime).Milliseconds})");
    return;
}
var gamePath = args[0];
if (File.Exists(gamePath) && Path.GetExtension(gamePath).Equals(".lnk", StringComparison.OrdinalIgnoreCase))
{
    gamePath = WindowsShortcutFactory.WindowsShortcut.Load(gamePath).Path ?? "Resolve lnk file failed";
}
if (!File.Exists(gamePath))
{
    MessageBox.Show(Strings.App_StartInvalidPath + $" \"{gamePath}\"");
    return;
}
#endregion

var stream = typeof(Program).Assembly.GetManifestResourceStream("ErogeHelper.assets.klee.png")!;
var splash = new SplashScreen(96, stream);
_ = Task.Run(() => PreProcessing(args.Contains("-le"), gamePath, splash));

splash.Run();

static void PreProcessing(bool leEnable, string gamePath, SplashScreen splash)
{
    #region Start Game
    Process? leProc;
    try
    {
        leProc = AppLauncher.RunGame(gamePath, leEnable);
    }
    catch (ArgumentException ex) when (ex.Message == string.Empty)
    {
        MessageBox.ShowX(Strings.App_LENotSetup, splash);
        return;
    }
    catch (ArgumentException ex) when (ex.Message != string.Empty)
    {
        MessageBox.ShowX(Strings.App_LENotFound + ex.Message, splash);
        return;
    }
    catch (InvalidOperationException)
    {
        MessageBox.ShowX(Strings.App_LENotSupport, splash);
        return;
    }
    var (game, pids) = AppLauncher.ProcessCollect(Path.GetFileNameWithoutExtension(gamePath));
    if (game is null)
    {
        MessageBox.ShowX(Strings.App_Timeout, splash);
        return;
    }
    leProc?.Kill();
    #endregion

    // TODO split
    var wpfThread = new Thread(() => Main(splash, game))
    {
        Name = "メイン スレッド"
    };
    wpfThread.SetApartmentState(ApartmentState.STA);
    wpfThread.Start();
}

static void Main(SplashScreen splash, Process game)
{
    var touch = new ErogeHelper.AssistiveTouch.AppInside();

    NewMethod(splash, game);
    
    touch.Run();

    static void NewMethod(SplashScreen splash, Process game)
    {
         if (game.HasExited)
        {
            System.Windows.Application.Current.Shutdown();
            return;
        }

        var gameWindowHandle = AppLauncher.FindMainWindowHandle(game);
        if (gameWindowHandle == IntPtr.Zero) // process exit
        {
            System.Windows.Application.Current.Shutdown();
            return;
        }
        else if (gameWindowHandle.ToInt32() == -1) // FindHandleFailed
        {
            MessageBox.ShowX(Strings.App_Timeout, splash);
            System.Windows.Application.Current.Shutdown();
            return;
        }


        var mainWindow = new ErogeHelper.AssistiveTouch.MainWindow();
        mainWindow.ContentRendered += (_, _) => splash.Close();

        mainWindow.Closed += (_, _) => 
            System.Windows.Application.Current.Shutdown();
        //mainWindow.Closed += (_, _) => NewMethod(splash, game);
        mainWindow.Show();
    }
};
