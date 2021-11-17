using UnityEngine;

public class CantCoveredElement : BaseElement
{
    public override void Awake()
    {
        base.Awake();
        elementType = ElementType.CantCovered;
        elementState = ElementState.Uncovered;
    }
}
