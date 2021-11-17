using UnityEngine;

public class GoldElement : DoubleCoveredElement 
{
    public GoldType goldType;

    public override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.Gold;
    }

    public override void OnUncovered()
    {
        Transform goldEffect = transform.Find("GoldEffect");
        if (goldEffect != null)
        {
            PoolManager.Instance.StoreInstance(EffectType.GoldEffect, goldEffect.gameObject);
        }
        GetGold();
        base.OnUncovered();
    }

    private void GetGold()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.pick);
        int x = 1;
        if (GameManager.Instance.isGrass == true) x = 2;
        switch (goldType)
        {
            case GoldType.One:
                GameManager.Instance.gold += 30 * x;
                break;
            case GoldType.Two:
                GameManager.Instance.gold += 60 * x;
                break;
            case GoldType.Three:
                GameManager.Instance.gold += 100 * x;
                break;
            case GoldType.Four:
                GameManager.Instance.gold += 150 * x;
                break;
            case GoldType.Five:
                GameManager.Instance.gold += 450 * x;
                break;
            case GoldType.Six:
                GameManager.Instance.gold += 600 * x;
                break;
            case GoldType.Seven:
                GameManager.Instance.gold += 1000 * x;
                break;
        }
        MainPanel.Instance.UpdateUI(MainPanel.Instance.goldText.rectTransform);
    }

    public override void ConfirmSprite()
    {
        Transform goldEffect = transform.Find("GoldEffect");
        if (goldEffect == null)
        {
            PoolManager.Instance.GetInstance(EffectType.GoldEffect, transform).name = "GoldEffect";
        }
        LoadSprite(GameManager.Instance.goldSprites[(int)goldType]);
    }
}
