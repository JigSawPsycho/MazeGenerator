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
        Cell.prefab = _cellPrefab;

        Cells = GenerateGrid();

        PopulateSharedEdgeDictionary(Cells, neighbourCells);

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
                //Debug to see if my math is right and thinking path lol.
                //cellList[i - 1].spawnedCell.transform.position += Vector3.up * 5;
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
                //Debug to see if my math is right and thinking path lol.
                //cellList[i - 1].spawnedCell.transform.position += Vector3.up * 5;

                Cell[] cellPair = new Cell[2]
                {
                    cellList[currentCellIndex],
                    cellList[rightCellIndex]
                };

                dictionaryToPopulate.Add(cellList[currentCellIndex].GetEdge(3), cellPair);
                sharedEdges.Add(cellList[currentCellIndex].GetEdge(3));
            }
        }

        //Debugging again to check if the edges were getting shared and indexed properly. They are at the moment.
        /*
        for(int i = 0; i < dictionaryToPopulate[sharedEdges[0]].Length; i++)
        {
            dictionaryToPopulate[sharedEdges[0]][i].spawnedCell.transform.position += Vector3.down * 5;
            sharedEdges[0].transform.position += Vector3.up * 10;
        }
        */
    }

    IEnumerator ConnectRandomCells(Dictionary<GameObject, Cell[]> connectingEdges)
    {
        int uniqueids = Cells.Count;
        int alphaBranch = 0;
        while (connectingEdges.Count > 0)
        {
            yield return new WaitForEndOfFrame();

            int index = UnityEngine.Random.Range(0, connectingEdges.Count);

            GameObject edge = sharedEdges[index];

            Cell[] cellPair = connectingEdges[edge];

            if (cellPair[0].highestBranch == cellPair[1].highestBranch && uniqueids > 1)
            {
                connectingEdges.Remove(edge);
                sharedEdges.Remove(edge);
                continue;
            } else if (uniqueids == 1)
            {
                break;
            }

            if (cellPair[0].highestBranch < cellPair[1].highestBranch)
            {
                edge.SetActive(false);
                connectingEdges.Remove(edge);
                sharedEdges.Remove(edge);
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
