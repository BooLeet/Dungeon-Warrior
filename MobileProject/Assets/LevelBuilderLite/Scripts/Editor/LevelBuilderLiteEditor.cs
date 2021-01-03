using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelBuilderLite))]
public class LevelBuilderLiteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelBuilderLite levelBuilder = (LevelBuilderLite)target;
        if (GUILayout.Button("Build"))
        {
            levelBuilder.Clear();
            levelBuilder.Build();
            EditorUtility.SetDirty(levelBuilder.gameObject);
        }

        if (GUILayout.Button("Clear"))
        {
            levelBuilder.Clear();
            EditorUtility.SetDirty(levelBuilder.gameObject);
        }
    }
}
