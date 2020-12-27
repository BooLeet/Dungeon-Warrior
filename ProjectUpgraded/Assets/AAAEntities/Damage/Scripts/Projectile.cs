using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    private Character sender;
    private ProjectileInfo info;
    private float damage;

    private float currentLifeTime;
    private uint currentPenetrations = 0;

    public static Projectile Create(float damage, ProjectileInfo info, Vector3 position, Quaternion rotation, Character sender = null)
    {
        GameObject obj = new GameObject(info.name);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        if(info.model)
            Instantiate(info.model, obj.transform);

        Projectile projectile = obj.AddComponent<Projectile>();
        projectile.sender = sender;
        projectile.damage = damage;
        projectile.info = info;

        return projectile;
    }

	void LateUpdate () {
        if (currentLifeTime >= info.lifeTime)
            Destroy(gameObject);

        RaycastHit hitInfo;
        float raycastDistance = info.speed * Time.deltaTime;
        currentLifeTime += Time.deltaTime;

        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo, raycastDistance))
        //if (Physics.SphereCast(transform.position, info.radius, transform.forward, out hitInfo, raycastDistance))
        {
            if(info.hitEffect)
                Instantiate(info.hitEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

            Entity entity = hitInfo.collider.GetComponent<Entity>();
            if(entity != sender)
            {
                if (!entity)
                    ProjectileDestruction();
                else
                {
                    currentPenetrations++;
                    Damage.SendDamageFeedback(sender, entity, entity.TakeDamage(damage, sender, transform.position));

                    if (currentPenetrations >= info.maxPenetrations)
                        ProjectileDestruction();
                }    
            }
        }

        transform.Translate(Vector3.forward * info.speed * Time.deltaTime);
    }

    private void ProjectileDestruction()
    {
        info.onHit.Execute(sender, transform.position);
        Destroy(gameObject);
    }
}

[System.Serializable]
public class ProjectileInfo
{
    public string name = "Projectile";
    [Space]
    [Range(0.05f,5)]
    public float radius = 0.05f;
    public float speed = 80;
    public float lifeTime = 1;
    public uint maxPenetrations = 0;
    public OnHitList onHit;
    public GameObject model;
    public GameObject hitEffect;
}

