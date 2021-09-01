using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigManager : Singleton<ConfigManager>
{
    private Dictionary<string, PlayerConfigData> PlayerConfigDataDict;

    public override void Init()
    {
        base.Init();

        PlayerConfigDataDict = new Dictionary<string, PlayerConfigData>();

        //LoadInitConfig();
    }

    public override void Release()
    {
        PlayerConfigDataDict.Clear();

        base.Release();
    }

    public void LoadInitConfig()
    {
        LoadPlayerConfig();
    }

    private void LoadPlayerConfig()
    {
        DownloadManager.Instance.ResourceLoadText("player.txt", (data) =>
        {
            StringReader sr = new StringReader(data);
            string temp = "";
            while ((temp = sr.ReadLine()) != null)
            {
                string[] strs = temp.Split(',');
                if (strs.Length > 0)
                {
                    PlayerConfigData playerConfigData = new PlayerConfigData();
                    playerConfigData.ID = strs[0];
                    playerConfigData.Name = strs[1];
                    playerConfigData.Icon = strs[2];
                    playerConfigData.BasePropertyID = strs[3];
                    playerConfigData.BaseStateID = strs[4];
                    playerConfigData.BaseBuffID = strs[5];

                    if (PlayerConfigDataDict.ContainsKey(playerConfigData.ID))
                    {
                        MDebug.LogError("Player Config Contains Repeated ID");
                        continue;
                    }

                    PlayerConfigDataDict.Add(playerConfigData.ID, playerConfigData);
                }
            }
        }, LoadConfigError, false);
    }

    private void LoadConfigError()
    {

    }
}


public class PlayerConfigData
{
    public string ID;
    public string Name;
    public string Icon;
    public string BasePropertyID;
    public string BaseStateID;
    public string BaseBuffID;
}