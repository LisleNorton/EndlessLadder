using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameData data;
    public  ManagerVars vars;



    /// <summary>
    /// 游戏状态：IsGameStarted,IsGameOver
    /// </summary>
    public bool IsGameStarted { get; set; }
    public bool IsGameOver { get; set; }
    public bool IsGamePause { get; set; }
    /// <summary>
    /// 玩家开始移动
    /// </summary>
    public bool PlayerIsMove { get; set; }
    /// <summary>
    /// 游戏成绩
    /// </summary>
    private int gameScore;
    /// <summary>
    /// 单局游戏砖石个数
    /// </summary>
    private int gameDiamondCount;
    /// <summary>
    /// 个人游戏砖石个数
    /// </summary>
    private int allDiamondCount;

    private bool isMusicOn;
    private bool isFirstGame;
    private int[] bestScoreArr;
    private int selectSkin;
    private bool[] skinUnlocked;


    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        Instance = this;
        EventCenter.AddListener(EventDefine.PlayerMove, PlayerMove);
        //计分监听
        EventCenter.AddListener(EventDefine.AddScore, AddGameScore);
        EventCenter.AddListener(EventDefine.AddDiamond, AddGameDiamond);

        if (GameData.IsAgainGame)
        {
            IsGameStarted = true;
        }

        InitGameData();
    }
    private void Start()
    {
        
    }
    private void OnDestroy()//移除监听
    {
        EventCenter.RemoveListener(EventDefine.AddScore,AddGameScore);
        EventCenter.RemoveListener(EventDefine.PlayerMove, PlayerMove);
        EventCenter.RemoveListener(EventDefine.AddDiamond, AddGameDiamond);
    }
    /// <summary>
    /// 增加游戏成绩
    /// </summary>
    private void AddGameScore()
    {
        if (IsGameStarted == false || IsGameOver || IsGamePause) return;
        gameScore++;
        EventCenter.Broadcast(EventDefine.UpdateScoreText,gameScore);
    }
    /// <summary>
    /// 获取分数，用于平台掉落控制
    /// </summary>
    public int GetGameScore()
    {
        return gameScore;
    }

    /// <summary>
    /// 保存成绩
    /// </summary>
    public void SaveScore(int score)//60
    {
        List<int> list = bestScoreArr.ToList();
        //Array转List,从大到小排序list传给本地Array
        list.Sort((x, y) => (-x.CompareTo(y)));
        bestScoreArr = list.ToArray();

        //50 20 10 Array直插法
        int index = -1;
        for (int i = 0; i < bestScoreArr.Length; i++)
        {
            if (score > bestScoreArr[i])
            {
                index = i;
            }
        }
        if (index == -1) return;
        //
        for (int i = bestScoreArr.Length - 1; i > index; i--)
        {
            bestScoreArr[i] = bestScoreArr[i - 1];
        }
        bestScoreArr[index] = score;

        Save();
    }
    /// <summary>
    /// 提取历史最高分
    /// </summary>
    /// <returns></returns>
    public int GetBestScore()
    {
        return bestScoreArr.Max();
    }
    /// <summary>
    /// 获得最高分数组
    /// </summary>
    /// <returns></returns>
    public int[] GetScoreArr()
    {
        List<int> list = bestScoreArr.ToList();
        //从大到小排序list
        list.Sort((x, y) => (-x.CompareTo(y)));
        bestScoreArr = list.ToArray();

        return bestScoreArr;
    }

    private void PlayerMove()
    {
        PlayerIsMove = true;
    }
    /// <summary>
    /// 获取当前皮肤是否解锁
    /// </summary>
    /// <returns></returns>
    public bool GetSkinUnlocked(int index)
    {
        Debug.Log(index);
        //List<bool> list = skinUnlocked.ToList();
        //skinUnlocked
        return skinUnlocked[index];
    }
    /// <summary>
    /// 设置当前皮肤解锁
    /// </summary>
    public void SetSkinUnloacked(int index)
    {
        skinUnlocked[index] = true;
        Save();
    }
    /// <summary>
    /// 获取所有得钻石数量
    /// </summary>
    /// <returns></returns>
    public int GetAllDiamond()
    {
        return allDiamondCount;
    }
    /// <summary>
    /// 更新总钻石数量
    /// </summary>
    public void UpdateAllDiamond(int value)
    {
        allDiamondCount += value;
        Save();
    }
    /// <summary>
    /// 设置当前选择的皮肤下标
    /// </summary>
    public void SetSelectedSkin(int index)
    {
        selectSkin = index;
        Save();
    }
    /// <summary>
    /// 获得当前选择的皮肤
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSelectedSkin()
    {
        return selectSkin;
    }
    /// <summary>
    /// 设置音效是否开启
    /// </summary>
    public void SetIsMusicOn(bool value)
    {
        isMusicOn = value;
        Save();
    }
    /// <summary>
    /// 获取音效是否开启
    /// </summary>
    /// <returns></returns>
    public bool GetIsMusicOn()
    {
        return isMusicOn;
    }

    /// <summary>
    /// 增加砖石
    /// </summary>
    private void AddGameDiamond()
    {
        if (IsGameStarted == false || IsGameOver || IsGamePause) return;
        gameDiamondCount++;
        EventCenter.Broadcast(EventDefine.UpdateDiamond, gameDiamondCount);
    }

    /// <summary>
    /// 获取单局分数
    /// </summary>
    public int GetGameDiamond()
    {
        allDiamondCount += gameDiamondCount;
        return gameDiamondCount;
    }
    /// <summary>
    /// 初始化游戏数据
    /// </summary>
    private void InitGameData()
    {
        Read();
        if (data != null)
        {
            isFirstGame = data.GetIsFirstGame();
        }
        else
        {
            isFirstGame = true;
        }
        //如果第一次开始游戏
        if (isFirstGame)
        {
            isFirstGame = false;
            isMusicOn = true;
            bestScoreArr = new int[3];
            selectSkin = 0;
            skinUnlocked = new bool[vars.skinSpriteList.Count];
            skinUnlocked[0] = true;
            allDiamondCount = 10;

            data = new GameData();
            Save();
        }
        else
        {
            isMusicOn = data.GetIsMusicOn();
            bestScoreArr = data.GetBestScoreArr();
            selectSkin = data.GetSelectSkin();
            skinUnlocked = data.GetSkinUnlocked();
            allDiamondCount = data.GetDiamondCount();
        }
    }
    /// <summary>
    /// 储存数据
    /// </summary>
    private void Save()
    {
        try
        {   
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.Create(Application.persistentDataPath + "/GameData.data"))
            {
                data.SetBestScoreArr(bestScoreArr);
                data.SetDiamondCount(allDiamondCount);
                data.SetIsFirstGame(isFirstGame);
                data.SetIsMusicOn(isMusicOn);
                data.SetSelectSkin(selectSkin);
                data.SetSkinUnlocked(skinUnlocked);

                bf.Serialize(fs, data);//序列化写入本地
            }

        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    /// <summary>
    /// 读取
    /// </summary>
    private void Read()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.Open(Application.persistentDataPath + "/GameData.data", FileMode.Open))
            {
                data = (GameData)bf.Deserialize(fs);
            }
        }
        catch (System.Exception e)
        {

            Debug.Log(e.Message);
        }
    }
    /// <summary>
    /// 重置数据
    /// </summary>
    public void ResetData()
    {
        isFirstGame = false;
        isMusicOn = true;
        bestScoreArr = new int[3];
        selectSkin = 0;
        skinUnlocked = new bool[vars.platformThemeSpriteList.Count];
        skinUnlocked[0] = true;
        allDiamondCount = 10;

        Save();
    }

}
