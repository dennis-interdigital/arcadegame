using UnityEngine;

namespace InterDigital
{
    public class Main : MonoBehaviour
    {
        public UserData userData;

        public PluginManager pluginManager;
        public GameManager gameManager;
        public GameSupport gameSupport;

        Savehandler saveHandler;

        bool ready;

        void Awake()
        {
            //Set FPS, multitouch, screen resolution, etc here
            gameObject.name = "Main";
            Application.targetFrameRate = 60;

            Input.multiTouchEnabled = false;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            pluginManager.Init(this);
            gameSupport = new GameSupport();
            gameSupport.Init(this);

            saveHandler = gameSupport.saveHandler;

            Load();

            gameManager.Init(this);

            gameManager.InitManagers();
            ready = true;
        }

        void FixedUpdate()
        {
            if (ready)
            {
                float dt = Time.deltaTime;
                gameSupport.DoUpdate(dt);
                gameManager.DoUpdate(dt);
            }
        }

        public void Load()
        {
            bool firstTimePlaying = false;

            bool userDataPrefExist = saveHandler.HasKey(Parameter.PlayerPrefKey.STRING_USERDATA);
            if (userDataPrefExist)
            {
                InterDigital.Log(LogType.Log, "UserDataPref Exist. Load data from PlayerPrefs");
                userData = saveHandler.GetData<UserData>(Parameter.PlayerPrefKey.STRING_USERDATA);
            }
            else
            {
                InterDigital.Log(LogType.Log, "New UserData");
                userData = new UserData();
                firstTimePlaying = true;
            }

            userData.Init(firstTimePlaying);

            string jsonUserData = JsonUtility.ToJson(userData);
            InterDigital.Log(LogType.Log, jsonUserData);
        }

        public void Save()
        {
            saveHandler.SetData(Parameter.PlayerPrefKey.STRING_USERDATA, userData);
            InterDigital.Log(LogType.Log, "SAVE");
        }

        void OnApplicationQuit()
        {
            Save();
        }
    }
}

