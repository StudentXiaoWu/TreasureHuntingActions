using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class GameManager : MonoBehaviour 
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    [Header("角色游戏物体")]
    public GameObject player;
    public Animator ani;

    [Header("元素预制体")]
    public GameObject bgElement;
    [Tooltip("边界预制体，顺序为：\n上、下、左、右、左上、右上、左下、右下")]
    public GameObject[] borderElements;
    public GameObject baseElement;
    public GameObject flagElement;
    public GameObject errorElement;

    [Header("特效预制体")]
    public GameObject smokeEffect;
    public GameObject uncoveredEffect;
    public GameObject goldEffect;
    public GameObject doorOpenEffect;

    [Header("道具范围提示效果")]
    public GameObject hoeSelected;
    public GameObject tntSelected;
    public GameObject mapSelected;

    [Header("图片资源")]
    public Sprite[] coverTileSprites;
    public Sprite[] trapSprites;
    public Sprite[] numberSprites;
    public Sprite[] toolSprites;
    public Sprite[] goldSprites;
    public Sprite[] bigwallSprites;
    public Sprite[] smallwallSprites;
    public Sprite[] enemySprites;
    public Sprite doorSprite;
    public Sprite exitSprite;

    [Header("关卡设置")]
    public int w;
    public int h;
    public float minTrapProbability;
    public float maxTrapProbability;
    public float uncoveredProbability;
    public int standAreaW;
    public int obstacleAreaW;
    [HideInInspector]
    public int obstacleAreaNum;

    [Header("关卡信息")]
    public int lv;
    public int hp;
    public int armor;
    public int key;
    public WeaponType weaponType;
    public int arrow;
    public int hoe;
    public int tnt;
    public int map;
    public bool isGrass;
    public int gold;

    [Header("UI面板")]
    public GameObject passPanel;
    public GameObject endPanel;

    public BaseElement[,] mapArray;
    public Tweener pathTweener;
    [HideInInspector]
    public bool pathFinding = false;
    [HideInInspector]
    public Vector3Int prePos;
    [HideInInspector]
    public Vector3Int nowPos;
    [HideInInspector]
    public Vector2Int dir;
    [HideInInspector]
    public bool isOver = false;

    private void Awake()
    {
        _instance = this;
        LoadData();
        mapArray = new BaseElement[w, h];
        obstacleAreaNum = (w - (standAreaW + 3)) / obstacleAreaW;
        AudioManager.Instance.PlayBGM();
    }

    private void Start()
    {
        CreateMap();
        ResetCamera();
        InitMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetTarget();
        }
        nowPos = player.transform.position.ToVector3Int();
        if (prePos != nowPos)
        {
            dir = new Vector2Int(Mathf.Clamp(nowPos.x - prePos.x, -1, 1), Mathf.Clamp(nowPos.y - prePos.y, -1, 1));
            ani.SetFloat("DirX", dir.x);
            ani.SetFloat("DirY", dir.y);
            mapArray[nowPos.x, nowPos.y].OnPlayerStand();
            mapArray[nowPos.x, nowPos.y].OnPlayerStand();
            if (mapArray[nowPos.x, nowPos.y].elementContent == ElementContent.Trap)
            {
                pathTweener.Kill();
                nowPos = prePos;
                player.transform.position = nowPos;
            }
            else
            {
                prePos = nowPos;
            }
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            player.transform.GetChild(0).transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0)) + new Vector3(0, 0, 10);
        }
        if (Input.GetMouseButton(0))
        {
            player.transform.GetChild(0).transform.position -= new Vector3(Input.GetAxis("Mouse X") * 0.5f, 0, 0);
        }
    }

    private void OnPanelShow()
    {
        transform.position = new Vector3(0, 0, -1);
    }

    private void LoadData()
    {
        DataManager.Instance.LoadData();
        w = DataManager.Instance.w;
        h = DataManager.Instance.h;
        lv = DataManager.Instance.lv;
        hp = DataManager.Instance.hp;
        armor = DataManager.Instance.armor;
        key = DataManager.Instance.key;
        hoe = DataManager.Instance.hoe;
        tnt = DataManager.Instance.tnt;
        map = DataManager.Instance.map;
        gold = DataManager.Instance.gold;
    }

    public void OnLevelPass()
    {
        if (isOver) return;
        isOver = true;
        if (hp < 3)
        {
            hp = 3;
        }
        Instance.lv++;
        DataManager.Instance.SaveData(lv,hp,armor,key,hoe,tnt,map,gold);
        ani.SetBool("Pass", true);
        passPanel.SetActive(true);
        OnPanelShow();
        Camera.main.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        AudioManager.Instance.PlayClip(AudioManager.Instance.pass);
        AudioManager.Instance.PlayClip(AudioManager.Instance.winbg);
    }

    /// <summary>
    /// 受伤方法
    /// </summary>
    public void TakeDamage()
    {
        if (armor > 0)
        {
            armor--;
            MainPanel.Instance.UpdateUI(MainPanel.Instance.armorIcon.rectTransform, MainPanel.Instance.armorText.rectTransform);
        }
        else
        {
            hp--;
            MainPanel.Instance.UpdateUI(MainPanel.Instance.hpText.rectTransform);
        }
        if (hp == 0)
        {
            DisplayAllTraps();
            ani.SetBool("Die", true);
            DataManager.Instance.ResetData();
            endPanel.SetActive(true);
            OnPanelShow();
            AudioManager.Instance.PlayClip(AudioManager.Instance.die);
            AudioManager.Instance.PlayClip(AudioManager.Instance.end);
        }
        else
        {
            ani.SetTrigger("TakeDamage");
            AudioManager.Instance.PlayClip(AudioManager.Instance.hurt);
        }
    }

    /// <summary>
    /// 寻路方法
    /// </summary>
    /// <param name="e">寻路终点</param>
    public void FindPath(Point e)
    {
        if (pathFinding == true) pathTweener.Kill();
        Point s = new Point((int)player.transform.position.x, (int)player.transform.position.y);
        List<Point> pathList = new List<Point>();
        if (AStarPathfinding.FindPath(s, e, pathList) == false)
        {
            ani.SetTrigger("Why");
            AudioManager.Instance.PlayClip(AudioManager.Instance.why);
            return;
        } 
        ResetTarget();
        pathFinding = true;
        AudioManager.Instance.PlayClip(AudioManager.Instance.move);
        ani.SetBool("Idle", !pathFinding);
        pathTweener = player.transform.DOPath(pathList.ToVector3Array(), pathList.Count * 0.1f);
        pathTweener.SetEase(Ease.Linear);
        pathTweener.onComplete += () => {
            pathFinding = false;
            ani.SetBool("Idle", !pathFinding);
        };
        pathTweener.onKill += () => {
            pathFinding = false;
            ani.SetBool("Idle", !pathFinding);
        };
    }

    /// <summary>
    /// 将虚拟摄像机的跟随目标与玩家的位置同步
    /// </summary>
    private void ResetTarget()
    {
        player.transform.GetChild(0).transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 创建关卡地图
    /// </summary>
    private void CreateMap()
    {
        Transform elementHolder = GameObject.Find("ElementsHolder").transform;
        Transform bgHolder = GameObject.Find("ElementsHolder/Background").transform;
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                Instantiate(bgElement, new Vector3(i, j, 0), Quaternion.identity, bgHolder);
                mapArray[i, j] = Instantiate(baseElement, new Vector3(i, j, 0), Quaternion.identity, elementHolder).GetComponent<BaseElement>();
            }
        }
        for (int i = 0; i < w; i++)
        {
            Instantiate(borderElements[0], new Vector3(i, h + 0.25f, 0), Quaternion.identity, bgHolder);
            Instantiate(borderElements[1], new Vector3(i, -1.25f, 0), Quaternion.identity, bgHolder);
        }
        for (int i = 0; i < h; i++)
        {
            Instantiate(borderElements[2], new Vector3(-1.25f, i, 0), Quaternion.identity, bgHolder);
            Instantiate(borderElements[3], new Vector3(w + 0.25f, i, 0), Quaternion.identity, bgHolder);
        }
        Instantiate(borderElements[4], new Vector3(-1.25f, h + 0.25f, 0), Quaternion.identity, bgHolder);
        Instantiate(borderElements[5], new Vector3(w + 0.25f, h + 0.25f, 0), Quaternion.identity, bgHolder);
        Instantiate(borderElements[6], new Vector3(-1.25f, -1.25f, 0), Quaternion.identity, bgHolder);
        Instantiate(borderElements[7], new Vector3(w + 0.25f, -1.25f, 0), Quaternion.identity, bgHolder);
    }

    /// <summary>
    /// 设置摄像机的参数与关卡地图匹配
    /// </summary>
    private void ResetCamera()
    {
        CinemachineVirtualCamera vCam = GameObject.Find("VCam").GetComponent<CinemachineVirtualCamera>();
        vCam.m_Lens.OrthographicSize = (h + 3) / 2f;
        CinemachineFramingTransposer ft = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFramingTransposer;
        ft.m_DeadZoneHeight = (h * 100) / (300 + h * 100f);
        ft.m_DeadZoneWidth = (h * 100) / (300 + h * 100f) / 9 * 16 / h;
        GetComponent<PolygonCollider2D>().SetPath(0,
            new Vector2[] {
                new Vector2(-2f,-2f),
                new Vector2(-2f,h+1f),
                new Vector2(w+1f,h+1f),
                new Vector2(w+1f,-2f)
            });
        Camera.main.transform.GetChild(0).transform.localPosition = new Vector3(0, vCam.m_Lens.OrthographicSize, 0);
        ParticleSystem.ShapeModule sm = Camera.main.transform.GetChild(0).GetComponent<ParticleSystem>().shape;
        sm.scale = new Vector3(vCam.m_Lens.OrthographicSize * 35.5f / 10, 1, 1);
    }

    /// <summary>
    /// 初始化地图元素
    /// </summary>
    private void InitMap()
    {
        List<int> availableIndex = new List<int>();
        for (int i = 0; i < w * h; i++)
        {
            availableIndex.Add(i);
        }
        int standAreaY = Random.Range(1, h - 1);
        GenerateExit(availableIndex);
        GenerateObstacleArea(availableIndex);
        GenerateTool(availableIndex);
        GenerateGold(availableIndex);
        GenerateTrap(standAreaY, availableIndex);
        GenerateNumber(availableIndex);
        GenerateStandArea(standAreaY);
    }

    /// <summary>
    /// 生成出口
    /// </summary>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void GenerateExit(List<int> availableIndex)
    {
        float x = w - 1.5f;
        float y = Random.Range(1, h) - 0.5f;
        BaseElement exit = SetElement(GetIndex((int)(x + 0.5), (int)(y - 0.5)), ElementContent.Exit);
        exit.transform.position = new Vector3(x, y, 0);
        Destroy(exit.GetComponent<BoxCollider2D>());
        exit.gameObject.AddComponent<BoxCollider2D>();
        availableIndex.Remove(GetIndex((int)(x + 0.5), (int)(y - 0.5)));
        availableIndex.Remove(GetIndex((int)(x + 0.5), (int)(y + 0.5)));
        availableIndex.Remove(GetIndex((int)(x - 0.5), (int)(y - 0.5)));
        availableIndex.Remove(GetIndex((int)(x - 0.5), (int)(y + 0.5)));
        Destroy(mapArray[(int)(x + 0.5), (int)(y + 0.5)].gameObject);
        Destroy(mapArray[(int)(x - 0.5), (int)(y - 0.5)].gameObject);
        Destroy(mapArray[(int)(x - 0.5), (int)(y + 0.5)].gameObject);
        mapArray[(int)(x + 0.5), (int)(y + 0.5)] = exit;
        mapArray[(int)(x - 0.5), (int)(y - 0.5)] = exit;
        mapArray[(int)(x - 0.5), (int)(y + 0.5)] = exit;
    }

    /// <summary>
    /// 生成障碍物区域
    /// </summary>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void GenerateObstacleArea(List<int> availableIndex)
    {
        for (int i = 0; i < obstacleAreaNum; i++)
        {
            if (Random.value < 0.5f)
            {
                CreateCloseArea(i, availableIndex);
            }
            else
            {
                CreateRandomWall(i, availableIndex);
            }
        }
    }

    /// <summary>
    /// 闭合区域信息结构体
    /// </summary>
    private struct CloseAreaInfo
    {
        public int x, y, sx, ex, sy, ey;
        public int doorType;
        public Vector2 doorPos;
        public int tx, ty;
        public ToolElement t;
        public int gx, gy;
        public GoldElement g;
        public int innerCount, goldNum;
    }

    /// <summary>
    /// 生成闭合区域信息
    /// </summary>
    /// <param name="type">闭合区域类型，0：与边界闭合；1：自闭合</param>
    /// <param name="nowArea">闭合区域索引值</param>
    /// <param name="info">要生成的闭合区域信息结构体</param>
    private void CreateCloseAreaInfo(int type,int nowArea,ref CloseAreaInfo info)
    {
        switch (type)
        {
            case 0:
                info.x = Random.Range(3, obstacleAreaW - 2);
                info.y = Random.Range(3, h - 3);
                info.sx = standAreaW + nowArea * obstacleAreaW + 1;
                info.ex = info.sx + info.x;
                info.doorType = Random.Range(4, 8);
                break;
            case 1:
                info.x = Random.Range(3, obstacleAreaW - 2);
                info.y = Random.Range(3, info.x + 1);
                info.sx = standAreaW + nowArea * obstacleAreaW + 1;
                info.ex = info.sx + info.x;
                info.sy = Random.Range(3, h - info.y - 1);
                info.ey = info.sy + info.y;
                info.doorType = (int)ElementContent.BigWall;
                break;
        }
    }

    /// <summary>
    /// 生成U或L闭合障碍物区域的门
    /// </summary>
    /// <param name="info">闭合区域信息</param>
    /// <param name="availableIndex">要生成的闭合区域信息结构体</param>
    private void CreateULShapeAreaDoor(CloseAreaInfo info,List<int> availableIndex)
    {
        availableIndex.Remove(GetIndex((int)info.doorPos.x, (int)info.doorPos.y));
        SetElement(GetIndex((int)info.doorPos.x, (int)info.doorPos.y), (ElementContent)info.doorType);
    }

    /// <summary>
    /// 生成闭合障碍物区内的奖励物品
    /// </summary>
    /// <param name="info">闭合区域信息</param>
    /// <param name="availableIndex">要生成的闭合区域信息结构体</param>
    private void CreateCloseAreaRewards(CloseAreaInfo info, List<int> availableIndex)
    {
        info.innerCount = info.x * info.y;
        info.goldNum = Random.Range(1, Random.value < 0.5f ? info.innerCount + 1 : info.innerCount / 2);
        for (int i = 0; i < info.goldNum; i++)
        {
            info.gy = i / info.x;
            info.gx = i - info.gy * info.x;
            info.gx = info.sx + info.gx + 1;
            info.gy = info.sy + info.gy + 1;
            if (availableIndex.Contains(GetIndex(info.gx, info.gy)))
            {
                availableIndex.Remove(GetIndex(info.gx, info.gy));
                info.g = (GoldElement)SetElement(GetIndex(info.gx, info.gy), ElementContent.Gold);
                info.g.goldType = (GoldType)Random.Range(0, 7);
                if (info.g.isHide == false)
                {
                    info.g.ConfirmSprite();
                }
            }
        }
    }

    /// <summary>
    /// 生成开启闭合障碍物区所需的道具
    /// </summary>
    /// <param name="info">闭合区域信息</param>
    /// <param name="availableIndex">要生成的闭合区域信息结构体</param>
    private void CreateCloseAreaTool(CloseAreaInfo info, List<int> availableIndex)
    {
        info.tx = Random.Range(0, info.sx);
        info.ty = Random.Range(0, h);
        for (; !availableIndex.Contains(GetIndex(info.tx,info.ty)); )
        {
            info.tx = Random.Range(0, info.sx);
            info.ty = Random.Range(0, h);
        }
        availableIndex.Remove(GetIndex(info.tx, info.ty));
        info.t = (ToolElement)SetElement(GetIndex(info.tx, info.ty), ElementContent.Tool);
        info.t.toolType = (ToolType)info.doorType;
        if (info.t.isHide == false)
        {
            info.t.ConfirmSprite();
        }
    }

    /// <summary>
    /// 生成闭合障碍物区域
    /// </summary>
    /// <param name="nowArea">当前障碍物区域的索引值</param>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void CreateCloseArea(int nowArea, List<int> availableIndex)
    {
        int shape = Random.Range(0, 2);
        CloseAreaInfo info = new CloseAreaInfo();
        switch (shape)
        {
            case 0:
                CreateCloseAreaInfo(0, nowArea, ref info);
                int dir = Random.Range(0, 4);
                switch (dir)
                {
                    case 0:
                        info.doorPos = Random.value < 0.5f ? new Vector2(Random.Range(info.sx + 1, info.ex), info.y) : new Vector2(Random.value < 0.5f ? info.sx : info.ex, Random.Range(info.y, h));
                        CreateULShapeAreaDoor(info, availableIndex);
                        for (int i = h - 1; i > info.y; i--)
                        {
                            if (availableIndex.Contains(GetIndex(info.sx, i)))
                            {
                                availableIndex.Remove(GetIndex(info.sx, i));
                                SetElement(GetIndex(info.sx, i), ElementContent.BigWall);
                            }
                        }
                        for (int i = info.sx; i < info.ex; i++)
                        {
                            if (availableIndex.Contains(GetIndex(i, info.y)))
                            {
                                availableIndex.Remove(GetIndex(i, info.y));
                                SetElement(GetIndex(i, info.y), ElementContent.BigWall);
                            }
                        }
                        for (int i = h - 1; i >= info.y; i--)
                        {
                            if (availableIndex.Contains(GetIndex(info.ex, i)))
                            {
                                availableIndex.Remove(GetIndex(info.ex, i));
                                SetElement(GetIndex(info.ex, i), ElementContent.BigWall);
                            }
                        }
                        info.sy = info.y;
                        info.ey = h - 1;
                        info.y = h - 1 - info.y;
                        CreateCloseAreaRewards(info, availableIndex);
                        break;
                    case 1:
                        info.doorPos = Random.value < 0.5f ? new Vector2(Random.Range(info.sx + 1, info.ex), info.y) : new Vector2(Random.value < 0.5f ? info.sx : info.ex, Random.Range(0, info.y));
                        CreateULShapeAreaDoor(info, availableIndex);
                        for (int i = 0; i < info.y; i++)
                        {
                            if (availableIndex.Contains(GetIndex(info.sx, i)))
                            {
                                availableIndex.Remove(GetIndex(info.sx, i));
                                SetElement(GetIndex(info.sx, i), ElementContent.BigWall);
                            }
                        }
                        for (int i = info.sx; i < info.ex; i++)
                        {
                            if (availableIndex.Contains(GetIndex(i, info.y)))
                            {
                                availableIndex.Remove(GetIndex(i, info.y));
                                SetElement(GetIndex(i, info.y), ElementContent.BigWall);
                            }
                        }
                        for (int i = 0; i <= info.y; i++)
                        {
                            if (availableIndex.Contains(GetIndex(info.ex, i)))
                            {
                                availableIndex.Remove(GetIndex(info.ex, i));
                                SetElement(GetIndex(info.ex, i), ElementContent.BigWall);
                            }
                        }
                        info.sy = 0;
                        info.ey = info.y;
                        CreateCloseAreaRewards(info, availableIndex);
                        break;
                    case 2:
                        info.doorPos = Random.value < 0.5f ? new Vector2(Random.Range(info.sx + 1, info.ex), info.y) : new Vector2(info.sx, Random.Range(info.y, h));
                        CreateULShapeAreaDoor(info, availableIndex);
                        for (int i = h - 1; i > info.y; i--)
                        {
                            if (availableIndex.Contains(GetIndex(info.sx, i)))
                            {
                                availableIndex.Remove(GetIndex(info.sx, i));
                                SetElement(GetIndex(info.sx, i), ElementContent.BigWall);
                            }
                        }
                        for (int i = info.sx; i < info.ex; i++)
                        {
                            if (availableIndex.Contains(GetIndex(i, info.y)))
                            {
                                availableIndex.Remove(GetIndex(i, info.y));
                                SetElement(GetIndex(i, info.y), ElementContent.BigWall);
                            }
                        }
                        for (int i = 0; i <= info.y; i++)
                        {
                            if (availableIndex.Contains(GetIndex(info.ex, i)))
                            {
                                availableIndex.Remove(GetIndex(info.ex, i));
                                SetElement(GetIndex(info.ex, i), ElementContent.BigWall);
                            }
                        }
                        break;
                    case 3:
                        info.doorPos = Random.value < 0.5f ? new Vector2(Random.Range(info.sx + 1, info.ex), info.y) : new Vector2(info.sx, Random.Range(0, info.y));
                        CreateULShapeAreaDoor(info, availableIndex);
                        for (int i = 0; i < info.y; i++)
                        {
                            if (availableIndex.Contains(GetIndex(info.sx, i)))
                            {
                                availableIndex.Remove(GetIndex(info.sx, i));
                                SetElement(GetIndex(info.sx, i), ElementContent.BigWall);
                            }
                        }
                        for (int i = info.sx; i < info.ex; i++)
                        {
                            if (availableIndex.Contains(GetIndex(i, info.y)))
                            {
                                availableIndex.Remove(GetIndex(i, info.y));
                                SetElement(GetIndex(i, info.y), ElementContent.BigWall);
                            }
                        }
                        for (int i = h - 1; i >= info.y; i--)
                        {
                            if (availableIndex.Contains(GetIndex(info.ex, i)))
                            {
                                availableIndex.Remove(GetIndex(info.ex, i));
                                SetElement(GetIndex(info.ex, i), ElementContent.BigWall);
                            }
                        }
                        break;
                }
                CreateCloseAreaTool(info, availableIndex);
                break;
            case 1:
                CreateCloseAreaInfo(1, nowArea, ref info);
                for (int i = info.sx; i <= info.ex; i++)
                {
                    if (availableIndex.Contains(GetIndex(i,info.sy)))
                    {
                        availableIndex.Remove(GetIndex(i, info.sy));
                        SetElement(GetIndex(i, info.sy), ElementContent.BigWall);
                    }
                    if (availableIndex.Contains(GetIndex(i, info.ey)))
                    {
                        availableIndex.Remove(GetIndex(i, info.ey));
                        SetElement(GetIndex(i, info.ey), ElementContent.BigWall);
                    }
                }
                for (int i = info.sy + 1; i < info.ey; i++)
                {
                    if (availableIndex.Contains(GetIndex(info.sx,i)))
                    {
                        availableIndex.Remove(GetIndex(info.sx, i));
                        SetElement(GetIndex(info.sx, i), ElementContent.BigWall);
                    }
                    if (availableIndex.Contains(GetIndex(info.ex, i)))
                    {
                        availableIndex.Remove(GetIndex(info.ex, i));
                        SetElement(GetIndex(info.ex, i), ElementContent.BigWall);
                    }
                }
                CreateCloseAreaTool(info, availableIndex);
                CreateCloseAreaRewards(info, availableIndex);
                break;
        }
    }

    /// <summary>
    /// 生成随机离散障碍物
    /// </summary>
    /// <param name="nowArea">当前障碍物区域的索引值</param>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void CreateRandomWall(int nowArea, List<int> availableIndex)
    {
        for (int i = 0; i < 5; i++)
        {
            int sx = standAreaW + nowArea * obstacleAreaW + 1;
            int ex = sx + obstacleAreaW;
            int wx = Random.Range(sx, ex);
            int wy = Random.Range(0, h);
            for (; !availableIndex.Contains(GetIndex(wx, wy));) 
            {
                wx = Random.Range(sx, ex);
                wy = Random.Range(0, h);
            }
            availableIndex.Remove(GetIndex(wx, wy));
            SetElement(GetIndex(wx, wy), Random.value < 0.5f ? ElementContent.SmallWall : ElementContent.BigWall);
        }
    }

    /// <summary>
    /// 生成道具
    /// </summary>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void GenerateTool(List<int> availableIndex)
    {
        for (int i = 0; i < 3; i++)
        {
            int tempIndex = availableIndex[Random.Range(0, availableIndex.Count)];
            availableIndex.Remove(tempIndex);
            ToolElement t = (ToolElement)SetElement(tempIndex, ElementContent.Tool);
            t.toolType = (ToolType)Random.Range(0, 9);
            if (t.isHide == false)
            {
                t.ConfirmSprite();
            }
        }
    }

    /// <summary>
    /// 生成金币
    /// </summary>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void GenerateGold(List<int> availableIndex)
    {
        for (int i = 0; i < obstacleAreaNum * 3; i++) 
        {
            int tempIndex = availableIndex[Random.Range(0, availableIndex.Count)];
            availableIndex.Remove(tempIndex);
            GoldElement g = (GoldElement)SetElement(tempIndex, ElementContent.Gold);
            g.goldType = (GoldType)Random.Range(0, 7);
            if (g.isHide == false)
            {
                g.ConfirmSprite();
            }
        }
    }

    /// <summary>
    /// 生成陷阱
    /// </summary>
    /// <param name="standAreaY">站立区域中心y坐标</param>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void GenerateTrap(int standAreaY, List<int> availableIndex)
    {
        float trapProbability = Random.Range(minTrapProbability, maxTrapProbability);
        int trapNum = (int)(availableIndex.Count * trapProbability);
        for (int i = 0; i < trapNum; i++)
        {
            int tempIndex = availableIndex[Random.Range(0, availableIndex.Count)];
            int x, y;
            GetPosition(tempIndex, out x, out y);
            if (x >= 0 && x < standAreaW && y >= standAreaY - 1 && y <= standAreaY + 1) continue;
            availableIndex.Remove(tempIndex);
            SetElement(tempIndex, ElementContent.Trap);
        }
    }

    /// <summary>
    /// 生成数字
    /// </summary>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void GenerateNumber(List<int> availableIndex)
    {
        foreach (int i in availableIndex)
        {
            SetElement(i, ElementContent.Number);
        }
        availableIndex.Clear();
    }

    /// <summary>
    /// 生成站立区域
    /// </summary>
    /// <param name="y">站立区域中心y坐标</param>
    private void GenerateStandArea(int y)
    {
        for (int i = 0; i < standAreaW; i++)
        {
            for (int j = y - 1; j <= y + 1; j++) 
            {
                ((SingleCoveredElement)mapArray[i, j]).UncoveredElementSingle();
            }
        }
        player.transform.position = new Vector3(1, y, 0);
        prePos = nowPos = player.transform.position.ToVector3Int();
        mapArray[1, y].OnPlayerStand();
    }

    /// <summary>
    /// 设置元素的类型
    /// </summary>
    /// <param name="index">元素所在位置的一维索引值</param>
    /// <param name="content">需要设置的类型</param>
    /// <returns>设置好的新类型的组件</returns>
    private BaseElement SetElement(int index,ElementContent content)
    {
        int x, y;
        GetPosition(index, out x, out y);
        GameObject tempGo = mapArray[x, y].gameObject;
        Destroy(tempGo.GetComponent<BaseElement>());
        switch (content)
        {
            case ElementContent.Number:
                mapArray[x, y] = tempGo.AddComponent<NumberElement>();
                break;
            case ElementContent.Trap:
                mapArray[x, y] = tempGo.AddComponent<TrapElement>();
                break;
            case ElementContent.Tool:
                mapArray[x, y] = tempGo.AddComponent<ToolElement>();
                break;
            case ElementContent.Gold:
                mapArray[x, y] = tempGo.AddComponent<GoldElement>();
                break;
            case ElementContent.Enemy:
                mapArray[x, y] = tempGo.AddComponent<EnemyElement>();
                break;
            case ElementContent.Door:
                mapArray[x, y] = tempGo.AddComponent<DoorElement>();
                break;
            case ElementContent.BigWall:
                mapArray[x, y] = tempGo.AddComponent<BigWallElement>();
                break;
            case ElementContent.SmallWall:
                mapArray[x, y] = tempGo.AddComponent<SmallWallElement>();
                break;
            case ElementContent.Exit:
                mapArray[x, y] = tempGo.AddComponent<ExitElement>();
                break;
        }
        return mapArray[x, y];
    }

    /// <summary>
    /// 将给定的一维索引值转换为二维索引值
    /// </summary>
    /// <param name="index">给定的一维索引值</param>
    /// <param name="x">转换后的二维索引值的x</param>
    /// <param name="y">转换后的二维索引值的y</param>
    private void GetPosition(int index, out int x, out int y)
    {
        y = index / w;
        x = index - y * w;
    }

    /// <summary>
    /// 将给定的二维索引值转换为一维索引值
    /// </summary>
    /// <param name="x">给定的二维索引值的x</param>
    /// <param name="y">给定的二维索引值的y</param>
    /// <returns>转换后的一维索引值</returns>
    private int GetIndex(int x, int y)
    {
        return w * y + x;
    }

    /// <summary>
    /// 计算指定位置元素的八领域中的陷阱个数
    /// </summary>
    /// <param name="x">元素所在位置的x</param>
    /// <param name="y">元素所在位置的y</param>
    /// <returns>陷阱个数</returns>
    public int CountAdjacentTraps(int x, int y)
    {
        int count = 0;
        if (IsSameContent(x, y + 1,ElementContent.Trap)) count++;
        if (IsSameContent(x, y - 1,ElementContent.Trap)) count++;
        if (IsSameContent(x - 1, y,ElementContent.Trap)) count++;
        if (IsSameContent(x + 1, y,ElementContent.Trap)) count++;
        if (IsSameContent(x - 1, y + 1,ElementContent.Trap)) count++;
        if (IsSameContent(x + 1, y + 1,ElementContent.Trap)) count++;
        if (IsSameContent(x - 1, y - 1,ElementContent.Trap)) count++;
        if (IsSameContent(x + 1, y - 1,ElementContent.Trap)) count++;
        return count;
    }

    /// <summary>
    /// 判断指定位置的元素类型
    /// </summary>
    /// <param name="x">元素所在位置的x</param>
    /// <param name="y">元素所在位置的x</param>
    /// <param name="content">需要比较的类型</param>
    /// <returns>比较结果</returns>
    public bool IsSameContent(int x,int y,ElementContent content)
    {
        if (x >= 0 && x < w && y >= 0 && y < h)
        {
            return mapArray[x, y].elementContent == content;
        }
        return false;
    }

    /// <summary>
    /// 泛洪算法翻开连片的空白区域
    /// </summary>
    /// <param name="x">开始泛洪的元素的位置的x</param>
    /// <param name="y">开始泛洪的元素的位置的y</param>
    /// <param name="visited">访问表</param>
    public void FloodFillElement(int x, int y, bool[,] visited)
    {
        if (x >= 0 && x < w && y >= 0 && y < h)
        {
            if (visited[x, y]) return;
            if (mapArray[x, y].elementType != ElementType.CantCovered)
            {
                ((SingleCoveredElement)mapArray[x, y]).UncoveredElementSingle();
            }
            if (CountAdjacentTraps(x, y) > 0) return;
            if (mapArray[x, y].elementType == ElementType.CantCovered) return;
            visited[x, y] = true;
            FloodFillElement(x - 1, y, visited);
            FloodFillElement(x + 1, y, visited);
            FloodFillElement(x, y - 1, visited);
            FloodFillElement(x, y + 1, visited);
            FloodFillElement(x - 1, y - 1, visited);
            FloodFillElement(x + 1, y + 1, visited);
            FloodFillElement(x + 1, y - 1, visited);
            FloodFillElement(x - 1, y + 1, visited);
        }
    }

    /// <summary>
    /// 快速翻开八领域的元素
    /// </summary>
    /// <param name="x">需要翻开的元素的位置的x</param>
    /// <param name="y">需要翻开的元素的位置的y</param>
    public void UncoveredAdjacentElements(int x,int y)
    {
        int marked = 0;
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && i < w && j >= 0 && j < h)
                {
                    if (mapArray[i, j].elementState == ElementState.Marked) marked++;
                    if (mapArray[i, j].elementState == ElementState.Uncovered && mapArray[i, j].elementContent == ElementContent.Trap) marked++;
                }
            }
        }
        if (CountAdjacentTraps(x, y) == marked)
        {
            ani.SetTrigger("QuickCheak");
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i >= 0 && i < w && j >= 0 && j < h)
                    {
                        if (mapArray[i, j].elementState != ElementState.Marked)
                        {
                            mapArray[i, j].OnPlayerStand();
                            mapArray[i, j].OnPlayerStand();
                        }
                    }
                }
            }
        }
        else
        {
            ani.SetTrigger("Why");
            AudioManager.Instance.PlayClip(AudioManager.Instance.why);
        }
    }

    /// <summary>
    /// 翻开地图内所有的陷阱
    /// </summary>
    public void DisplayAllTraps()
    {
        foreach (BaseElement element in mapArray)
        {
            if (element.elementContent == ElementContent.Trap)
            {
                ((TrapElement)element).UncoveredElementSingle();
            }
            if (element.elementContent != ElementContent.Trap && element.elementState == ElementState.Marked)
            {
                Instantiate(errorElement, element.transform);
            }
        }
    }
}
