using UnityEngine;

public class ToolElement : DoubleCoveredElement
{
    public ToolType toolType;

    public override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.Tool;
    }

    public override void OnUncovered()
    {
        GetTool();
        base.OnUncovered();
    }

    private void GetTool()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.pick);
        switch (toolType)
        {
            case ToolType.Hp:
                GameManager.Instance.hp++;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.hpText.rectTransform);
                break;
            case ToolType.Armor:
                GameManager.Instance.armor++;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.armorIcon.rectTransform, MainPanel.Instance.armorText.rectTransform);
                break;
            case ToolType.Sword:
                GameManager.Instance.weaponType = WeaponType.Sword;
                GameManager.Instance.arrow = 0;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.swordIcon.rectTransform);
                break;
            case ToolType.Map:
                GameManager.Instance.map++;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.mapIcon.rectTransform, MainPanel.Instance.mapText.rectTransform);
                break;
            case ToolType.Arrow:
                GameManager.Instance.weaponType = WeaponType.Arrow;
                GameManager.Instance.arrow++;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.arrowIcon.rectTransform, MainPanel.Instance.weaponText.rectTransform);
                break;
            case ToolType.Key:
                GameManager.Instance.key++;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.keyIcon.rectTransform, MainPanel.Instance.keyText.rectTransform);
                break;
            case ToolType.Tnt:
                GameManager.Instance.tnt++;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.tntIcon.rectTransform, MainPanel.Instance.tntText.rectTransform);
                break;
            case ToolType.Hoe:
                GameManager.Instance.hoe++;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.hoeIcon.rectTransform, MainPanel.Instance.hoeText.rectTransform);
                break;
            case ToolType.Grass:
                GameManager.Instance.isGrass = true;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.grassIcon.rectTransform);
                break;
        }
    }

    public override void ConfirmSprite()
    {
        LoadSprite(GameManager.Instance.toolSprites[(int)toolType]);
    }
}
