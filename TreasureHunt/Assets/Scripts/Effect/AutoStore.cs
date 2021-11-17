using UnityEngine;

public class AutoStore : MonoBehaviour 
{
    public EffectType effectType;
    public float delay;
    public bool needStore = true;

    private void OnEnable()
    {
        if (needStore)
        {
            Invoke("Delay", delay);
        }
        else
        {
            Destroy(gameObject, delay);
        }
    }

    private void Delay()
    {
        PoolManager.Instance.StoreInstance(effectType, gameObject);
    }
}
