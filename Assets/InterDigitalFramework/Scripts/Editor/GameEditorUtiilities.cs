using System.IO;
using UnityEditor;
using UnityEngine;

public class GameEditorUtiilities
{
    [MenuItem("InterDigital/ClearUserData")]
    public static void ClearUserData()
    {
        PlayerPrefs.DeleteAll();
        string persistentDataPath = Application.persistentDataPath;
        if (Directory.Exists(persistentDataPath))
        {
            Directory.Delete(persistentDataPath, true);
        }
        Debug.Log("ESI/ClearUserData: All PlayerPrefs and local data are deleted!");
    }
}
