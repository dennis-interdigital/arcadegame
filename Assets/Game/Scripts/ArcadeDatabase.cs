using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ArcadeDatabase", menuName = "Game/Arcade Database", order = 1)]
public class ArcadeDatabase : ScriptableObject
{
    [System.Serializable]
    public class ArcadeEntry
    {
        [Header("Basic Info")]
        public string arcadeName;
        public string arcadeId;

        [Header("Visual")]
        public Sprite icon;

#if UNITY_EDITOR
        [Header("Scene Reference (Editor Only)")]
        public SceneAsset sceneAsset;
#endif
        [HideInInspector]
        public string sceneName; // Stored for runtime loading
    }

    [Header("List of All Arcades")]
    public List<ArcadeEntry> arcades = new List<ArcadeEntry>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Sync all sceneAsset -> sceneName
        foreach (var entry in arcades)
        {
            if (entry != null && entry.sceneAsset != null)
            {
                string path = AssetDatabase.GetAssetPath(entry.sceneAsset);
                entry.sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            }
        }
    }
#endif
}
