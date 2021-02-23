using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] int mazeXSize;
    [SerializeField] int mazeZSize;
    [SerializeField] GameObject _cellPrefab;
    Dictionary<GameObject, Cell[]> neighbourCells = new Dictionary<GameObject, Cell[]>();
    List<GameObject> sharedEdges = new List<GameObject>();
    List<Cell> Cells;

    private void Start()
    {
        //Assign the applied prefab to the static cellPrefab field for Cells.
        Cell.prefab = _cellPrefab;

        //Generate a grid and remove overlapping walls.
        Cells = GenerateGrid();

        //Fill the neighbourCells dictionary
        PopulateSharedEdgeDictionary(Cells, neighbourCells);
        
        //Connect random cells until all cells can be reached from the entrance and there is one exit.
        StartCoroutine(ConnectRandomCells(neighbourCells));
    }

    List<Cell> GenerateGrid()
    {
        List<Cell> gridArr = new List<Cell>();
        int cellCount = 0;
        for (int x = 0; x < mazeXSize; x++)
        {
            for (int z = 0; z < mazeZSize; z++)
            {
                //Spawn in a cell
                Cell cell = new Cell(new Vector3(x * 10, 0, z * 10), cellCount);

                if (cellCount == 0)
                {
                    //Create maze entrance
                    cell.RemoveEdge(4);
                }

                if (x == mazeXSize - 1 && z == mazeZSize - 1)
                {
                    //Create maze exit
                    cell.RemoveEdge(2);

                }

                cellCount++;
                
                if (x > 0)
                {
                    //Remove the redundant edge for cells adjacent to other cells on the x axis
                    cell.RemoveEdge(4);
                }

                if (z > 0)
                {
                    //Remove the redundant edge for cells adjacent to other cells on the z axis
                    cell.RemoveEdge(1);
                }

                gridArr.Add(cell);
            }
        }

        return gridArr;
    }

    void PopulateSharedEdgeDictionary(List<Cell> cellList, Dictionary<GameObject, Cell[]> dictionaryToPopulate)
    {
        for (int i = 1; i <= cellList.Count; i++)
        {
            int currentCellIndex = i - 1;
            int rightCellIndex = i + mazeZSize - 1;
            int aboveCellIndex = i;
            if (i % mazeZSize != 0)
            {
                Cell[] cellPair = new Cell[2]
                {
                    cellList[currentCellIndex],
                    cellList[aboveCellIndex]
                };

                dictionaryToPopulate.Add(cellList[currentCellIndex].GetEdge(2), cellPair);
                sharedEdges.Add(cellList[currentCellIndex].GetEdge(2));
            }

            if (i < mazeZSize * mazeXSize - mazeZSize)
            {
                Cell[] cellPair = new Cell[2]
                {
                    cellList[currentCellIndex],
                    cellList[rightCellIndex]
                };

                dictionaryToPopulate.Add(cellList[currentCellIndex].GetEdge(3), cellPair);
                sharedEdges.Add(cellList[currentCellIndex].GetEdge(3));
            }
        }
    }

    IEnumerator ConnectRandomCells(Dictionary<GameObject, Cell[]> connectingEdges)
    {
        int uniqueids = Cells.Count;
        int alphaBranch = 0;
        while (connectingEdges.Count > 0)
        {
            //Used to visually represent the maze being opened up. I just think it looks nice.
            yield return new WaitForEndOfFrame();

            //Grab the index of a random edge connecting two cells.
            int index = UnityEngine.Random.Range(0, connectingEdges.Count);

            //Get the edge gameobject.
            GameObject edge = sharedEdges[index];

            //Get the two cells that are connected via the edge.
            Cell[] cellPair = connectingEdges[edge];

            //If there are still paths that can't be reached from the entrance but both of these cells are connected to the entrance,
            //remove them from the dictionary and continue on to the next connecting edge.
            if (cellPair[0].highestBranch == cellPair[1].highestBranch && uniqueids > 1)
            {
                connectingEdges.Remove(edge);
                sharedEdges.Remove(edge);
                continue;
            } else if (uniqueids == 1) //If all routes are connected, break.
            {
                break;
            }

            //Operate based off the lowest id of the two adjacent cells.
            if (cellPair[0].highestBranch < cellPair[1].highestBranch)
            {
                //Connect the cells by removing the connecting edge.
                edge.SetActive(false);
                connectingEdges.Remove(edge);
                sharedEdges.Remove(edge);
                
                //Set all cells with the same id as the higher id cell, to the lower id cell.
                //This ensures any cell connected to the higher id cell will now have the id of the lower cell.
                SetCellsFromidToid(cellPair[1].highestBranch, cellPair[0].highestBranch);
            } else
            {
                edge.SetActive(false);
                connectingEdges.Remove(edge);
                sharedEdges.Remove(edge);
                SetCellsFromidToid(cellPair[0].highestBranch, cellPair[1].highestBranch);
            }
            uniqueids--;
        }
    }

    void SetCellsFromidToid(int fromId, int toId)
    {
        for(int i = 0; i < Cells.Count; i++)
        {
            if (Cells[i].highestBranch == fromId)
                Cells[i].highestBranch = toId;
        }
    }
}
