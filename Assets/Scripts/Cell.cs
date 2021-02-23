using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public static GameObject prefab;
    public GameObject spawnedCell;
    public int highestBranch;
    public Cell(Vector3 position, int cellNumber)
    {
        spawnedCell = GameObject.Instantiate(prefab, position, Quaternion.identity);
        highestBranch = cellNumber;
    }

    public GameObject[] Edges
    {
        get
        {
            GameObject[] edges = new GameObject[spawnedCell.transform.childCount];
            for (int i = 0; i < spawnedCell.transform.childCount; i++)
            {
                edges[i] = spawnedCell.transform.GetChild(i).gameObject;
            }

            return edges;
        }
    }

    public void RemoveEdge(int edgeNumber)
    {
        int childIndex = edgeNumber - 1;
        GameObject.Destroy(Edges[childIndex]);
    }

    public GameObject GetEdge(int edgeNumber)
    {
        int index = edgeNumber - 1;
        return Edges[index];
    }
}
