using UnityEngine;

public class DoubleCoveredElement : SingleCoveredElement
{
    public bool isHide = true;

    public override void Awake()
    {
        base.Awake();
        elementType = ElementType.DoubleCovered;
        if (Random.value < GameManager.Instance.uncoveredProbability)
        {
            UncoveredElementSingle();
        }
    }

    public override void OnPlayerStand()
    {
        switch (elementState)
        {
            case ElementState.Covered:
                if (isHide == true)
                {
                    UncoveredElementSingle();
                }
                else
                {
                    UncoveredElement();
                }
                break;
            case ElementState.Uncovered:
                return;
            case ElementState.Marked:
                if (isHide == true)
                {
                    RemoveFlag();
                }
                break;
        }
    }

    public override void OnMiddleMouseButton()
    {
        GameManager.Instance.UncoveredAdjacentElements(x, y);
    }

    public override void OnRightMouseButton()
    {
        switch (elementState)
        {
            case ElementState.Covered:
                if (isHide == true)
                {
                    AddFlag();
                }
                break;
            case ElementState.Uncovered:
                return;
            case ElementState.Marked:
                if (isHide == true)
                {
                    RemoveFlag();
                }
                break;
        }
    }

    public override void UncoveredElementSingle()
    {
        if (elementState == ElementState.Uncovered) return;
        isHide = false;
        RemoveFlag();
        ClearShadow();
        ConfirmSprite();
    }

    public override void OnUncovered()
    {
        elementState = ElementState.Uncovered;
        ToNumberElement(false);
    }

    /// <summary>
    /// 确认泥土下藏着的东西的图片
    /// </summary>
    public virtual void ConfirmSprite() { }
}
