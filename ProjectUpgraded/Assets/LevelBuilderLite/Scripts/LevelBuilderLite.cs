using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilderLite : MonoBehaviour
{
    public struct LayoutObject
    {
        public enum PieceType { Simple, Wall, Door }
        public Material floorMaterial;
        public Material ceilingMaterial;
        public Material mainMaterial;
        public PieceType type;
        public GameObject prop;
    }

    public Texture2D map;
    public LvlBldrLitePieceDB levelPieces;
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

    LayoutObject[,] CreateLayoutFromTexture(Texture2D map, LvlBldrLitePieceDB pieceDataBase)
    {
        LayoutObject[,] result = new LayoutObject[map.height, map.width];

        foreach (LvlBldrLitePiece piece in pieceDataBase.pieces)
        {
            for (int i = 0; i < map.height; ++i)
                for (int j = 0; j < map.width; ++j)
                    if (map.GetPixel(i, j) == piece.mapColor)
                    {
                        result[i, j].type = piece.type;
                        result[i, j].floorMaterial = piece.floorMaterial;
                        result[i, j].ceilingMaterial = piece.ceilingMaterial;
                        result[i, j].mainMaterial = piece.mainMaterial;

                        for (int k = 0; k < piece.props.Length; ++k)
                            if(Random.value <= piece.props[k].probability)
                            {
                                result[i, j].prop = piece.props[k].prefab;
                                break;
                            }
                    }
        }

        return result;
    }

    void BuildLevel(LayoutObject[,] layout, float blockWidth, Vector3 position)
    {
        // Create one and two faced quads
        Mesh oneFacedQuad = CreateQuadOneFace(blockWidth);
        Mesh doorQuad = CreateQuadTwoFace(blockWidth, new Vector2(-0.5f, -0.5f));

        for (int i = 0; i < layout.GetLength(0); ++i)
            for (int j = 0; j < layout.GetLength(1); ++j)
            {
                InstantiatePiece(layout[i, j].floorMaterial, blockWidth, position + blockWidth * new Vector3(i, 0, j), Quaternion.Euler(90, 0, 0), oneFacedQuad);
                InstantiatePiece(layout[i, j].ceilingMaterial, blockWidth, position + blockWidth * new Vector3(i, 1, j), Quaternion.Euler(-90, 0, 0), oneFacedQuad);

                if (layout[i, j].prop)
                    Instantiate(layout[i, j].prop, position + blockWidth * new Vector3(i, 0, j), Quaternion.identity, transform);

                switch (layout[i, j].type)
                {
                    case LayoutObject.PieceType.Simple:

                        break;
                    case LayoutObject.PieceType.Wall:
                        if (i + 1 < layout.GetLength(0) && layout[i + 1, j].type != LayoutObject.PieceType.Wall)
                            InstantiatePiece(layout[i, j].mainMaterial, blockWidth, position + blockWidth * new Vector3(i + 0.5f, 0.5f, j), Quaternion.Euler(0, 270, 0), oneFacedQuad);

                        if (i > 0 && layout[i - 1, j].type != LayoutObject.PieceType.Wall)
                            InstantiatePiece(layout[i, j].mainMaterial, blockWidth, position + blockWidth * new Vector3(i - 0.5f, 0.5f, j), Quaternion.Euler(0, 90, 0), oneFacedQuad);

                        if (j + 1 < layout.GetLength(1) && layout[i, j + 1].type != LayoutObject.PieceType.Wall)
                            InstantiatePiece(layout[i, j].mainMaterial, blockWidth, position + blockWidth * new Vector3(i, 0.5f, j + 0.5f), Quaternion.Euler(0, 180, 0), oneFacedQuad);

                        if (j > 0 && layout[i, j - 1].type != LayoutObject.PieceType.Wall)
                            InstantiatePiece(layout[i, j].mainMaterial, blockWidth, position + blockWidth * new Vector3(i, 0.5f, j - 0.5f), Quaternion.Euler(0, 0, 0), oneFacedQuad);
                        break;
                    case LayoutObject.PieceType.Door:
                        GameObject door;
                        if (layout[i + 1, j].type == LayoutObject.PieceType.Simple || layout[i - 1, j].type == LayoutObject.PieceType.Simple)
                            door = InstantiatePiece(layout[i, j].mainMaterial, blockWidth, position + blockWidth * new Vector3(i - 0.5f, 0, j + 0.5f), Quaternion.Euler(0, 90, 0), doorQuad);
                        else
                            door = InstantiatePiece(layout[i, j].mainMaterial, blockWidth, position + blockWidth * new Vector3(i - 0.5f, 0, j - 0.5f), Quaternion.identity, doorQuad);
                        

                        GameObject doorInteractPoint = new GameObject("InteractPoint");
                        doorInteractPoint.transform.parent = door.transform;
                        doorInteractPoint.transform.localPosition = new Vector3(blockWidth / 2, blockWidth / 2, 0);
                        doorInteractPoint.AddComponent<BoxCollider>().size = Vector3.one * 0.2f;

                        LvlBldrLiteDoor doorScript = doorInteractPoint.AddComponent<LvlBldrLiteDoor>();

                        doorScript.doorTransform = door.transform;

                        doorScript.interactPoint = doorInteractPoint.transform;
                        break;
                }
            } 
    }

    private Mesh CreateQuadOneFace(float size)
    {
        Mesh mesh = new Mesh();

        float halfSize = size / 2;
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-halfSize, -halfSize, 0),
            new Vector3(halfSize, -halfSize, 0),
            new Vector3(-halfSize, halfSize, 0),
            new Vector3(halfSize, halfSize, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
              new Vector2(0, 0),
              new Vector2(1, 0),
              new Vector2(0, 1),
              new Vector2(1, 1)
        };
        mesh.uv = uv;

        return mesh;
    }

    private Mesh CreateQuadTwoFace(float size, Vector2 relativeVertexOffset)
    {
        Mesh mesh = new Mesh();

        float halfSize = size / 2;
        float xOffset = relativeVertexOffset.x * size;
        float yOffset = relativeVertexOffset.y * size;
        Vector3[] vertices = new Vector3[8]
        {
            new Vector3(-halfSize - xOffset, -halfSize - yOffset, 0),
            new Vector3(halfSize - xOffset, -halfSize - yOffset, 0),
            new Vector3(-halfSize - xOffset, halfSize - yOffset, 0),
            new Vector3(halfSize - xOffset, halfSize - yOffset, 0),

            new Vector3(-halfSize - xOffset, -halfSize - yOffset, 0),
            new Vector3(halfSize - xOffset, -halfSize - yOffset, 0),
            new Vector3(-halfSize - xOffset, halfSize - yOffset, 0),
            new Vector3(halfSize - xOffset, halfSize - yOffset, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[12]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1,

            // lower left triangle
            5, 6, 4,
            // upper right triangle
            5, 7, 6
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[8]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,

            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[8]
        {
              new Vector2(0, 0),
              new Vector2(1, 0),
              new Vector2(0, 1),
              new Vector2(1, 1),

              new Vector2(0, 0),
              new Vector2(1, 0),
              new Vector2(0, 1),
              new Vector2(1, 1)
        };
        mesh.uv = uv;

        return mesh;
    }

    private GameObject InstantiatePiece(Material material, float blockWidth, Vector3 position, Quaternion rotation, Mesh mesh)
    {
        if (material == null)
            return null;

        GameObject obj = new GameObject();

        MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        meshRenderer.sharedMaterial = material;
        obj.AddComponent<MeshFilter>().mesh = mesh;
        obj.AddComponent<MeshCollider>().sharedMesh = mesh;

        obj.transform.parent = transform;
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return obj;
    }
}
