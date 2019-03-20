using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShopPanel : MonoBehaviour {

    private ManagerVars vars;
    private Transform parent;
    private int selectIndex;
    private Text txt_SkinName, txt_Diamond;
    private Button btn_Back, btn_Select,btn_Buy;

    private void Awake()
    {
        //UI组件
        parent = transform.Find("ScrolRect/Parent");
        txt_SkinName = transform.Find("txt_SkinName").GetComponent<Text>();
        txt_Diamond = transform.Find("Diamond/txt_DiamondCount").GetComponent<Text>();
        btn_Back = transform.Find("btn_Back").GetComponent<Button>();
        btn_Back.onClick.AddListener(OnBackButtonClick);
        btn_Select = transform.Find("btn_Select").GetComponent<Button>();
        btn_Select.onClick.AddListener(OnSelectButtonClick);
        btn_Buy = transform.Find("btn_Buy").GetComponent<Button>();
        btn_Buy.onClick.AddListener(OnBuyButtonClick);
        vars = ManagerVars.GetManagerVars();
        //事件监听
        EventCenter.AddListener(EventDefine.ShowShopPanel,ShowPanel);

        HidePanel();

    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowShopPanel, ShowPanel);
    }

    private void Start()
    {
        InitScrolRect();

    }
    private void Update()
    {
        //按parent居左的锚点与父物体 的内部位置计算皮肤编号，0/1/2/3
        selectIndex = (int)Mathf.Round(parent.transform.localPosition.x / -160.0f);       
        //滑动一次
        if (Input.GetMouseButtonUp(0))
        {
            parent.transform.DOLocalMoveX(selectIndex * -160, 0.2f);
            //parent.transform.localPosition = new Vector3(currentIndex * -160, 0);
        }
        SetItemSize(selectIndex);
        RefreshUI(selectIndex);

    }
    /// <summary>
    /// 界面状态
    /// </summary>
    private void ShowPanel()
    {
        gameObject.SetActive(true);
    }
    private void HidePanel()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 返回按钮点击
    /// </summary>
    private void OnBackButtonClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClikAudio);
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        HidePanel();
    }
    /// <summary>
    /// 购买按钮点击
    /// </summary>
    private void OnBuyButtonClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClikAudio);

        int price = int.Parse(btn_Buy.GetComponentInChildren<Text>().text);
        if (price > GameManager.Instance.GetAllDiamond())
        {
            EventCenter.Broadcast(EventDefine.Hint, "钻石不足");
            Debug.Log("钻石不足，不能购买");
            return;
        }
        GameManager.Instance.UpdateAllDiamond(-price);
        GameManager.Instance.SetSkinUnloacked(selectIndex);
        parent.GetChild(selectIndex).GetChild(0).GetComponent<Image>().color = Color.white;
    }
    /// <summary>
    /// 选择按钮点击
    /// </summary>
    private void OnSelectButtonClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClikAudio);

        EventCenter.Broadcast(EventDefine.ChangeSkin, selectIndex);//PlayerController中修改
        GameManager.Instance.SetSelectedSkin(selectIndex);//GameData修改数据
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        HidePanel();
    }
    /// <summary>
    /// 初始化滑动栏
    /// </summary>
    private void InitScrolRect()
    {
        //滑动栏的宽长设置
        parent.GetComponent<RectTransform>().sizeDelta = new Vector2((vars.skinSpriteList.Count + 2) * 160, 300);
        for (int i = 0; i < vars.skinSpriteList.Count; i++)
        {
            //在父物体下实例化物体
            GameObject go = Instantiate(vars.skinChooseItemPre, parent);
            //未解锁
            if (GameManager.Instance.GetSkinUnlocked(i) == false)
            {
                go.GetComponentInChildren<Image>().color = Color.gray;
            }
            else//解锁了
            {
                go.GetComponentInChildren<Image>().color = Color.white;
            }
            go.GetComponentInChildren<Image>().sprite = vars.skinSpriteList[i];
            go.transform.localPosition = new Vector3((i + 1) * 160, 0, 0);
        }
        parent.transform.localPosition =
            new Vector3(GameManager.Instance.GetCurrentSelectedSkin() * -160, 0);
    }
    /// <summary>
    /// 皮肤滑动居中变大效果
    /// 传递的编号与parent子物体数组号对应0/1/2/3
    /// 该子物体变大
    /// </summary>
    private void SetItemSize(int selectIndex)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if (selectIndex == i)
            {
                parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(160, 160);
            }
            else
            {
                parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
            }
        }
    }
    /// <summary>
    /// 商品页面信息
    /// </summary>
    private void RefreshUI(int selectIndex)
    {
        txt_SkinName.text = vars.skinNameList[selectIndex];
        txt_Diamond.text = GameManager.Instance.GetAllDiamond().ToString();
        //未解锁
        if (GameManager.Instance.GetSkinUnlocked(selectIndex) == false)
        {
            btn_Select.gameObject.SetActive(false);
            btn_Buy.gameObject.SetActive(true);
            btn_Buy.GetComponentInChildren<Text>().text = vars.skinPrice[selectIndex].ToString();
        }
        else
        {
            btn_Select.gameObject.SetActive(true);
            btn_Buy.gameObject.SetActive(false);
        }
    }

}
