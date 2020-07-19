using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Damage {

    public static void MeleeDamage(Vector3 origin, Vector3 direction, float damage, float range, float angle, Entity damagingEntity)
    {
        var closestEntitiesWithinAngle = from entity in EntityRegistry.GetInstance().GetClosestEntities(origin, range, damagingEntity)
                                         where Utility.WithinAngle(origin, direction, entity.Position, angle) && Vector3.Distance(entity.Position, origin) <= range && Utility.IsVisible(origin,entity.gameObject,range,entity.verticalTargetingOffset)
                                         select new { entity, distance = Vector3.Distance(entity.Position, origin) };

        IEnumerable<Entity> entitiesToDamage = from entityInfo in closestEntitiesWithinAngle
                               where entityInfo.distance == closestEntitiesWithinAngle.Min(e => e.distance)
                               select entityInfo.entity;

        if (entitiesToDamage.Count() > 0)
        {
            entitiesToDamage.First().TakeDamage(damage, damagingEntity, damagingEntity.Position);
            SendDamageFeedback(damagingEntity, entitiesToDamage.First());
        }
            
    }


    public static void ExplosiveDamage(Vector3 origin, float damage, float range, Entity damagingEntity)//float explosionForce
    {
        var closestVisibleEntities = from entity in EntityRegistry.GetInstance().GetClosestEntities(origin, range)
                                     where Utility.IsVisible(origin, entity.gameObject, range,entity.verticalTargetingOffset)
                                     select entity;

        foreach (Entity e in closestVisibleEntities)
        {
            float distance = Vector3.Distance(e.Position, origin);
            float distanceMultiplier = (distance / range) > 0.5f ? (distance / range) : 1;

            e.TakeDamage(damage * distanceMultiplier, damagingEntity, origin);
            SendDamageFeedback(damagingEntity, e);
            //if (e as Character)
            //    (e as Character).AddExplosionForce(origin, explosionForce * distanceMultiplier);
        }

        //PlayerCamera.ScreenShake(0.2f, origin);
    }

    public static void HitscanDamage(Vector3 origin, Vector3 direction, float damage, float range, Entity damagingEntity)
    {
        Ray ray = new Ray(origin, direction);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, range))
        {
            Entity entity = hitInfo.collider.GetComponent<Entity>();
            if (entity)
                entity.TakeDamage(damage, damagingEntity, damagingEntity.Position);

        }
    }

    public static void SendDamageFeedback(Entity entity,Entity damagedEntity)
    {
        if (entity && entity as Character)
            (entity as Character).DamageFeedback(damagedEntity);
    }
}
