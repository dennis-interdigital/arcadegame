using InterDigital;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    GameManager gameManager;

    public ArcadeSelectorUI arcadeSelectorUI;

    public void Init(GameManager inGameManager)
    {
        gameManager = inGameManager;

        arcadeSelectorUI.Init(gameManager);
    }

    public void DoUpdate(float dt)
    {

    }
}
