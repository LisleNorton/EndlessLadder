using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//编辑器拓展：管理容器。生成后防止继续生成，可注射掉
//[CreateAssetMenu(fileName = "ManagerVarsContainer", menuName = "CreateManagerVarsContainer", order = 0)]

public class ManagerVars : ScriptableObject
{
	
	/// <summary>
	/// 使用静态方法 获得到ManagerVarsContainer 文件
	/// </summary>
	public static ManagerVars GetManagerVars()
	{
		return Resources.Load<ManagerVars>("ManagerVarsContainer");//使用UnityEngine.Resources(需要创建Resources文件夹对应，不能是其他)
	}
	public List<Sprite> bgThemeSpriteList = new List<Sprite>();
    public List<Sprite> platformThemeSpriteList = new List<Sprite>();//normal/fire/ice/grass
    public List<Sprite> skinSpriteList = new List<Sprite>();    
    public List<Sprite> characterSkinSpriteList = new List<Sprite>();
    public List<string> skinNameList = new List<string>();
    /// <summary>
    /// 皮肤价格
    /// </summary>
    public List<int> skinPrice = new List<int>();

    public GameObject skinChooseItemPre;
    public GameObject normalPlatformPre;
    public List<GameObject> commonPlatformGroup = new List<GameObject>();//通用平台组合
    public List<GameObject> grassPlatformGroup = new List<GameObject>();
    public List<GameObject> winterPlatformGroup = new List<GameObject>();
    public GameObject spikePlatformLeft;
    public GameObject spikePlatformRight;// = new GameObject();
    public GameObject deathEffect;
    public GameObject diamondPre;
    public GameObject characterPre;//关联预制体Platform
	public float nextXPos = 0.554f, nextYPos = 0.645f;//平台生成位置偏移值
    public float nextFirstSpikePlatformXPos = 1.65f,nextSpikeformYPos = 0.645f;//钉子后续平台生成位置偏移

    public AudioClip jumpClip, fallClip, hitClip, diamondClip, buttonClip;
    public Sprite musicOn, musicOff;
}
