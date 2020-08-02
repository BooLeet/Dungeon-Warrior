using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeadlyObjectRegistry : MonoBehaviour {
    public static DeadlyObjectRegistry instance;
    public List<DeadlyObject> deadlyObjects = new List<DeadlyObject>();
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

    public IEnumerable<DeadlyObject> GetClosestObjects(Vector3 position, float distance)
    {
        return from obj in deadlyObjects
               where Vector3.Distance(position, obj.transform.position) <= distance
               select obj;
    }
}
