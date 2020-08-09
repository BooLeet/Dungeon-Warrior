using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeadlyObjectRegistry : MonoBehaviour {
    public static DeadlyObjectRegistry instance;
    private List<DeadlyObject> deadlyObjects = new List<DeadlyObject>();
    public static DeadlyObjectRegistry GetInstance()
    {
        if (instance)
            return instance;
        instance = new GameObject("DeadlyObjectRegistry").AddComponent<DeadlyObjectRegistry>();
        return instance;
    }

    public void Register(DeadlyObject obj)
    {
        deadlyObjects.Add(obj);
    }

    public void Unregister(DeadlyObject obj)
    {
        deadlyObjects.Remove(obj);
    }

    private void ClearDestroyedObjects()
    {
        for (int i = 0; i < deadlyObjects.Count; ++i)
            if (deadlyObjects[i] == null)
            {
                deadlyObjects[i] = deadlyObjects[deadlyObjects.Count - 1];
                deadlyObjects.RemoveAt(deadlyObjects.Count - 1);
                --i;
            }
    }

    public IEnumerable<DeadlyObject> GetClosestObjects(Vector3 position, float distance)
    {
        ClearDestroyedObjects();

        return from obj in deadlyObjects
               where Vector3.Distance(position, obj.transform.position) <= distance
               select obj;
    }
}
