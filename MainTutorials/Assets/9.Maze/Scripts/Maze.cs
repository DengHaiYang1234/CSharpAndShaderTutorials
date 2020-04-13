using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;



public class Maze : MonoBehaviour
{
    public IntVector2 size;

    public MazeCell cellPrefab;

    public MazePassage passagePrefab;

    public MazeWall[] wallPrefabs;

    public MazeDoor doorPrefab;

    [Range(0f,1f)]
    public float doorProbability;

    public float generationStepDelay;

    private MazeCell[,] cells;

    public IntVector2 RandomCoordionates
    {
        get { return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z)); }
    }

    public MazeCell GetCell(IntVector2 coordinates)
    {
        return cells[coordinates.x, coordinates.z];
    }

    //生成迷宫
    public IEnumerator Generate()
    {
        WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
        cells = new MazeCell[size.x, size.z];

        //有效格子数
        List<MazeCell> activeCells = new List<MazeCell>();
        DoFirstGenerationStep(activeCells);

        while (activeCells.Count > 0)
        {
            yield return delay;
            DoNextGenerationStep(activeCells);
        }
    }

    //检测边界
    public bool ContainsCoordinates(IntVector2 coordinate)
    {
        return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
    }

    //创建方块
    MazeCell CreateCell(IntVector2 coordinates)
    {
        MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
        cells[coordinates.x, coordinates.z] = newCell;
        newCell.coordinates = coordinates;
        newCell.name = "Maze Cell " + coordinates.x + "," + coordinates.z;
        newCell.transform.parent = transform;
        //以中间点为中心点。每个格子相隔1
        newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);
        return newCell;
    }

    //记录通道（两个cell之间相对位置）
    void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazePassage prefab = Random.value < doorProbability ? doorPrefab : passagePrefab;
        MazePassage passage = Instantiate(prefab) as MazePassage;
        passage.Initialize(cell, otherCell, direction);
        passage = Instantiate(prefab) as MazePassage;
        passage.Initialize(otherCell, cell, direction.GetOpposite());
    }

    //记录墙（两个cell之间相对位置）
    void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        int rangeIndex = Random.Range(0, wallPrefabs.Length);

        MazeWall wall = Instantiate(wallPrefabs[rangeIndex]) as MazeWall;
        wall.Initialize(cell, otherCell, direction);
        if (otherCell != null)
        {
            wall = Instantiate(wallPrefabs[rangeIndex]) as MazeWall;
            wall.Initialize(otherCell, cell, direction.GetOpposite());
        }
    }

    //先随机一个
    void DoFirstGenerationStep(List<MazeCell> activeCells)
    {
        activeCells.Add(CreateCell(RandomCoordionates));
    }

    //根据上一个创建下一个
    void DoNextGenerationStep(List<MazeCell> activeCells)
    {
        int currentIndex = activeCells.Count - 1;
        MazeCell currentCell = activeCells[currentIndex];

        //四个方向全部初始化
        if (currentCell.IsFullyInitialized)
        {
            activeCells.RemoveAt(currentIndex);
            return;
        }

        MazeDirection direction = currentCell.RandomUninitializedDirection;
        //基于上个cell，然后随机初始化的方向，来确定下一个cell位置（neighbor）
        IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();
        if (ContainsCoordinates(coordinates))
        {
            MazeCell neighbor = GetCell(coordinates);

            //若隔壁地板还不存在，那么默认设置为通道，可移动
            if (neighbor == null)
            {
                //创建新格子，并标记当前格子和新格子的位置信息
                neighbor = CreateCell(coordinates);
                CreatePassage(currentCell, neighbor, direction);
                activeCells.Add(neighbor);
            }
            else//若隔壁已经有格子了就生成墙，不能移动
            {
                CreateWall(currentCell, neighbor, direction);
            }
        }
        else
        {
            //边界位置就修墙。
            CreateWall(currentCell, null, direction);
        }
    }
}
