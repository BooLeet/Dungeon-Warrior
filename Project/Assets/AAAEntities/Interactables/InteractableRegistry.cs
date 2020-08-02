using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractableRegistry : MonoBehaviour {
    private static InteractableRegistry instance;
    public List<Interactable> interactables = new List<Interactable>();

    void Awake()
    {
        if(instance)
        {
            if (instance != this)
                Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public static InteractableRegistry GetInstance()
    {
        if (!instance)
            instance = new GameObject("InteractableRegistry").AddComponent<InteractableRegistry>();
        return instance;
    }

    public void Register(Interactable interactable)
    {
        interactables.Add(interactable);
    }

    public void Unregister(Interactable interactable)
    {
        interactables.Remove(interactable);
    }


    public IEnumerable<Interactable> GetClosestInteractables(Vector3 position, float distance)
    {
        return from interactable in interactables
               where Vector3.Distance(position, interactable.ButtonPosition) <= distance
               select interactable;
    }
}
