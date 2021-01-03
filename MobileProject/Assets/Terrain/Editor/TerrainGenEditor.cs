using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGeneration))]
public class TerrainGenEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainGeneration terrain = (TerrainGeneration)target;
        if (GUILayout.Button("Generate"))
        {
            terrain.ClearTerrain();
            terrain.GenerateTerrain();

            // Sets Terrain Chunks to be navigation static
            for (int i = 0; i < terrain.transform.childCount; ++i)
            {
                GameObjectUtility.SetStaticEditorFlags(terrain.transform.GetChild(i).gameObject, StaticEditorFlags.NavigationStatic | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic);
            }
        }
        if (GUILayout.Button("Clear"))
        {
            terrain.ClearTerrain();
        }

    }
}


