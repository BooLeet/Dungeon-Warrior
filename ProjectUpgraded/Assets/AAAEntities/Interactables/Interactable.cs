using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {
    public Transform interactPoint;
    public bool interactionEnabled = true;
    public Vector3 ButtonPosition { get { return interactPoint.position; } }

    void Awake()
    {
        if (interactPoint == null)
            interactPoint = transform;
        InteractableRegistry.GetInstance().Register(this);
    }

    public void Interact(Character interactingCharacter)
    {
        GameMode.gameModeInstance.OnInteraction(this);
        _Interact(interactingCharacter);
    }

    protected abstract void _Interact(Character interactingCharacter);

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
