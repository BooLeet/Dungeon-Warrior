using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObjectPiece : MonoBehaviour {
    private MeshCollider meshCollider;
    private Rigidbody rb;
    public void Destruct(float explosionForce,Vector3 explosionPosition,float explosionRadius,float collisionActiveTime)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter)
            return;

        transform.parent = null;
        DestructableObjectPieceRegistry.GetInstance().Register(this);
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.mesh;
        meshCollider.convex = true;

        rb =  gameObject.AddComponent<Rigidbody>();
        rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
        StartCoroutine(DisableCollisions(collisionActiveTime));
    }

    public void DestructSticky()
    {
        transform.parent = null;
        DestructableObjectPieceRegistry.GetInstance().Register(this);
    }

    private IEnumerator DisableCollisions(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(rb);
        Destroy(meshCollider);
    }
}
