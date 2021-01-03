using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneNameContainer : MonoBehaviour {
    public static SceneNameContainer instance;
    public string sceneName;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
