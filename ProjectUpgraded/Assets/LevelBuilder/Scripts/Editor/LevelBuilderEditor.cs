using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelBuilder))]
public class LevelBuilderEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelBuilder levelBuilder = (LevelBuilder)target;
        if (GUILayout.Button("Build"))
        {
            levelBuilder.Clear();
            levelBuilder.Build();
        }

        if (GUILayout.Button("Clear"))
        {
            levelBuilder.Clear();
        }
    }
}
