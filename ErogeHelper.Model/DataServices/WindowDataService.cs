﻿using ErogeHelper.Model.DataServices.Interface;
using Vanara.PInvoke;

namespace ErogeHelper.Model.DataServices;

public class WindowDataService : IWindowDataService
{
    public void InitMainWindowHandle(HWND handle) => MainWindowHandle = handle;
    public HWND MainWindowHandle { get; private set; }
}
