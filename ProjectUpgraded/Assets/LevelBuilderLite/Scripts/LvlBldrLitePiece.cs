using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelBuilderLite/Piece")]
public class LvlBldrLitePiece : ScriptableObject
{
    public Color mapColor = Color.white;
    public Material floorMaterial;
    public Material ceilingMaterial;
    public Material mainMaterial;
    public LevelBuilderLite.LayoutObject.PieceType type;

    [System.Serializable]
    public class Prop
    {
        public GameObject prefab;
        [Range(0, 1)]
        public float probability = 1f;
    }

    public Prop[] props;
}
