using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelBuilder : MonoBehaviour
{
    public struct LayoutObject
    {
        public enum PieceType { Simple, Wall, Door }
        public GameObject prefab;
        public GameObject optionalPrefab;
        public PieceType type;
    }

    public Texture2D map;
    public LevelPieceDB levelPieces;
    public float blockWidth = 5;

    public void Build()
    {
        Clear();
        BuildLevel(CreateLayoutFromTexture(map, levelPieces), blockWidth, transform.position);
    }

    public void Clear()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    LayoutObject[,] CreateLayoutFromTexture(Texture2D map, LevelPieceDB pieceDataBase)
    {
        LayoutObject[,] result = new LayoutObject[map.height, map.width];

        foreach (LevelPiece piece in pieceDataBase.pieces)
        {
            for (int i = 0; i < map.height; ++i)
                for (int j = 0; j < map.width; ++j)
                {
                    if (map.GetPixel(i, j) == piece.mapColor)
                    {
                        result[i, j].type = piece.type;
                        if (piece.prefabs.Length > 0)
                            result[i, j].prefab = piece.prefabs[Random.Range(0, piece.prefabs.Length)];
                        if (piece.optionalPrefabs != null && piece.optionalPrefabs.Length > 0)
                            result[i, j].optionalPrefab = piece.optionalPrefabs[Random.Range(0, piece.optionalPrefabs.Length)];
                    }
                }
        }

        return result;
    }

    void BuildLevel(LayoutObject[,] layout, float blockWidth, Vector3 position)
    {
        for (int i = 0; i < layout.GetLength(0); ++i)
            for (int j = 0; j < layout.GetLength(1); ++j)
                switch (layout[i, j].type)
                {
                    case LayoutObject.PieceType.Simple:
                        InstantiatePiece(layout[i, j].prefab, blockWidth, position + blockWidth * new Vector3(i, 0, j), Quaternion.identity);
                        break;
                    case LayoutObject.PieceType.Wall:
                        if (layout[i, j].optionalPrefab != null)
                            InstantiatePiece(layout[i, j].optionalPrefab, blockWidth, blockWidth * new Vector3(i, 0, j), Quaternion.identity);

                        if (i + 1 < layout.GetLength(0) && layout[i + 1, j].type != LayoutObject.PieceType.Wall)
                            InstantiatePiece(layout[i, j].prefab, blockWidth, position + blockWidth * new Vector3(i + 0.5f, 0, j), Quaternion.Euler(0, 90, 0));

                        if (i > 0 && layout[i - 1, j].type != LayoutObject.PieceType.Wall)
                            InstantiatePiece(layout[i, j].prefab, blockWidth, position + blockWidth * new Vector3(i - 0.5f, 0, j), Quaternion.Euler(0, -90, 0));

                        if (j + 1 < layout.GetLength(1) && layout[i, j + 1].type != LayoutObject.PieceType.Wall)
                            InstantiatePiece(layout[i, j].prefab, blockWidth, position + blockWidth * new Vector3(i, 0, j + 0.5f), Quaternion.Euler(0, 0, 0));

                        if (j > 0 && layout[i, j - 1].type != LayoutObject.PieceType.Wall)
                            InstantiatePiece(layout[i, j].prefab, blockWidth, position + blockWidth * new Vector3(i, 0, j - 0.5f), Quaternion.Euler(0, 180, 0));
                        break;
                    case LayoutObject.PieceType.Door:
                        InstantiatePiece(layout[i, j].prefab, blockWidth, position + blockWidth * new Vector3(i, 0, j), Quaternion.identity);

                        if (layout[i + 1, j].type == LayoutObject.PieceType.Simple || layout[i - 1, j].type == LayoutObject.PieceType.Simple)
                            InstantiatePiece(layout[i, j].optionalPrefab, blockWidth, position + blockWidth * new Vector3(i, 0, j), Quaternion.Euler(0, 90, 0));
                        else
                            InstantiatePiece(layout[i, j].optionalPrefab, blockWidth, position + blockWidth * new Vector3(i, 0, j), Quaternion.identity);
                        //if (layout[i + 1, j].type == LayoutObject.PieceType.Wall || layout[i - 1, j].type == LayoutObject.PieceType.Wall)
                        //    InstantiatePiece(layout[i, j].prefab, blockWidth, position + blockWidth * new Vector3(i, 0, j), Quaternion.identity);
                        //else
                        //    InstantiatePiece(layout[i, j].prefab, blockWidth, position + blockWidth * new Vector3(i, 0, j), Quaternion.Euler(0, 90, 0));
                        break;
                }
    }

    private void InstantiatePiece(GameObject prefab, float blockWidth, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
            return;

        GameObject piece;
        if (Application.isPlaying)
        {
            piece = Instantiate(prefab, position, rotation, transform);
        }
        else
        {
#if UNITY_EDITOR
            piece = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            piece.transform.position = position;
            piece.transform.rotation = rotation;
            piece.transform.parent = transform;
#endif
        }
        //LevelPieceProcedure procedure = piece.GetComponent<LevelPieceProcedure>();
        //if (procedure)
        //    procedure.Procedure();

        piece.transform.localScale = Vector3.one * blockWidth;
    }
}
