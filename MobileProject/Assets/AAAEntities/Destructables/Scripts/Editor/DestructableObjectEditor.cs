using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DestructableObject))]
public class DestructableObjectEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DestructableObject obj = (DestructableObject)target;
        if (GUILayout.Button("Set Up"))
        {
            obj.SetUpChildDestructables();
        }

        if (GUILayout.Button("Clear"))
        {
            obj.ClearChildDestructables();
        }
    }
}
