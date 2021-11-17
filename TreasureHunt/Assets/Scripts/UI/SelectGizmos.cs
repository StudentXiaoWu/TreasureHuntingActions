using UnityEngine;

public class SelectGizmos : MonoBehaviour 
{
    public ToolType toolType;

    private void OnMouseUp()
    {
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;
        switch (toolType)
        {
            case ToolType.Map:
                MainPanel.Instance.mapToggle.isOn = false;
                GameManager.Instance.map--;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.mapIcon.rectTransform,MainPanel.Instance.mapText.rectTransform);
                for (int i = x - 3; i <= x + 3; i++)
                {
                    for (int j = y - 3; j <= y + 3; j++)
                    {
                        if (i >= 0 && i < GameManager.Instance.w && j >= 0 && j < GameManager.Instance.h && GameManager.Instance.mapArray[i, j].elementContent != ElementContent.Exit)
                        {
                            if (GameManager.Instance.mapArray[i, j].elementContent == ElementContent.Trap && GameManager.Instance.mapArray[i, j].elementState != ElementState.Marked)
                            {
                                GameManager.Instance.mapArray[i, j].OnRightMouseButton();
                            }
                            if (GameManager.Instance.mapArray[i, j].elementContent != ElementContent.Trap && GameManager.Instance.mapArray[i, j].elementState == ElementState.Marked)
                            {
                                GameManager.Instance.mapArray[i, j].OnRightMouseButton();
                            }
                        }
                    }
                }
                AudioManager.Instance.PlayClip(AudioManager.Instance.map);
                break;
            case ToolType.Tnt:
                MainPanel.Instance.tntToggle.isOn = false;
                GameManager.Instance.tnt--;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.tntIcon.rectTransform, MainPanel.Instance.tntText.rectTransform);
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (i >= 0 && i < GameManager.Instance.w && j >= 0 && j < GameManager.Instance.h && GameManager.Instance.mapArray[i, j].elementContent != ElementContent.Exit)
                        {
                            if (GameManager.Instance.mapArray[i, j].elementType == ElementType.DoubleCovered)
                            {
                                ((DoubleCoveredElement)GameManager.Instance.mapArray[i, j]).UncoveredElementSingle();
                            }
                            else
                            {
                                GameManager.Instance.mapArray[i, j].ToNumberElement(true);
                            }
                        }
                    }
                }
                AudioManager.Instance.PlayClip(AudioManager.Instance.tnt);
                break;
            case ToolType.Hoe:
                MainPanel.Instance.hoeToggle.isOn = false;
                GameManager.Instance.hoe--;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.hoeIcon.rectTransform, MainPanel.Instance.hoeText.rectTransform);
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (i >= 0 && i < GameManager.Instance.w && j >= 0 && j < GameManager.Instance.h && GameManager.Instance.mapArray[i, j].elementContent != ElementContent.Exit)
                        {
                            if (GameManager.Instance.mapArray[i, j].elementType != ElementType.CantCovered)
                            {
                                ((SingleCoveredElement)GameManager.Instance.mapArray[i, j]).UncoveredElementSingle();
                            }
                            else
                            {
                                if (GameManager.Instance.mapArray[i, j].elementContent == ElementContent.SmallWall)
                                {
                                    GameManager.Instance.mapArray[i, j].ToNumberElement(true);
                                }
                            }
                        }
                    }
                }
                AudioManager.Instance.PlayClip(AudioManager.Instance.hoe);
                break;
            default:
                break;
        }
    }
}
