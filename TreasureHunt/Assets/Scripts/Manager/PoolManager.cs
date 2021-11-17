using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour 
{
    private static PoolManager _instance;
    public static PoolManager Instance
    {
        get { return _instance; }
    }

    Dictionary<EffectType, List<GameObject>> poolListDic = new Dictionary<EffectType, List<GameObject>>();
    List<GameObject> uncoveredEffectList = new List<GameObject>();
    List<GameObject> smokeEffectList = new List<GameObject>();
    List<GameObject> goldEffectList = new List<GameObject>();
    Dictionary<EffectType, int> poolListCapacityDic = new Dictionary<EffectType, int>();
    int uncoveredEffectListCapacity;
    int smokeEffectListCapacity = 5;
    int goldEffectListCapacity = 20;
    Dictionary<EffectType, GameObject> effectGoDic = new Dictionary<EffectType, GameObject>();

    private void Awake()
    {
        _instance = this;
        poolListDic.Add(EffectType.UncoveredEffect, uncoveredEffectList);
        poolListDic.Add(EffectType.SmokeEffect, smokeEffectList);
        poolListDic.Add(EffectType.GoldEffect, goldEffectList);
        uncoveredEffectListCapacity = (int)(GameManager.Instance.w * GameManager.Instance.h * 0.2f);
        poolListCapacityDic.Add(EffectType.UncoveredEffect, uncoveredEffectListCapacity);
        poolListCapacityDic.Add(EffectType.SmokeEffect, smokeEffectListCapacity);
        poolListCapacityDic.Add(EffectType.GoldEffect, goldEffectListCapacity);
        effectGoDic.Add(EffectType.UncoveredEffect, GameManager.Instance.uncoveredEffect);
        effectGoDic.Add(EffectType.SmokeEffect, GameManager.Instance.smokeEffect);
        effectGoDic.Add(EffectType.GoldEffect, GameManager.Instance.goldEffect);
    }

    public GameObject GetInstance(EffectType type, Transform t = null, bool worldPosStays = false)
    {
        List<GameObject> list;
        poolListDic.TryGetValue(type, out list);
        if (list.Count > 0)
        {
            GameObject tempGo = list[list.Count - 1];
            tempGo.SetActive(true);
            ResetInstance(tempGo);
            if (t != null)
            {
                tempGo.transform.SetParent(t, worldPosStays);
            }
            list.RemoveAt(list.Count - 1);
            return tempGo;
        }
        else
        {
            GameObject go;
            effectGoDic.TryGetValue(type, out go);
            if (t != null)
            {
                return Instantiate(go, t, worldPosStays);
            }
            return Instantiate(go);
        }
    }

    public void StoreInstance(EffectType type,GameObject go)
    {
        List<GameObject> list;
        poolListDic.TryGetValue(type, out list);
        int listCap;
        poolListCapacityDic.TryGetValue(type, out listCap);
        if (list.Count < listCap)
        {
            go.SetActive(false);
            list.Add(go);
        }
        else
        {
            Destroy(go);
        }
    }

    private void ResetInstance(GameObject go)
    {
        ParticleSystem ps = go.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Stop();
            ps.Play();
        }
        foreach (Transform t in go.transform)
        {
            ParticleSystem tps = go.GetComponent<ParticleSystem>();
            if (tps != null)
            {
                tps.Stop();
                tps.Play();
            }
        }
    }
}
