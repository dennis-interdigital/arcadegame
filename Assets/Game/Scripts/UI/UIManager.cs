using InterDigital;
using UnityEngine;

public enum UIState
{

}

public enum PopupState
{

}

public class UIManager : MonoBehaviour
{
    GameManager gameManager;

    public ArcadeSelector arcadeSelector;

    public void Init(GameManager inGameManager)
    {
        gameManager = inGameManager;
    }

    public void DoUpdate(float dt)
    {

    }
}
