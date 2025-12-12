namespace InterDigital
{
    public class GameSupport
    {
        public Savehandler saveHandler;

        Main main;
        PluginManager pluginManager;

        public void Init(Main inMain)
        {
            main = inMain;
            pluginManager = main.pluginManager;

            saveHandler = new Savehandler();

            saveHandler.Init(main);
        }

        public void DoUpdate(float dt)
        {

        }
    }
}

