using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LandscapeFromTexture))]
public class LandscapeFromTextureEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LandscapeFromTexture landscapeFromTexture = (LandscapeFromTexture)target;
        if (GUILayout.Button("Generate"))
        {
            landscapeFromTexture.Clear();
            landscapeFromTexture.Generate();

            // Sets Terrain Chunks to be navigation static
            //for (int i = 0; i < terrain.transform.childCount; ++i)
            //{
            //    GameObjectUtility.SetStaticEditorFlags(terrain.transform.GetChild(i).gameObject, StaticEditorFlags.NavigationStatic);
            //}
        }
        if (GUILayout.Button("Clear"))
        {
            landscapeFromTexture.Clear();
        }

    }

}
