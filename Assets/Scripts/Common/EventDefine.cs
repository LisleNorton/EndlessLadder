public enum EventDefine
{
    DecidePath,
    //计分里程碑
    AddScore,
    UpdateScoreText,

    PlayerMove,
    //砖石计分
    AddDiamond,
    UpdateDiamond,
    //页面转换
    ShowMainPanel,    //主页面
    HideMainPanel,
    ShowGamePanel,    //游戏页面
    HideGamePanel,
    ShowGameOverPanel,//结束页面
    HideGameOverPanel,
    ShowShopPanel,//商场页面
    HideShopPanel,
    ShowRankPanel,//排行榜页面
    HideRankPanel,
    ShowResetPanel, //重置页面
    HideResetPanel,
    //修改平台生成器中人物皮肤
    ChangeSkin,
    //消息提示
    Hint,
    //点击音乐
    PlayClikAudio,
    //是否音乐
    IsMusicOn
}
