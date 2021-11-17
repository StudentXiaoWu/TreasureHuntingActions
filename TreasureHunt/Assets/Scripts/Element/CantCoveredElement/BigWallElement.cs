using UnityEngine;

public class BigWallElement : CantCoveredElement 
{
    public override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.BigWall;
        LoadSprite(GameManager.Instance.bigwallSprites[Random.Range(0, GameManager.Instance.bigwallSprites.Length)]);
    }
}
