using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DeadlyObject : MonoBehaviour {
    public float verticalTargetingOffset;
    public Vector3 Position { get { return transform.position + Vector3.up * verticalTargetingOffset; } }

    private void Awake()
    {
        DeadlyObjectRegistry.GetInstance().Register(this);
    }

    protected void RemoveObject()
    {
        DeadlyObjectRegistry.GetInstance().Unregister(this);
        Destroy(gameObject);
    }

}
