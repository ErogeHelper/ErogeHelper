﻿using ErogeHelper.Common.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ErogeHelper.Model.Service.Interface
{
    public interface IGameWindowHooker
    {
        event Action<GameWindowPosition> GamePosArea;

        event Action<GameWindowPositionChanged> GamePosChanged;

        event Action<WindowSize> NewWindowSize;

        Task SetGameWindowHookAsync(Process gameProcess, List<Process> gameProcesses);

        void InvokeLastWindowPosition();

        GameWindowPosition GetLastWindowPosition();

        /// <summary>
        /// Some games may change their handle when the window is switched full screen
        /// </summary>
        /// <returns></returns>
        void ResetWindowHandler();
    }
}