using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;



public class Maze : MonoBehaviour
{
    public IntVector2 size;

    public MazeCell cellPrefab;

    public MazePassage passagePrefab;

    public MazeWall wallPrefab;

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

    //记录通道的东西南北
    void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazePassage passage = Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(cell, otherCell, direction);
        passage = Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(otherCell, cell, direction.GetOpposite());
    }

    //记录墙的东西南北
    void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazeWall wall = Instantiate(wallPrefab) as MazeWall;
        wall.Initialize(cell, otherCell, direction);
        if (otherCell != null)
        {
            wall = Instantiate(wallPrefab) as MazeWall;
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
        MazeDirection direction = MazeDirections.RandomValue;
        //取一个随机方向+1
        IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();
        if (ContainsCoordinates(coordinates))
        {
            MazeCell neighbor = GetCell(coordinates);

            //若隔壁的点没有取到
            if (neighbor == null)
            {
                //创建新格子，并标记当前格子和新格子的位置信息
                neighbor = CreateCell(coordinates);
                CreatePassage(currentCell, neighbor, direction);
                activeCells.Add(neighbor);
            }
            else//若隔壁已经有格子了就生成墙
            {
                CreateWall(currentCell, neighbor, direction);
                //剔除当前，继续找一格子
                activeCells.RemoveAt(currentIndex);
            }
        }
        else
        {
            //超出边界就修墙。
            CreateWall(currentCell, null, direction);
            activeCells.RemoveAt(currentIndex);
        }

    }


}
