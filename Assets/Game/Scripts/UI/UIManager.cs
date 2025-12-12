using InterDigital;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIState
{
    ArcadeSelector
}

[Serializable]
public class UIClass
{
    public UIState state;
    public BaseUI baseUI;
}

public class UIManager : MonoBehaviour
{
    public GameObject objBG;
    public List<UIClass> uiList;
    [HideInInspector] public UIClass currUIClass;

    GameManager gameManager;

    public void Init(GameManager inGameManager)
    {
        gameManager = inGameManager;
        currUIClass = null;

        int count = uiList.Count;
        for (int i = 0; i < count; i++)
        {
            uiList[i].baseUI.Init(inGameManager);
        }

        ShowUI(UIState.ArcadeSelector);
    }

    public void DoUpdate(float dt)
    {

    }

    public void ShowUI(UIState state)
    {
        if (currUIClass != null)
        {
            currUIClass.baseUI.Hide();
            currUIClass = null;
        }

        foreach (UIClass uiClass in uiList)
        {
            if (uiClass.state == state)
            {
                currUIClass = uiClass;
                break;
            }
        }

        currUIClass.baseUI.Show();
    }
}
