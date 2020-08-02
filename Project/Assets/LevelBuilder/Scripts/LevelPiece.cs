using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelBuilder/Piece")]
public class LevelPiece : ScriptableObject {
    public Color mapColor = Color.white;
    public GameObject[] prefabs;
    public GameObject[] optionalPrefabs;
    public LevelBuilder.LayoutObject.PieceType type;
}
