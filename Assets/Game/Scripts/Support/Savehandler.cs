using UnityEngine;

namespace InterDigital
{
    public class Savehandler
    {
        Main main;

        public void Init(Main inMain)
        {
            main = inMain;
        }

        public void ResetAllData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        public void SetInteger(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        public int GetInteger(string key)
        {
            int result = 0;
            if (HasKey(key))
            {
                result = PlayerPrefs.GetInt(key);
            }

            return result;
        }

        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        public string GetString(string key)
        {
            string result = "";
            if (HasKey(key))
            {
                result = PlayerPrefs.GetString(key);
            }

            return result;
        }

        public void SetData<T>(string key, T data)
        {
            string jsonString = JsonUtility.ToJson(data);
            SetString(key, jsonString);
        }

        public T GetData<T>(string key)
        {
            T result;
            if (HasKey(key))
            {
                string str = PlayerPrefs.GetString(key);
#if UNITY_IOS
                result = JsonUtility.FromJson<T>(str);
#elif UNITY_ANDROID
                if (str == "{}")
                {
                    result = default(T);
                }
                else
                {
                    result = JsonUtility.FromJson<T>(str);
                }
#else
                result = default(T);
#endif
            }
            else
            {
                result = default(T);
            }

            return result;
        }

        public void Remove(string key)
        {
            if (HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();
            }
        }

        public bool HasKey(string key)
        {
            bool result = PlayerPrefs.HasKey(key);
            return result;
        }
    }
}
