using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelRemover : MonoBehaviour
{
    // Removes model on Start
    void Start()
    {
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());
    }
}
