using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField]
    //地板
    Transform ground;
    [SerializeField]
    //箭头
    GameTile titlePrefab;

    Vector2 size;

    //箭头集合
    GameTile[] tiles;

    Queue<GameTile> searchFrontier = new Queue<GameTile>();




    public void Initialize(Vector2 size)
    {
        //生成地板
        this.size = size;
        ground.localScale = new Vector3(size.x, size.y, 1f);
        //创建箭头
        tiles = new GameTile[(int)(size.x * size.y)];
        //获取间距
        Vector2 offset = new Vector2(
            (size.x - 1) * 0.5f, (size.y - 1) * 0.5f
        );

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
            }
        }

        FindPaths();
    }

    void FindPaths()
    {
        foreach (GameTile tile in tiles)
        {
            tile.ClearPath();
        }

        //设置目标位置
        tiles[tiles.Length / 2].BecomeDestination();
        searchFrontier.Enqueue(tiles[tiles.Length / 2]);


        //获取每个tile的上下左右位置，累计与初始点的距离，并记录与初始点朝向
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

        foreach (GameTile tile in tiles)
            tile.ShowPath();
    }



}
