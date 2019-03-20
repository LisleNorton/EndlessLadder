using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour {

	private Button btn_Start;
	private Button btn_Shop;
	private Button btn_Rank;
    private Button btn_Reset;
    private Button btn_Sound;
    private ManagerVars vars;
	
	private void Awake() {
        vars = ManagerVars.GetManagerVars();
        Init();
        EventCenter.AddListener(EventDefine.ShowMainPanel,ShowPanel);
    }
    private void OnDestroy()
    {
       EventCenter.RemoveListener(EventDefine.ShowMainPanel, ShowPanel);

    }
    private void Start()
    {
        
        if (GameData.IsAgainGame)
        {
            HidePanel();
            EventCenter.Broadcast(EventDefine.ShowGamePanel);
        }
    }

    private void Init() {
		btn_Start = transform.Find("btn_Start").GetComponent<Button>();
		btn_Start.onClick.AddListener(OnStartButtonClick);
		btn_Shop = transform.Find("Btns/btn_Shop").GetComponent<Button>();
		btn_Shop.onClick.AddListener(OnShopButtonClick);
		btn_Rank = transform.Find("Btns/btn_Rank").GetComponent<Button>();
		btn_Rank.onClick.AddListener(OnRankButtonClick);
		btn_Sound = transform.Find("Btns/btn_Sound").GetComponent<Button>();
		btn_Sound.onClick.AddListener(OnSoundButtonClick);
        btn_Reset = transform.Find("Btns/btn_Reset").GetComponent<Button>();
        btn_Reset.onClick.AddListener(OnResetButtonClick);
        ShowPanel();
    }

    private void ShowPanel()
    {
        gameObject.SetActive(true);
    }
    private void HidePanel()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 开始按钮点击
    /// </summary>
    private void OnStartButtonClick()
	{
        EventCenter.Broadcast(EventDefine.PlayClikAudio);
        GameManager.Instance.IsGameStarted = true;
        EventCenter.Broadcast(EventDefine.HideGameOverPanel);
		EventCenter.Broadcast(EventDefine.ShowGamePanel);
        HidePanel();

    }
	/// <summary>
	/// 商场按钮点击
	/// </summary>
	private void OnShopButtonClick(){
        EventCenter.Broadcast(EventDefine.PlayClikAudio);
        EventCenter.Broadcast(EventDefine.ShowShopPanel);
        HidePanel();
    }
	/// <summary>
	/// 排行榜按钮点击
	/// </summary>
	private void OnRankButtonClick(){
        EventCenter.Broadcast(EventDefine.PlayClikAudio);
        EventCenter.Broadcast(EventDefine.ShowRankPanel);
    }
    /// <summary>
	/// 重置按钮
	/// </summary>
	private void OnResetButtonClick(){
        EventCenter.Broadcast(EventDefine.PlayClikAudio);
        EventCenter.Broadcast(EventDefine.ShowResetPanel);
    }

	/// <summary>
	/// 音效按钮点击
	/// </summary>
	private void OnSoundButtonClick(){
        EventCenter.Broadcast(EventDefine.PlayClikAudio);
        GameManager.Instance.SetIsMusicOn(!GameManager.Instance.GetIsMusicOn());
        Sound();
    }

    private void Sound()
    {
        if (GameManager.Instance.GetIsMusicOn())
        {
            btn_Sound.transform.GetChild(0).GetComponent<Image>().sprite = vars.musicOn;
        }
        else
        {
            btn_Sound.transform.GetChild(0).GetComponent<Image>().sprite = vars.musicOff;
        }
        EventCenter.Broadcast(EventDefine.IsMusicOn, GameManager.Instance.GetIsMusicOn());
    }
}
