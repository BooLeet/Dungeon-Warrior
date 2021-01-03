using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeGenerator : MonoBehaviour {
    public struct Node
    {
        public GameObject prefab;
        public float heightOffset;

        public Node (GameObject aPrefab, float aHeightOffset)
        {
            prefab = aPrefab;
            heightOffset = aHeightOffset;
        }
    }
    public float nodeSize = 5;
    public float heightMultiplier = 50;

    public void Generate(Node[,] nodes)
    {
        for (int i = 0; i < nodes.GetLength(0); ++i)
        {
            for (int j = 0; j < nodes.GetLength(1); ++j)
            {
                Instantiate(nodes[i, j], i, j);
            }
        }
    }

    public void Clear()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    private void Instantiate(Node node,int i,int j)
    {
        if (node.prefab)
        {
            GameObject obj = Instantiate(node.prefab, transform.position + new Vector3(i, 0, j) * nodeSize + Vector3.up * node.heightOffset * heightMultiplier, Quaternion.identity, transform);
            obj.transform.localScale = Vector3.one * nodeSize;
            LandscapeNodeFunction nodeFunction = obj.GetComponent<LandscapeNodeFunction>();
            if (nodeFunction)
                nodeFunction.Function(nodeSize);
        }
    }
}
