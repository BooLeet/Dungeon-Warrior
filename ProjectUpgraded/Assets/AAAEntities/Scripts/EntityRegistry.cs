using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityRegistry : MonoBehaviour {
    private static EntityRegistry instance;
    public List<Entity> entities = new List<Entity>();

    public static EntityRegistry GetInstance()
    {
        if (instance)
            return instance;
        instance = new GameObject("EntityRegistry").AddComponent<EntityRegistry>();
        return instance;
    }

    public void Register(Entity entity)
    {
        entities.Add(entity);
    }

    public void Unregister(Entity entity)
    {
        entities.Remove(entity);
    }

    public IEnumerable<Entity> GetClosestEntities(Vector3 position, float distance, Entity ignoredEntity = null)
    {
        return from entity in entities
               where Vector3.Distance(position, entity.Position) <= distance && entity != ignoredEntity
               select entity;
    }
}
