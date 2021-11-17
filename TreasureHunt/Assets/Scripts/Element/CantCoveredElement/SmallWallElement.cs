using UnityEngine;

public class SmallWallElement : CantCoveredElement 
{
    public override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.SmallWall;
        ClearShadow();
        LoadSprite(GameManager.Instance.smallwallSprites[Random.Range(0, GameManager.Instance.smallwallSprites.Length)]);
    }
}
