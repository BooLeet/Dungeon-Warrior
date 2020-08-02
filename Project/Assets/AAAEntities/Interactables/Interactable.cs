using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {
    public Transform interactPoint;
    public Vector3 ButtonPosition { get { return interactPoint.position; } }

    void Awake()
    {
        if (interactPoint == null)
            interactPoint = transform;
        InteractableRegistry.GetInstance().Register(this);
    }

    public abstract void Interact(Character interactingCharacter);

    public abstract string GetPrompt(Character interactingCharacter);

    public void Unregister()
    {
        InteractableRegistry.GetInstance().Unregister(this);
    }

    protected void RemoveInteractable()
    {
        Unregister();
        Destroy(gameObject);
    }

}
