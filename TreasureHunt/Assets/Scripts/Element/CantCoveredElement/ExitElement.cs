using UnityEngine;

public class ExitElement : CantCoveredElement 
{
    public override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.Exit;
        ClearShadow();
        name = "Exit";
        LoadSprite(GameManager.Instance.exitSprite);
    }

    public override void OnPlayerStand()
    {
        GameManager.Instance.OnLevelPass();
    }
}
