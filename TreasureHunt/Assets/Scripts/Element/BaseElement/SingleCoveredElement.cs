using UnityEngine;
using DG.Tweening;

public class SingleCoveredElement : BaseElement 
{
    public override void Awake()
    {
        base.Awake();
        elementType = ElementType.SingleCovered;
        elementState = ElementState.Covered;
        LoadSprite(GameManager.Instance.coverTileSprites[Random.Range(0, GameManager.Instance.coverTileSprites.Length)]);
    }

    public override void OnPlayerStand()
    {
        switch (elementState)
        {
            case ElementState.Covered:
                UncoveredElement();
                break;
            case ElementState.Uncovered:
                return;
            case ElementState.Marked:
                RemoveFlag();
                break;
        }
    }

    public override void OnRightMouseButton()
    {
        switch (elementState)
        {
            case ElementState.Covered:
                AddFlag();
                break;
            case ElementState.Uncovered:
                return;
            case ElementState.Marked:
                RemoveFlag();
                break;
        }
    }

    /// <summary>
    /// 翻开当前元素的操作
    /// </summary>
    public virtual void UncoveredElement()
    {
        if (elementState == ElementState.Uncovered) return;
        AudioManager.Instance.PlayClip(AudioManager.Instance.dig);
        UncoveredElementSingle();
        OnUncovered();
    }

    /// <summary>
    /// 仅仅翻开当前元素自身
    /// </summary>
    public virtual void UncoveredElementSingle() { }

    /// <summary>
    /// 翻开自身后要做的操作
    /// </summary>
    public virtual void OnUncovered() { }

    /// <summary>
    /// 将元素标记为陷阱
    /// </summary>
    public void AddFlag()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.flag);
        elementState = ElementState.Marked;
        GameObject flag = Instantiate(GameManager.Instance.flagElement, transform);
        flag.name = "FlagElement";
        flag.transform.DOLocalMoveY(0, 0.1f);
        PoolManager.Instance.GetInstance(EffectType.SmokeEffect, transform);
    }

    /// <summary>
    /// 解除元素的标记
    /// </summary>
    public void RemoveFlag()
    {
        Transform flag = transform.Find("FlagElement");
        if (flag != null)
        {
            elementState = ElementState.Covered;
            flag.DOLocalMoveY(0.15f, 0.1f).onComplete += () =>
            {
                Destroy(flag.gameObject);
            };
        }
    }
}
