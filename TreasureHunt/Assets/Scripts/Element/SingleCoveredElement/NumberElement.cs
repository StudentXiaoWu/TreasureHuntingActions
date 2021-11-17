using UnityEngine;

public class NumberElement : SingleCoveredElement 
{
    public bool needEffect = true;

    public override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.Number;
    }

    public override void OnMiddleMouseButton()
    {
        if ((int)GameManager.Instance.player.transform.position.x == x && (int)GameManager.Instance.player.transform.position.y == y)
        {
            GameManager.Instance.UncoveredAdjacentElements(x, y);
        }
        else
        {
            OnLeftMouseButton();
        }
    }

    public override void UncoveredElementSingle()
    {
        if (elementState == ElementState.Uncovered) return;
        RemoveFlag();
        elementState = ElementState.Uncovered;
        ClearShadow();
        if (needEffect == true)
        {
            PoolManager.Instance.GetInstance(EffectType.UncoveredEffect, transform);
        }
        LoadSprite(GameManager.Instance.numberSprites[GameManager.Instance.CountAdjacentTraps(x, y)]);
    }

    public override void OnUncovered()
    {
        GameManager.Instance.FloodFillElement(x, y, new bool[GameManager.Instance.w, GameManager.Instance.h]);
    }
}
