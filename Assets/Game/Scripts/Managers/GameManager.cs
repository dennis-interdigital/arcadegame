using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InterDigital
{
    public class GameManager : MonoBehaviour
    {
        public Main main;
        public UserData userData;
        public GameSupport gameSupport;

        public CurrencyManager currencyManager;
        public InventoryManager inventoryManager;

        public UIManager uiManager;

        public string currGameSceneName;

        const float SCENE_LOAD_MAX_PROGRESS = 0.9f;

        public void Init(Main inMain)
        {
            main = inMain;
            userData = main.userData;
            gameSupport = main.gameSupport;

            //READ ME: manager constructor here
            inventoryManager = new InventoryManager();
            currencyManager = new CurrencyManager();
        }

        public void InitManagers(long serverTime = 0)
        {
            //READ ME: manager init here
            currencyManager.Init(this);
            inventoryManager.Init(this);

            uiManager.Init(this);
        }

        public void DoUpdate(float dt)
        {
            uiManager.DoUpdate(dt);
        }

        public void LoadGameScene(string sceneName)
        {
            StartCoroutine(LoadingSceneAsync(sceneName));
        }

        IEnumerator LoadingSceneAsync(string sceneName)
        {
            currGameSceneName = sceneName;

            ArcadeSelectorUI arcadeSelectorUI = uiManager.currUIClass.baseUI as ArcadeSelectorUI;

            yield return arcadeSelectorUI.HideTransition();

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / SCENE_LOAD_MAX_PROGRESS);

                if (progress >= SCENE_LOAD_MAX_PROGRESS)
                {
                    operation.allowSceneActivation = true;
                }
                yield return null;
            }

            uiManager.objBG.SetActive(false);
        }

        public void UnloadGameScene()
        {
            StartCoroutine(UnloadingSceneAsync());
        }

        IEnumerator UnloadingSceneAsync()
        {
            AsyncOperation operation = SceneManager.UnloadSceneAsync(currGameSceneName);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / SCENE_LOAD_MAX_PROGRESS);

                if(progress >= SCENE_LOAD_MAX_PROGRESS)
                {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            ArcadeSelectorUI arcadeSelectorUI = uiManager.currUIClass.baseUI as ArcadeSelectorUI;

            uiManager.objBG.SetActive(true);
            yield return arcadeSelectorUI.ShowTransition();
        }
    }
}
