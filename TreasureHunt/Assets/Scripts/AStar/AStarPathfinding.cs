using System;
using System.Collections.Generic;

public class AStarPathfinding 
{
    // 地图元素
    private const char START = 'S';      // 起点
    private const char END = 'E';        // 终点
    private const char SPACE0 = '.';     // 空地0
    private const char SPACE1 = '*';     // 空地1
    private const char WALL = 'W';       // 墙
    private const char VISITED = '-';    // 被访问过
    private const char ON_PATH = '@';    // 在结果路径上
    // 地图字符串
    public static char[,] MAP = null;
    // 地图最大尺寸
    public static Point MAX_PNT = null;
    // 起点
    public static Point START_PNT = null;
    // 终点
    public static Point END_PNT = null;

    /// <summary>
    /// 将游戏地图翻译为寻路地图
    /// </summary>
    /// <param name="s">寻路起点</param>
    /// <param name="e">寻路终点</param>
    /// <returns>是否翻译成功</returns>
    private static bool GenerateMap(Point s,Point e)
    {
        if (s.Equals(e))
        {
            return false;
        }
        MAX_PNT = new Point(GameManager.Instance.w, GameManager.Instance.h);
        START_PNT = s;
        END_PNT = e;
        MAP = new char[MAX_PNT.x, MAX_PNT.y];
        for (int i = 0; i < MAX_PNT.x; i++)
        {
            for (int j = 0; j < MAX_PNT.y; j++)
            {
                if (GameManager.Instance.mapArray[i, j].elementContent == ElementContent.Door ||
                    GameManager.Instance.mapArray[i, j].elementContent == ElementContent.Enemy ||
                    GameManager.Instance.mapArray[i, j].elementContent == ElementContent.BigWall ||
                    GameManager.Instance.mapArray[i, j].elementContent == ElementContent.SmallWall ||
                    GameManager.Instance.mapArray[i, j].elementState == ElementState.Marked ||
                    (GameManager.Instance.mapArray[i, j].elementContent == ElementContent.Trap && GameManager.Instance.mapArray[i, j].elementState == ElementState.Uncovered))
                {
                    MAP[i, j] = WALL;
                }
                else if (GameManager.Instance.mapArray[i, j].elementState == ElementState.Uncovered ||
                    (GameManager.Instance.mapArray[i, j].elementContent == ElementContent.Tool && ((ToolElement)GameManager.Instance.mapArray[i, j]).isHide == false) ||
                    (GameManager.Instance.mapArray[i, j].elementContent == ElementContent.Gold && ((GoldElement)GameManager.Instance.mapArray[i, j]).isHide == false))
                {
                    MAP[i, j] = SPACE0;
                }
                else
                {
                    MAP[i, j] = SPACE1;
                }
            }
        }
        if (MAP[e.x, e.y] == WALL)
        {
            return false;
        }
        MAP[START_PNT.x, START_PNT.y] = START;
        MAP[END_PNT.x, END_PNT.y] = END;
        return true;
    }

    /// <summary>
    /// 曼哈顿距离,小于等于实际值
    /// </summary>
    /// <param name="pnt">当前评估点</param>
    /// <returns>按照曼哈顿距离所评估的H值</returns>
    private static double HManhattanDistance(Point pnt)
    {
        if (MAP[pnt.x, pnt.y] == SPACE0)
        {
            return Math.Abs(pnt.x - END_PNT.x) + Math.Abs(pnt.y - END_PNT.y);
        }
        else
        {
            return Math.Abs(pnt.x - END_PNT.x) + Math.Abs(pnt.y - END_PNT.y) + (GameManager.Instance.w * 100);
        }
    }

    /// <summary>
    /// 欧式距离,小于等于实际值
    /// </summary>
    /// <param name="pnt">当前评估点</param>
    /// <returns>按照欧式距离所评估的H值</returns>
    private static double HEuclidianDistance(Point pnt)
    {
        return Math.Sqrt(Math.Pow(pnt.x - END_PNT.x, 2) + Math.Pow(pnt.y - END_PNT.y, 2));
    }

    /// <summary>
    /// 欧式距离平方,大于等于实际值
    /// </summary>
    /// <param name="pnt">当前评估点</param>
    /// <returns>按照欧式距离平方所评估的H值</returns>
    private static double HPowEuclidianDistance(Point pnt)
    {
        return Math.Pow(pnt.x - END_PNT.x, 2) + Math.Pow(pnt.y - END_PNT.y, 2);
    }

    /// <summary>
    /// H函数
    /// </summary>
    /// <param name="pnt">当前评估点</param>
    /// <returns>按照指定方法所评估的H值</returns>
    private static double HFun(Point pnt)
    {
        return HManhattanDistance(pnt);
    }

    /// <summary>
    /// 寻路函数
    /// </summary>
    /// <param name="pathList">找到的路径点列表</param>
    /// <returns>是否找到路</returns>
    private static bool Search(List<Point> pathList)
    {
        // 用List集合做“开启列表”来记录扩展的点
        List<PointData> openList = new List<PointData>();
        // 八个扩展方向
        int[,] directs = { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 }, { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };
        // 把起点放入堆
        openList.Add(new PointData(START_PNT, 0, 0, null));
        // 找到的最后一个点的数据,用来反推路径
        PointData endData = null;
        // 找到终点或“开启列表”为空时退出循环
        for (bool finish = false; !finish && openList.Count > 0;)
        {
            // 取出f值最小的点
            openList.Sort((x, y) => { return x.F().CompareTo(y.F()); });
            PointData data = openList[0];
            openList.RemoveAt(0);
            Point point = data.point;
            // 将取出的点标识为已访问点
            if (MAP[point.x, point.y] == SPACE0 || MAP[point.x, point.y] == SPACE1)
            {
                MAP[point.x, point.y] = VISITED;
            }
            // 遍历八个方向的点
            for (int i = 0; i < directs.GetUpperBound(0) + 1; i ++)
            {
                Point newPoint = new Point(point.x + directs[i, 0], point.y + directs[i, 1]);
                if (newPoint.x >= 0 && newPoint.x < MAX_PNT.x && newPoint.y >= 0 && newPoint.y < MAX_PNT.y)
                {
                    // 如果是终点,则跳出循环,不用再找
                    char e = MAP[newPoint.x, newPoint.y];
                    if (e == END)
                    {
                        endData = data;
                        finish = true;
                        break;
                    }
                    // 如果不是空地,就不需要再扩展
                    if (e != SPACE0 && e != SPACE1)
                    {
                        continue;
                    }
                    // 如果在“开启列表”里,则更新g值
                    PointData tempData = openList.Find((x) => { return x.point.Equals(newPoint); });
                    if (tempData != null)
                    {
                        float goffset;
                        if (Math.Abs(directs[i, 0]) + Math.Abs(directs[i, 1]) > 1)
                        {
                            goffset = 1.4f;
                        }
                        else
                        {
                            goffset = 1.0f;
                        }
                        if (tempData.g > data.g + goffset)
                        {
                            tempData.g = data.g + goffset;
                            tempData.parent = data;
                        }
                    }
                    // 如果不在“开启列表”里,则放入“开启列表”中,并计算g,h值
                    else
                    {
                        float goffset;
                        if (Math.Abs(directs[i, 0]) + Math.Abs(directs[i, 1]) > 1)
                        {
                            goffset = 1.4f;
                        }
                        else
                        {
                            goffset = 1.0f;
                        }
                        double h = HFun(newPoint);
                        PointData newData = new PointData(newPoint, data.g + goffset, h, data);
                        openList.Add(newData);
                    }
                }
            }
        }
        // 反向找出路径
        pathList.Add(END_PNT);
        for (PointData pathData = endData; pathData != null;)
        {
            Point point = pathData.point;
            if (MAP[point.x, point.y] == VISITED)
            {
                MAP[point.x, point.y] = ON_PATH;
                pathList.Add(new Point(point.x, point.y));
            }
            pathData = pathData.parent;
        }
        pathList.Add(START_PNT);
        pathList.Reverse();
        //判断是否找到路
        float distance = (float)Math.Sqrt(Math.Pow(START_PNT.x - END_PNT.x, 2) + Math.Pow(START_PNT.y - END_PNT.y, 2));
        if (distance > 1.5f && pathList.Count <= 2)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 寻路方法
    /// </summary>
    /// <param name="s">起点</param>
    /// <param name="e">终点</param>
    /// <param name="pathList">路径点列表</param>
    /// <returns>是否成功</returns>
    public static bool FindPath(Point s,Point e,List<Point> pathList)
    {
        return GenerateMap(s, e) && Search(pathList);
    }
}
