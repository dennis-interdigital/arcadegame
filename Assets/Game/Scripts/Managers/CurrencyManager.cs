using InterDigital;

public class CurrencyManager
{
    GameManager gameManager;
    UserData userData;

    public void Init(GameManager inGameManager)
    {
        gameManager = inGameManager;
        userData = gameManager.userData;
    }

    public bool IsCoinSufficient(int cost)
    {
        int coin = userData.coin;
        bool result = coin >= cost;

        return result;
    }

    public void SpendCoin(int cost)
    {
        userData.coin -= cost;
    }

    public void AddCoin(int amount)
    {
        userData.coin += amount;
    }

    public bool IsDiamondSufficient(int cost)
    {
        int diamond = userData.diamond;
        bool result = diamond >= cost;

        return result;
    }

    public void SpendDiamond(int amount)
    {
        userData.diamond -= amount;
    }

    public void AddDiamond(int amount)
    {
        userData.diamond += amount;
    }
}
