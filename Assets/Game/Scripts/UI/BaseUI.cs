using InterDigital;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    protected GameManager gameManager;
    protected UIManager uiManager;

    public virtual void Init(GameManager inGameManager)
    {
        gameManager = inGameManager;
        uiManager = inGameManager.uiManager;
    }

    public virtual void DoUpdate(float dt)
    {

    }
}
