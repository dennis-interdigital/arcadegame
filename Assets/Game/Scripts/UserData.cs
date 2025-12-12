namespace InterDigital
{
    public class UserData
    {
        public long lastPlayTime;
        public int coin;

        public void Init(bool firstTimePlay = false)
        {
            if (firstTimePlay)
            {
                lastPlayTime = 0;
                coin = 0;
            }
        }
    }
}

