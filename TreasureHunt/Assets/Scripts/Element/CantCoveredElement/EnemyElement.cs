using UnityEngine;

public class EnemyElement : CantCoveredElement 
{
    public override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.Enemy;
        ClearShadow();
        LoadSprite(GameManager.Instance.enemySprites[Random.Range(0, GameManager.Instance.enemySprites.Length)]);
    }

    public override void OnLeftMouseButton()
    {
        if (Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < 1.5f)
        {
            switch (GameManager.Instance.weaponType)
            {
                case WeaponType.None:
                    base.OnLeftMouseButton();
                    break;
                case WeaponType.Arrow:
                    AudioManager.Instance.PlayClip(AudioManager.Instance.enemy);
                    GameManager.Instance.armor--;
                    if (GameManager.Instance.armor == 0)
                    {
                        GameManager.Instance.weaponType = WeaponType.None;
                    }
                    MainPanel.Instance.UpdateUI(MainPanel.Instance.arrowIcon.rectTransform, MainPanel.Instance.weaponText.rectTransform);
                    ToNumberElement(true);
                    break;
                case WeaponType.Sword:
                    AudioManager.Instance.PlayClip(AudioManager.Instance.enemy);
                    MainPanel.Instance.UpdateUI(MainPanel.Instance.swordIcon.rectTransform);
                    ToNumberElement(true);
                    break;
            }
        }
        else
        {
            base.OnLeftMouseButton();
        }
    }
}
