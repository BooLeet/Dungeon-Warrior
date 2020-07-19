using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeFromTexture : MonoBehaviour {
    public LandscapeGenerator generator;
    public Texture2D objectMap;
    public Texture2D heightMap;
    [Space]
    public LandscapeSet set;


    public void Generate()
    {
        generator.Generate(GenerateNodes());
    }

    public void Clear()
    {
        generator.Clear();
    }
	
    public LandscapeGenerator.Node[,] GenerateNodes()
    {
        if (objectMap.width != heightMap.width || objectMap.height != heightMap.height)
        {
            Debug.LogError("Map dimentions should be the same");
            return null;
        }

        LandscapeGenerator.Node[,] result = new LandscapeGenerator.Node[objectMap.height, objectMap.width];
        for (int i = 0; i < objectMap.height; ++i)
        {
            for (int j = 0; j < objectMap.width; ++j)
            {
                result[i, j] = new LandscapeGenerator.Node(GetPrefab(objectMap.GetPixel(i, j)), heightMap.GetPixel(i, j).r);
            }
        }

        return result;
    }

    private GameObject GetPrefab(Color color)
    {
        foreach (LandscapeSet.ColorToPrefab tuple in set.colorsToPrefabs)
        {
            if (tuple.color == color && tuple.prefabs.Length > 0)
                return tuple.prefabs[Random.Range(0, tuple.prefabs.Length)];
        }
        return null;
    }
}
