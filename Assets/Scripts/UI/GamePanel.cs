using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    private Button btn_Pause;
    private Button btn_Play;
    private Text text_Score;
    private Text text_DiamondCount;

    private void Awake() {
        Init();
        EventCenter.AddListener(EventDefine.ShowGamePanel, ShowPanel);
        EventCenter.AddListener(EventDefine.HideGamePanel, HidePanel);       
        EventCenter.AddListener<int>(EventDefine.UpdateScoreText, ShowScoreText);
        EventCenter.AddListener<int>(EventDefine.UpdateDiamond, ShowDiamond);

    }
    private void Init()
    {
        btn_Pause = transform.Find("btn_Pause").GetComponent<Button>();
        btn_Pause.onClick.AddListener(OnPauseButtonClick);
        btn_Play = transform.Find("btn_Play").GetComponent<Button>();
        btn_Play.onClick.AddListener(OnPlayButtonClick);
        text_Score = transform.Find("text_Score").GetComponent<Text>();
        text_DiamondCount = transform.Find("Diamond/text_DiamondCount").GetComponent<Text>();
        btn_Pause.gameObject.SetActive(true);
        btn_Play.gameObject.SetActive(false);
        HidePanel();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowGamePanel, ShowPanel);//事件中心移除监听器ShowGamePanel，Show方法
        EventCenter.RemoveListener(EventDefine.HideGamePanel, HidePanel);//隐藏游戏界面
        EventCenter.RemoveListener<int>(EventDefine.UpdateScoreText, ShowScoreText);
        EventCenter.RemoveListener<int>(EventDefine.UpdateDiamond, ShowDiamond);
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
    /// 更新成绩显示
    /// </summary>
    private void ShowScoreText(int score)
    {
        text_Score.text = score.ToString();
    }
    /// <summary>
    /// 更新砖石显示
    /// </summary>
    private void ShowDiamond(int diamondCount)
    {
        text_DiamondCount.text = diamondCount.ToString();
    }
    /// <summary>
    /// 暂停按钮点击
    /// </summary>
    private void OnPauseButtonClick()
    {
        btn_Pause.gameObject.SetActive(false);
        btn_Play.gameObject.SetActive(true);
        //游戏暂停GetMouseButtonDown(0)
        GameManager.Instance.IsGamePause = true;

        Time.timeScale = 0;
    }
    /// <summary>
    /// 开始按钮点击
    /// </summary>
    private void OnPlayButtonClick()
    {
        btn_Play.gameObject.SetActive(false);
        btn_Pause.gameObject.SetActive(true);
        //继续游戏
        GameManager.Instance.IsGamePause = false;
        Time.timeScale = 1;
    }

}
