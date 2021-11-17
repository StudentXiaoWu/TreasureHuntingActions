using UnityEngine;

public class DoorElement : CantCoveredElement 
{
    public override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.Door;
        LoadSprite(GameManager.Instance.doorSprite);
    }

    public override void OnLeftMouseButton()
    {
        if (Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < 1.5f)
        {
            if (GameManager.Instance.key > 0)
            {
                AudioManager.Instance.PlayClip(AudioManager.Instance.door);
                GameManager.Instance.key--;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.keyIcon.rectTransform, MainPanel.Instance.keyText.rectTransform);
                Instantiate(GameManager.Instance.doorOpenEffect, transform);
                ToNumberElement(true);
            }
            else
            {
                base.OnLeftMouseButton();
            }
        }
        else
        {
            base.OnLeftMouseButton();
        }
    }
}
