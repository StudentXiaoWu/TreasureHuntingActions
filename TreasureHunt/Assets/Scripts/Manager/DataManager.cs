using UnityEngine;

public class DataManager : MonoBehaviour 
{
    private static DataManager _instance;
    public static DataManager Instance
    {
        get { return _instance; }
    }

    public bool isMute;

    public int w;
    public int h;

    public int lv;
    public int hp;
    public int armor;
    public int key;
    public int hoe;
    public int tnt;
    public int map;
    public int gold;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        LoadData();
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void LoadData()
    {
        isMute = PlayerPrefs.GetInt("mute", 0) == 0 ? false : true;
        lv = PlayerPrefs.GetInt("lv", 1);
        hp = PlayerPrefs.GetInt("hp", 3);
        armor = PlayerPrefs.GetInt("armor", 0);
        key = PlayerPrefs.GetInt("key", 0);
        hoe = PlayerPrefs.GetInt("hoe", 0);
        tnt = PlayerPrefs.GetInt("tnt", 0);
        map = PlayerPrefs.GetInt("map", 0);
        gold = PlayerPrefs.GetInt("gold", 0);
        w = 20 + (lv * 3);
        h = Random.Range(9, 12);
    }

    public void SetMuteState(bool isMute)
    {
        this.isMute = isMute;
        PlayerPrefs.SetInt("mute", isMute == false ? 0 : 1);
    }

    public void ResetData()
    {
        SaveData(1, 3, 0, 0, 0, 0, 0, 0);
    }

    public void SaveData(int lv, int hp, int armor, int key, int hoe, int tnt, int map, int gold)
    {
        this.lv = lv;
        this.hp = hp;
        this.armor = armor;
        this.key = key;
        this.hoe = hoe;
        this.tnt = tnt;
        this.map = map;
        this.gold = gold;
        PlayerPrefs.SetInt("lv", lv);
        PlayerPrefs.SetInt("hp", hp);
        PlayerPrefs.SetInt("armor", armor);
        PlayerPrefs.SetInt("key", key);
        PlayerPrefs.SetInt("hoe", hoe);
        PlayerPrefs.SetInt("tnt", tnt);
        PlayerPrefs.SetInt("map", map);
        PlayerPrefs.SetInt("gold", gold);
    }
}
