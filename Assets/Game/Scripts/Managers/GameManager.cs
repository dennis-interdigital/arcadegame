using UnityEngine;

namespace InterDigital
{
    public class GameManager : MonoBehaviour
    {
        public Main main;
        public UserData userData;
        public GameSupport gameSupport;

        public UIManager uiManager;

        public void Init(Main inMain)
        {
            main = inMain;
            userData = main.userData;
            gameSupport = main.gameSupport;

            //READ ME: manager constructor here
        }

        public void InitManagers(long serverTime = 0)
        {
            //READ ME: manager init here

            uiManager.Init(this);
        }

        public void DoUpdate(float dt)
        {
            uiManager.DoUpdate(dt);
        }
    }
}
