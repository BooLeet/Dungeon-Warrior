using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public LanguagePack languagePack;

    void Awake()
    {
        if (instance)
            Destroy(gameObject);
        instance = this;
    }

	void Start () {
		
	}
	

	void Update () {
		
	}
}
