using UnityEngine;

namespace InterDigital
{
    public enum LogType
    {
        Log,
        Warning,
        Error
    }

    public class PluginManager : MonoBehaviour
    {
        public CoroutineCache coroutineCache;

        public void Init(Main inMain)
        {
            GameObject coroutineCacheObj = new GameObject(typeof(CoroutineCache) + "");
            coroutineCache = coroutineCacheObj.AddComponent<CoroutineCache>();
            coroutineCache.Init();
            DontDestroyOnLoad(coroutineCache);
        }
    }

    public static class InterDigital
    {
        public static void Log(LogType type, string message)
        {
            Color logColor = Color.cyan;
            string hexColor = ColorUtility.ToHtmlStringRGB(logColor);

            string log = $"<color=#{hexColor}>[InterDigital]:</color> {message}";

            switch (type)
            {
                case LogType.Log: Debug.Log(log); break;
                case LogType.Warning: Debug.LogWarning(log); break;
                case LogType.Error: Debug.LogError(log); break;
            }
        }
    }
}

