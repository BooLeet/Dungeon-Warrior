using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObjectPieceRegistry : MonoBehaviour
{
    private static DestructableObjectPieceRegistry instance;
    public List<DestructableObjectPiece> pieces = new List<DestructableObjectPiece>();

    void Awake()
    {
        if (instance)
        {
            if (instance != this)
                Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void Register(DestructableObjectPiece piece)
    {
        pieces.Add(piece);
    }

    public void Unregister(DestructableObjectPiece piece)
    {
        pieces.Remove(piece);
    }

    public static DestructableObjectPieceRegistry GetInstance()
    {
        if (!instance)
            instance = new GameObject("DestructableObjectPieceRegistry").AddComponent<DestructableObjectPieceRegistry>();
        return instance;
    }

    public void Clear()
    {
        int count = pieces.Count;
        for (int i = 0; i < count; ++i)
        {
            Destroy(pieces[0].gameObject);
            pieces.RemoveAt(0);
        }
    }
}
