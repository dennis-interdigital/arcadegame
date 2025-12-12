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

        public UIManager uiManager;

        public string currGameSceneName;

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

        public void LoadGameScene(string sceneName)
        {
            StartCoroutine(LoadingSceneAsync(sceneName));
        }

        IEnumerator LoadingSceneAsync(string sceneName)
        {
            const float SCENE_LOAD_MAX_PROGRESS = 0.9f;

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
    }
}
