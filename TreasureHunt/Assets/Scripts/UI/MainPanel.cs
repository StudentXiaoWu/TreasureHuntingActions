using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainPanel : MonoBehaviour 
{
    private static MainPanel _instance;
    public static MainPanel Instance
    {
        get { return _instance; }
    }

    public bool isHide = false;
    public Image armorIcon;
    public Image keyIcon;
    public Image arrowBg;
    public Image arrowIcon;
    public Image swordIcon;
    public Image hoeIcon;
    public Image hoeBag;
    public Image tntIcon;
    public Image tntBag;
    public Image mapIcon;
    public Image mapBag;
    public Image grassIcon;
    public Text levelText;
    public Text hpText;
    public Text armorText;
    public Text keyText;
    public Text weaponText;
    public Text hoeText;
    public Text tntText;
    public Text mapText;
    public Text goldText;
    public Toggle hoeToggle;
    public Toggle tntToggle;
    public Toggle mapToggle;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void OnMuteButtonClick()
    {
        AudioManager.Instance.SwitchMuteState();
    }

    public void OnBackButtonClick()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.button);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void OnNextButtonClick()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.button);
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    public void OnHoeSelected(bool isOn)
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.button);
        GameManager.Instance.hoeSelected.SetActive(isOn);
    }

    public void OnTntSelected(bool isOn)
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.button);
        GameManager.Instance.tntSelected.SetActive(isOn);
    }

    public void OnMapSelected(bool isOn)
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.button);
        GameManager.Instance.mapSelected.SetActive(isOn);
    }

    public void OnLevelButtonClick()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.button);
        if (isHide == false)
        {
            isHide = true;
            GetComponent<RectTransform>().DOAnchorPosY(-7, 0.5f);
        }
        else
        {
            isHide = false;
            GetComponent<RectTransform>().DOAnchorPosY(45, 0.5f);
        }
    }

    /// <summary>
    /// 更新UI显示
    /// </summary>
    /// <param name="rts">需要闪动的UI元素</param>
    public void UpdateUI(params RectTransform[] rts)
    {
        levelText.text = "Level " + GameManager.Instance.lv;
        hpText.text = GameManager.Instance.hp.ToString();
        if (GameManager.Instance.armor == 0)
        {
            armorIcon.gameObject.SetActive(false);
            armorText.gameObject.SetActive(false);
        }
        else
        {
            armorIcon.gameObject.SetActive(true);
            armorText.gameObject.SetActive(true);
            armorText.text = GameManager.Instance.armor.ToString();
        }
        if (GameManager.Instance.key == 0)
        {
            keyIcon.gameObject.SetActive(false);
            keyText.gameObject.SetActive(false);
        }
        else
        {
            keyIcon.gameObject.SetActive(true);
            keyText.gameObject.SetActive(true);
            keyText.text = GameManager.Instance.key.ToString();
        }
        switch (GameManager.Instance.weaponType)
        {
            case WeaponType.None:
                arrowBg.gameObject.SetActive(true);
                arrowIcon.gameObject.SetActive(false);
                swordIcon.gameObject.SetActive(false);
                weaponText.gameObject.SetActive(false);
                break;
            case WeaponType.Arrow:
                arrowBg.gameObject.SetActive(false);
                arrowIcon.gameObject.SetActive(true);
                swordIcon.gameObject.SetActive(false);
                weaponText.gameObject.SetActive(true);
                weaponText.text = GameManager.Instance.arrow.ToString();
                break;
            case WeaponType.Sword:
                arrowBg.gameObject.SetActive(false);
                arrowIcon.gameObject.SetActive(false);
                swordIcon.gameObject.SetActive(true);
                weaponText.gameObject.SetActive(false);
                break;
        }
        if (GameManager.Instance.hoe == 0)
        {
            hoeIcon.gameObject.SetActive(false);
            hoeText.gameObject.SetActive(false);
            hoeBag.gameObject.SetActive(false);
        }
        else
        {
            hoeIcon.gameObject.SetActive(true);
            hoeText.gameObject.SetActive(true);
            hoeBag.gameObject.SetActive(true);
            hoeText.text = GameManager.Instance.hoe.ToString();
        }
        if (GameManager.Instance.tnt == 0)
        {
            tntIcon.gameObject.SetActive(false);
            tntText.gameObject.SetActive(false);
            tntBag.gameObject.SetActive(false);
        }
        else
        {
            tntIcon.gameObject.SetActive(true);
            tntText.gameObject.SetActive(true);
            tntBag.gameObject.SetActive(true);
            tntText.text = GameManager.Instance.tnt.ToString();
        }
        if (GameManager.Instance.map == 0)
        {
            mapIcon.gameObject.SetActive(false);
            mapText.gameObject.SetActive(false);
            mapBag.gameObject.SetActive(false);
        }
        else
        {
            mapIcon.gameObject.SetActive(true);
            mapText.gameObject.SetActive(true);
            mapBag.gameObject.SetActive(true);
            mapText.text = GameManager.Instance.map.ToString();
        }
        grassIcon.gameObject.SetActive(GameManager.Instance.isGrass);
        goldText.text = GameManager.Instance.gold.ToString();
        foreach (RectTransform rt in rts)
        {
            rt.DOShakeScale(0.5f).onComplete += () =>{
                rt.localScale = new Vector3(1, 1, 1);
            };
        }
    }
}
