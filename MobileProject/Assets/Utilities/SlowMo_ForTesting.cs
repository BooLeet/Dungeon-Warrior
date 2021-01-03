using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMo_ForTesting : MonoBehaviour
{
    public float timeScale = 0.2f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
    }
}
