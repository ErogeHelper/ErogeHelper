﻿using ErogeHelper.Model.DataModel.Tables;

namespace ErogeHelper.Model.Repositories.Interface;

public interface IGameInfoRepository
{
    public GameInfoTable GameInfo { get; }

    public GameInfoTable? TryGetGameInfo();

    public void AddGameInfo(GameInfoTable gameInfoTable);

    public void UpdateGameInfo(GameInfoTable gameInfoTable);

    public void UpdateCloudStatus(bool useCloudSaveData);

    public void UpdateSaveDataPath(string path);

    public void UpdateLostFocusStatus(bool status);

    public void UpdateTouchEnable(bool status);

    public void UpdateTextractorSetting(string setting);
}
