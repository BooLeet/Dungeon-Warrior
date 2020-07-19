using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Landscape/Set")]
public class LandscapeSet : ScriptableObject {
    [System.Serializable]
    public struct ColorToPrefab
    {
        public Color color;
        public GameObject[] prefabs;
    }

    public ColorToPrefab[] colorsToPrefabs;
}
