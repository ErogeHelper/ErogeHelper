﻿using ErogeHelper;
using SplashScreenGdip;
using System.Diagnostics;
using System.IO.Pipes;

if (args.Length == 1 && int.TryParse(args[0], out var pid)) 
{
    Run(Process.GetProcessById(pid)); 
    return;
}

#region Arguments Check
if (args.Length == 0)
{
#if RELEASE
    MessageBox.Show(Strings.App_StartNoParameter);
#else
    MessageBox.Show($"{Strings.App_StartNoParameter}({(DateTime.Now - Process.GetCurrentProcess().StartTime).Milliseconds})");
#endif
    return;
}
var gamePath = args[0];
if (File.Exists(gamePath) && Path.GetExtension(gamePath).Equals(".lnk", StringComparison.OrdinalIgnoreCase))
{
    gamePath = WinShortcutWrapper(gamePath);
}
if (!File.Exists(gamePath))
{
    MessageBox.Show(Strings.App_StartInvalidPath + $" \"{gamePath}\"");
    return;
}
#endregion

var stream = typeof(Program).Assembly.GetManifestResourceStream("ErogeHelper.assets.klee.png")!;
var splash = new SplashScreen(96, stream);
new Thread(() => PreProcessing(args.Contains("-le"), gamePath, splash)).Start();

splash.Run();


// Wrapper for lazy load dlls (50ms startup speed)
static string WinShortcutWrapper(string gamePath) =>
    WindowsShortcutFactory.WindowsShortcut.Load(gamePath).Path ?? "Resolve lnk file failed";

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
        splash.Close();
        MessageBox.Show(Strings.App_LENotSetup);
        return;
    }
    catch (ArgumentException ex) when (ex.Message != string.Empty)
    {
        splash.Close();
        MessageBox.Show(Strings.App_LENotFound + ex.Message);
        return;
    }
    catch (InvalidOperationException)
    {
        splash.Close();
        MessageBox.Show(Strings.App_LENotSupport);
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

    try
    {
        _ = game.HasExited;
    }
    catch (System.ComponentModel.Win32Exception)
    {
        splash.Close();
        MessageBox.Show(Strings.App_ElevatedError);
        return;
    }
    #endregion

    Run(game, splash);
    
    // prevent exception when startup
    splash.Close();
}

static void Run(Process game, SplashScreen? splash = null)
{
    var pipeServer = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
    _ = new IpcMain(pipeServer);

    if (splash != null) IpcMain.Once("Loaded", splash!.Close);

    Environment.CurrentDirectory = AppContext.BaseDirectory;
    while (!game.HasExited)
    {
        var gameWindowHandle = AppLauncher.FindMainWindowHandle(game);
        if (gameWindowHandle == IntPtr.Zero) // process exit
        {
            splash?.Close();
            break;
        }
        else if (gameWindowHandle.ToInt32() == -1) // FindHandleFailed
        {
            splash?.Close();
            MessageBox.Show(Strings.App_Timeout);
            break;
        }

        if (Toast.IsAdmin) _ = Task.Run( () => Toast.Send("ErogeHelper is running as admin", 5));
        var touch = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ErogeHelper.AssistiveTouch.exe",
                Arguments = pipeServer.GetClientHandleAsString() + ' ' + gameWindowHandle,
                UseShellExecute = false,
            }
        };

        touch.Start();

        if (AppdataRoming.UseEnterKeyMapping())
        {
            // Exited with touch
            ProcessStart.GlobalKeyHook(touch.Id, gameWindowHandle);
        }
    
        touch.WaitForExit();
    }
}
