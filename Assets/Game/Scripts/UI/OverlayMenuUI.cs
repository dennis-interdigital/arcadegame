using InterDigital;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverlayMenuUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textCoin;
    [SerializeField] TextMeshProUGUI textDiamond;

    GameManager gameManager;
    UserData userData;

    public void Init(GameManager inGameManager)
    {
        gameManager = inGameManager;
        userData = gameManager.userData;

        RefreshUI();
    }

    public void RefreshUI()
    {
        int coin = userData.coin;
        int diamond = userData.diamond;

        textCoin.SetText(coin.ToString());
        textDiamond.SetText(diamond.ToString());
    }
}
