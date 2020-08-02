using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour {
    public DestructableObjectPiece[] pieces;
    public void Destruct(float explosionForce, Vector3 explosionPosition, float explosionRadius, bool sticky)
    {
        foreach (DestructableObjectPiece piece in pieces)
        {
            if (sticky)
                piece.DestructSticky();
            else
                piece.Destruct(explosionForce, explosionPosition, explosionRadius, 5);
        }
            
    }


    public void SetUpChildDestructables()
    {
        pieces = GetComponentsInChildren<DestructableObjectPiece>();
    }

    public void ClearChildDestructables()
    {
        pieces = null;
    }
}
