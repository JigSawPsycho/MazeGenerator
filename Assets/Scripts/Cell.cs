using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public static GameObject prefab;
    public GameObject spawnedCell;
    public int highestBranch;
    
    //I decided to use a constructor here to easily assign the cell it's number and spawn in the corresponding prefab in one go.
    public Cell(Vector3 position, int cellNumber)
    {
        spawnedCell = GameObject.Instantiate(prefab, position, Quaternion.identity);
        highestBranch = cellNumber;
    }

    public GameObject[] Edges
    {
        get
        {
            //Because the number of active edges can change every frame, I made the decision not to cache an edges array. 
            //And instead get the edges whenever it is called.
            GameObject[] edges = new GameObject[spawnedCell.transform.childCount];
            for (int i = 0; i < spawnedCell.transform.childCount; i++)
            {
                edges[i] = spawnedCell.transform.GetChild(i).gameObject;
            }

            return edges;
        }
    }

    //Destroys one of the walls of the cell.
    public void RemoveEdge(int edgeNumber)
    {
        int childIndex = edgeNumber - 1;
        GameObject.Destroy(Edges[childIndex]);
    }

    //Returns one of the walls of the cell
    public GameObject GetEdge(int edgeNumber)
    {
        int index = edgeNumber - 1;
        return Edges[index];
    }
}
