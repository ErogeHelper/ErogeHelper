﻿// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
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

var stream = typeof(Program).Assembly.GetManifestResourceStream("ErogeHelper.assets.klee.png") ?? throw new ArgumentException("stream");
var splash = new SplashScreen(96, stream);
_ = Task.Run(() => splash.Run());

#region Start Game
Process? leProc;
try
{
    leProc = AppLauncher.RunGame(gamePath, args.Contains("-le"));// || or contain le.config file
}
catch (InvalidOperationException)
{
    splash.Close();
    MessageBox.Show(Strings.App_LENotInstall);
    return;
}
catch (ArgumentException ex)
{
    splash.Close();
    MessageBox.Show(Strings.App_LENotFound + ex.Message);
    return;
}
var (game, pids) = AppLauncher.ProcessCollect(Path.GetFileNameWithoutExtension(gamePath));
if (game is null)
{
    splash.Close();
    MessageBox.Show(Strings.App_Timeout);
    return;
}
leProc?.Kill();
#endregion

ErogeHelper.AssistiveTouch.MainWindow.CloseSplash = splash.Close;

Environment.CurrentDirectory = AppContext.BaseDirectory;
Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
while (!game.HasExited)
{
    var gameWindowHandle = AppLauncher.FindMainWindowHandle(game);
    if (gameWindowHandle == IntPtr.Zero) // process exit
    {
        break;
    }
    else if (gameWindowHandle.ToInt32() == -1) // FindHandleFailed
    {
        MessageBox.Show(Strings.App_Timeout);
        break;
    }

    var touch = new ErogeHelper.AssistiveTouch.AppInside(gameWindowHandle);

    touch.Run();
}
