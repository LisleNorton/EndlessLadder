using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{

    public Text txt_Score;
    public Text txt_BestScore;
    public Text txt_AddDiamondCount;
    public Button btn_Restart,btn_Home,btn_Rank;
    public Image img_New;

    private void Awake()
    {
        btn_Restart.onClick.AddListener(OnRestartButtonclick);
        btn_Rank.onClick.AddListener(OnRankButtonclick);
        btn_Home.onClick.AddListener(OnHomeButtonclick);
        EventCenter.AddListener(EventDefine.ShowGameOverPanel, ShowPanel);
        EventCenter.AddListener(EventDefine.HideGameOverPanel,HidePanel);
        HidePanel();

    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowGameOverPanel, ShowPanel);
        EventCenter.RemoveListener(EventDefine.HideGameOverPanel, HidePanel);
    }

    private void ShowPanel()
    {


        if (GameManager.Instance.GetGameScore() > GameManager.Instance.GetBestScore())
        {
            img_New.gameObject.SetActive(true);
            txt_BestScore.text = "最高分  " + GameManager.Instance.GetGameScore();
        }
        else
        {
            img_New.gameObject.SetActive(false);
            txt_BestScore.text = "最高分  " + GameManager.Instance.GetBestScore();
        }
        txt_Score.text = GameManager.Instance.GetGameScore().ToString();
        GameManager.Instance.SaveScore(GameManager.Instance.GetGameScore());
        txt_AddDiamondCount.text = "+" + GameManager.Instance.GetGameDiamond().ToString();
        GameManager.Instance.UpdateAllDiamond(GameManager.Instance.GetGameDiamond());
        gameObject.SetActive(true);
    }
    private void HidePanel()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 点击再来一局
    /// </summary>
    private void OnRestartButtonclick()
    {
        EventCenter.Broadcast(EventDefine.PlayClikAudio);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameData.IsAgainGame = true;
    }
    /// <summary>
    /// 点击排行榜
    /// </summary>
    private void OnRankButtonclick()
    {
        EventCenter.Broadcast(EventDefine.PlayClikAudio);
        EventCenter.Broadcast(EventDefine.ShowRankPanel);
    }
    /// <summary>
    /// 点击返回主界面
    /// 重新加载场景
    /// 获取当前场景名称并激活
    /// </summary>
    private void OnHomeButtonclick()
    {
        EventCenter.Broadcast(EventDefine.PlayClikAudio);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameData.IsAgainGame = false;
    }
}
