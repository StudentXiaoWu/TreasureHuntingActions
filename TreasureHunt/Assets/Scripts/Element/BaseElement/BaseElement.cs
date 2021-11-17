using UnityEngine;

public class BaseElement : MonoBehaviour 
{
    public int x;
    public int y;
    public ElementState elementState;
    public ElementType elementType;
    public ElementContent elementContent;

    public virtual void Awake()
    {
        x = (int)transform.position.x;
        y = (int)transform.position.y;
        name = "(" + x + "," + y + ")";
    }

    public virtual void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(2) && elementState == ElementState.Uncovered)
        {
            OnMiddleMouseButton();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            OnLeftMouseButton();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            OnRightMouseButton();
        }
    }

    /// <summary>
    /// 当角色站立在当前元素上时候执行的操作
    /// </summary>
    public virtual void OnPlayerStand() { }

    /// <summary>
    /// 当玩家左键点击当前元素时候执行的操作
    /// </summary>
    public virtual void OnLeftMouseButton()
    {
        GameManager.Instance.FindPath(new Point(x, y));
    }

    /// <summary>
    /// 当玩家中键点击当前元素时候执行的操作
    /// </summary>
    public virtual void OnMiddleMouseButton() { }

    /// <summary>
    /// 当玩家右键点击当前元素时候执行的操作
    /// </summary>
    public virtual void OnRightMouseButton() { }

    /// <summary>
    /// 切换当前元素的图片
    /// </summary>
    /// <param name="sprite">要切换到的图片</param>
    public void LoadSprite(Sprite sprite)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    /// <summary>
    /// 去除当前元素的阴影效果
    /// </summary>
    public void ClearShadow()
    {
        Transform shadow = transform.Find("shadow");
        if (shadow != null)
        {
            Destroy(shadow.gameObject);
        }
    }

    /// <summary>
    /// 将当前元素转化为数字元素并翻开
    /// </summary>
    /// <param name="needEffect">是否需要显示泥土特效</param>
    public void ToNumberElement(bool needEffect)
    {
        GameManager.Instance.mapArray[x, y] = gameObject.AddComponent<NumberElement>();
        ((NumberElement)GameManager.Instance.mapArray[x, y]).needEffect = needEffect;
        ((NumberElement)GameManager.Instance.mapArray[x, y]).UncoveredElement();
        Destroy(this);
    }
}
