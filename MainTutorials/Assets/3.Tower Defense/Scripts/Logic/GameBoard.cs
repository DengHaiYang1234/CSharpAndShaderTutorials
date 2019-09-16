using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//地图生成类
public class GameBoard : MonoBehaviour
{
    [SerializeField]
    //地板
    Transform ground;
    [SerializeField]
    //箭头
    GameTile titlePrefab;

    [SerializeField]
    Texture2D gridTexture;

    Vector2 size;

    //箭头集合
    GameTile[] tiles;

    Queue<GameTile> searchFrontier = new Queue<GameTile>();


    GameTileContentFactory contentFactory;

    bool showPaths, showGrid;

    //出生点集合
    List<GameTile> spawnPoints = new List<GameTile>();

    List<GameTileContent> updatingContent = new List<GameTileContent>();

    public void GameUpdate()
    {
        for (int i = 0; i < updatingContent.Count; i++)
        {
            updatingContent[i].GameUpdate();
        }
    }

    public int SpawnPointCount
    {
        get
        {
            return spawnPoints.Count;
        }
    }


    public void Initialize(Vector2 size, GameTileContentFactory contentFactory)
    {
        //生成地板
        this.size = size;
        this.contentFactory = contentFactory;
        ground.localScale = new Vector3(size.x, size.y, 1f);
        //创建箭头
        tiles = new GameTile[(int)(size.x * size.y)];
        //x轴偏移及y轴偏移
        Vector2 offset = new Vector2(
            (size.x - 1) * 0.5f, (size.y - 1) * 0.5f
        );

        //i:个数   x:行  y:列
        for (int i = 0, y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++, i++)
            {
                GameTile tile = tiles[i] = Instantiate(titlePrefab);
                tile.gameObject.name = i.ToString();
                tile.transform.SetParent(transform, false);
                //从左到右排列，以0为中心点
                tile.transform.localPosition = new Vector3(
                    x - offset.x, 0f, y - offset.y
                );

                if (x > 0)
                {   //构建行的左右关系
                    //Tip:从左到右读取
                    GameTile.MakeLeftRightNeighbors(tile, tiles[i - 1]);
                }

                //进入就可以表示已经有第二行了
                if (y > 0)
                {
                    //构建列的上下关系
                    //Tip:i - (int)size.x 当>0时，才表示有下一行，也才可确定当前该列的上下
                    GameTile.MakeUpBottomNeightbors(tile, tiles[i - (int)size.x]);
                }
                //都是把左右两边变成二进制,然后逐位进行运算
                //1 & 1 = 1   1 & 0 = 0   0 & 1 = 0  0 & 0 = 0
                //1 | 1 = 1   1 | 0 = 1   0 | 1 = 1  0 | 0 = 0

                //偶数&1肯定为0
                tile.IsAlternative = (x & 1) == 0;
                if ((y & 1) == 0)
                {	//偶数行取反
                    tile.IsAlternative = !tile.IsAlternative;
                }
                //tile.Content = contentFactory.Get(GameTileContentType.Empty);
            }
        }

        // ToggleDestination(tiles[tiles.Length / 2]);
        // ToggleSpawnPoint(tiles[0]);
        Clear();
    }
    
    

    //寻路算法
    bool FindPaths()
    {
        foreach (GameTile tile in tiles)
        {
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.BecomeDestination();
                searchFrontier.Enqueue(tile);
            }
            else
                tile.ClearPath();
        }

        if (searchFrontier.Count == 0)
        {
            return false;
        }


        // //设置目标位置
        // tiles[tiles.Length / 2].BecomeDestination();
        // searchFrontier.Enqueue(tiles[tiles.Length / 2]);



        //获取每个tile的上下左右位置，累计与初始点的距离，并记录与初始点朝向
        //注意，这里计算寻路是根据终点的位置来计算的（具体的画图理解...）
        while (searchFrontier.Count > 0)
        {
            GameTile tile = searchFrontier.Dequeue();
            if (tile != null)
            {
                //正常朝向
                // searchFrontier.Enqueue(tile.GrowPathUp());
                // searchFrontier.Enqueue(tile.GrowPathBottom());
                // searchFrontier.Enqueue(tile.GrowPathLeft());
                // searchFrontier.Enqueue(tile.GrowPathRight());

                //交叉朝向
                if (tile.IsAlternative)
                {
                    searchFrontier.Enqueue(tile.GrowPathUp());
                    searchFrontier.Enqueue(tile.GrowPathBottom());
                    searchFrontier.Enqueue(tile.GrowPathLeft());
                    searchFrontier.Enqueue(tile.GrowPathRight());
                }
                else
                {
                    searchFrontier.Enqueue(tile.GrowPathRight());
                    searchFrontier.Enqueue(tile.GrowPathLeft());
                    searchFrontier.Enqueue(tile.GrowPathBottom());
                    searchFrontier.Enqueue(tile.GrowPathUp());
                }

            }
        }

        foreach (var tile in tiles)
        {
            if (!tile.HashPath)
            {
                return false;
            }
        }

        if (showPaths)
        {
            foreach (GameTile tile in tiles)
                tile.ShowPath();
        }

        return true;
    }

    public GameTile GetTile(Ray ray)
    {
        //实现原理：根据鼠标点击传过来的射线与带有碰撞器的物体碰撞所产生的坐标hit.point，就代表当前所点击的世界空间下的位置，
        //那么通过(int)(hit.point.x + size.x * 0.5f)可以算出这是某行中的第几个
        //通过(int)(hit.point.z + size.y * 0.5f)可以算出某列中的第几个
        //最后因为每行之间相差size.x个元素，所以x + y * (int)size.x可以算出当前点击位置，在所有obj里面的index（所有obj的存储都是升序）
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, float.MaxValue, 1);
        if (isHit)
        {
            int x = (int)(hit.point.x + size.x * 0.5f);
            int y = (int)(hit.point.z + size.y * 0.5f);
            if (x >= 0 && x < size.x && y >= 0 && y < size.y)
            {
                return tiles[x + y * (int)size.x];
            }
        }
        return null;
    }

    public GameTile GetSpawnPoint(int index)
    {
        return spawnPoints[index];
    }

    //终点
    public void ToggleDestination(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Destination)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Destination);
                FindPaths();
            }
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Destination);
            FindPaths();
        }
    }
    //墙
    public void ToggleWall(GameTile tile)
    {
        //若已经是Wall
        if (tile.Content.Type == GameTileContentType.Wall)
        {
            //那么就清除
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            //刷新Path
            FindPaths();
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Wall);
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
    }
    //塔
    public void ToggleTower(GameTile tile, TowerType towerType)
    {
        if (tile.Content.Type == GameTileContentType.Tower)
        {
            updatingContent.Remove(tile.Content);
            if (((Tower) tile.Content).TowerType == towerType)
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
            else
            {
                tile.Content = contentFactory.Get(towerType);
                updatingContent.Add(tile.Content);
            }
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(towerType);
            if (FindPaths())
            {
                updatingContent.Add(tile.Content);
            }
            else
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
        else if (tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = contentFactory.Get(towerType);
            updatingContent.Add(tile.Content);
        }
    }
    //Enemy出生点
    public void ToggleSpawnPoint(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.SpawnPoint)
        {
            if (spawnPoints.Count > 1)
            {
                //再次点击清空
                spawnPoints.Remove(tile);
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
            }
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.SpawnPoint);
            spawnPoints.Add(tile);
        }
    }

    public bool ShowPaths
    {
        get
        {
            return showPaths;
        }
        set
        {
            showPaths = value;
            if (showPaths)
            {
                foreach (var tile in tiles)
                {
                    tile.ShowPath();
                }
            }
            else
            {
                foreach (var tile in tiles)
                {
                    tile.HidePath();
                }
            }
        }
    }



    public bool ShowGrid
    {
        get
        {
            return showGrid;
        }
        set
        {
            showGrid = value;
            Material m = ground.GetComponent<MeshRenderer>().material;
            if (showGrid)
            {
                m.mainTexture = gridTexture;
                m.SetTextureScale("_MainTex", size);
            }
            else
            {
                m.mainTexture = null;
            }
        }
    }

    public void Clear()
    {
        foreach (GameTile tile in tiles)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
        }

        spawnPoints.Clear();
        updatingContent.Clear();
        ToggleDestination(tiles[tiles.Length / 2]);
        ToggleSpawnPoint(tiles[0]);
    }
}
