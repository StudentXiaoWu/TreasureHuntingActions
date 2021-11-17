using UnityEngine;

public class TrapElement : SingleCoveredElement
{
    public override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.Trap;
    }

    public override void UncoveredElementSingle()
    {
        if (elementState == ElementState.Uncovered) return;
        RemoveFlag();
        elementState = ElementState.Uncovered;
        ClearShadow();
        PoolManager.Instance.GetInstance(EffectType.UncoveredEffect, transform);
        LoadSprite(GameManager.Instance.trapSprites[Random.Range(0, GameManager.Instance.trapSprites.Length)]);
    }

    public override void OnUncovered()
    {
        GameManager.Instance.TakeDamage();
    }
}
