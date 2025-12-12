using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ArcadeGameItem
{
    public string name;
    public string sceneName;
    public GameObject prefabGameIcon;
}

[CreateAssetMenu(fileName = "ArcadeGameSO", menuName = "SO/Arcade Database")]
public class ArcadeGameSO : ScriptableObject
{
    public List<ArcadeGameItem> arcadeGameItemList = new List<ArcadeGameItem>();
}
