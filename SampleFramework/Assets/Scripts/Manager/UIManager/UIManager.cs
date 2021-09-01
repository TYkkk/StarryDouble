﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public UIRoot UIRoot => uiRoot;

    private UIRoot uiRoot;

    private const string uiRootPath = "UI/UIRoot";

    private UIRoot uiRootTemplate;

    private readonly Dictionary<string, UIData> uiDataDict = new Dictionary<string, UIData>()
    {
    };

    private static Dictionary<string, List<UIBase>> loadedUIDict;

    public override void Init()
    {
        base.Init();

        loadedUIDict = new Dictionary<string, List<UIBase>>();

        InitUIRoot();
    }

    public override void Release()
    {
        base.Release();

        loadedUIDict = null;

        ClearUIRoot();
    }

    private void InitUIRoot()
    {
        if (uiRootTemplate == null)
        {
            uiRootTemplate = Resources.Load<UIRoot>(uiRootPath);
        }

        if (uiRootTemplate == null)
        {
            MDebug.LogError("uiRootTemplate Load Error");
            return;
        }

        ClearUIRoot();

        GameObject UIRootObj = Object.Instantiate(uiRootTemplate.gameObject);
        uiRoot = UIRootObj.GetComponent<UIRoot>();
    }

    private void ClearUIRoot()
    {
        if (uiRoot != null)
        {
            Object.Destroy(uiRoot.gameObject);
        }
    }

    public UIBase OpenUI(string uiName)
    {
        UIBase uiBase = GetOrCreateUI(uiName);

        uiBase.Register();

        uiBase.Opening();

        uiBase.transform.SetParent(UIRoot.LayerRoot.GetChild((int)uiDataDict[uiName].Layer), false);

        uiBase.Open();

        return uiBase;
    }

    private UIBase GetOrCreateUI(string uiName)
    {
        if (loadedUIDict.ContainsKey(uiName))
        {
            if (loadedUIDict[uiName] != null && loadedUIDict[uiName].Count > 0)
            {
                return loadedUIDict[uiName][0];
            }
        }

        GameObject createUI = Object.Instantiate(Resources.Load<GameObject>(uiDataDict[uiName].UIPath));

        UIBase uiBase = createUI.GetComponent<UIBase>();

        uiBase.SetName(uiName);

        if (!loadedUIDict.ContainsKey(uiName))
        {
            loadedUIDict.Add(uiName, new List<UIBase>());
        }

        loadedUIDict[uiName].Add(uiBase);

        return uiBase;
    }

    public void CloseUI(string uiName)
    {
        if (!loadedUIDict.ContainsKey(uiName))
        {
            return;
        }

        for (int i = 0; i < loadedUIDict[uiName].Count; i++)
        {
            loadedUIDict[uiName][i].UnRegister();

            loadedUIDict[uiName][i].Close();

            Object.Destroy(loadedUIDict[uiName][i].gameObject);
        }

        loadedUIDict.Remove(uiName);
    }

    public void CloseUI(UIBase uiBase)
    {
        uiBase.UnRegister();

        uiBase.Close();

        loadedUIDict[uiBase.UIName].Remove(uiBase);

        Object.Destroy(uiBase.gameObject);
    }
}


public class UIData
{
    public string UIName;
    public string UIPath;
    public UILayer Layer;

    public UIData(string UIName, string UIPath, UILayer Layer)
    {
        this.UIName = UIName;
        this.UIPath = UIPath;
        this.Layer = Layer;
    }
}

public enum UILayer
{
    Bottom,
    Layer01,
    Layer02,
    Layer03,
    Pop,
    Top,
    System,
}