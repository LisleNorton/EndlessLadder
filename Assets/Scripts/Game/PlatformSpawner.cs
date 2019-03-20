using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformGroupType
{
    Grass,
    Winter
}
public class PlatformSpawner : MonoBehaviour {

	
	/// <summary>----------Platform
	/// 生成平台数量
	/// </summary>
	private int spawnPlatformCount = 5;
    public Vector3 startSpawnPos;//(0,-2.4,0)
    private ManagerVars vars;
	/// <summary>
	/// 平台生成方向
	/// </summary>
	private Vector3 platformSpawnPosition;
	/// <summary>
	/// 平台生成方向是否为左
	/// </summary>
	private bool isLeftSpawn = false;

    /// <summary>----------Spike
    /// 生成钉子后续平台数量
    /// </summary>
    private int spawnSpikeNextPlatformCount = 3;
    /// <summary>
    /// 钉子组合平台生成平台方向
    /// -1为left ; 1 为Right;0为未赋值
    /// </summary>
    private int spikeSpawnDir = 0;
    /// <summary>
    /// 钉子后续平台生成位置
    /// </summary>
    private Vector3 spikeNextPlatformPos;

    /// <summary>----------Sprite/Type
    /// 选择的平台生成的图片
    /// </summary>
    private Sprite selectPlatformSprite;
    /// <summary>
    /// 组合平台的类型
    /// </summary>
    private PlatformGroupType groupType;

    /// <summary>----------Fall & Mile
    /// 里程碑数目
    /// </summary>
    public int milestoneCount = 10;
    /// <summary>
    /// 掉落时间、最小掉落时间
    /// 时间系数，不同里程碑不同速度
    /// </summary>
    public float fallTime;
    public float minFallTime;
    public float multiple;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.DecidePath, DecidePath);
        vars = ManagerVars.GetManagerVars();//得到管理器容器
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.DecidePath,DecidePath);
    }

    private void Start()
	{
        RandomPlatformTheme();
        platformSpawnPosition = startSpawnPos;
		
		for(int i=0; i<5; i++)
		{
			// spawnPlatformCount = 5;
			DecidePath();
		}	
		Character();
	}

    private void Update()
    {
        //何时 更新掉落时间
        if (GameManager.Instance.IsGameStarted && !GameManager.Instance.IsGameOver &&
            !GameManager.Instance.IsGamePause)
        {
            UpdateFallTime();
        }    
    }

    /// <summary>
    /// 确认路径
    /// </summary>
    private void DecidePath()
	{
		if(spawnPlatformCount > 0)
		{
			spawnPlatformCount--;
			SpawnPlatform();
		}else
		{
			isLeftSpawn = !isLeftSpawn;//方向转向
			spawnPlatformCount = Random.Range(1,4);
			SpawnPlatform();
		}
	}
    /// <summary>
    /// 随机平台主题
    /// </summary>
    private void RandomPlatformTheme()
    {
        int ran = Random.Range(0, vars.platformThemeSpriteList.Count);
        selectPlatformSprite = vars.platformThemeSpriteList[ran];

        if (ran == 2)
        {
            groupType = PlatformGroupType.Winter;
        }
        else  
        {
            groupType = PlatformGroupType.Grass;
        }
    }
	/// <summary>
	/// 按方向生成平台
	/// </summary>
	private void SpawnPlatform()
	{
        int ranObstacleDir = Random.Range(0,2);//随机钉子平台方向 0,1
        //生产普通平台
        if (spawnPlatformCount >= 1)
        {
            SpawnNormalPlatform(ranObstacleDir);
        }        
        //生成组合平台
        else if (spawnPlatformCount == 0)
        {
            int ran = Random.Range(0,3);
            //生成通用平台
            if (ran == 0)
            {
                SpawnCommonPlatformGroup(ranObstacleDir);
            }
            //生成主题组合平台
            else if (ran == 1)
            {
                switch (groupType)
                {
                    case PlatformGroupType.Grass:
                        SpawnGrassPlatformGroup(ranObstacleDir);
                        break;
                    case PlatformGroupType.Winter:
                        SpawnWinterPlatformGroup(ranObstacleDir);
                        break;
                    default:
                        break;
                }
            }
            //生成钉子组合平台
            else
            {
                int dir = -1;
                if (isLeftSpawn)//向左生成平台
                {
                    dir = 0;    //向右生成钉子平台           
                }
                else
                {
                    dir = 1;    //向左生成钉子平台
                }
                SpawnSpikePlatformGroup(dir);
                    //计算钉子后续第一个平台位置
                if(spikeSpawnDir != 0 )
                {
                    spikeNextPlatformPos = 
                        new Vector3(platformSpawnPosition.x + spikeSpawnDir*vars.nextFirstSpikePlatformXPos,
                        platformSpawnPosition.y + vars.nextSpikeformYPos, 0);
                }
                //生产钉子后续平台
                for (int i= spawnSpikeNextPlatformCount;i >= 1;i-- )
                {
                    spawnSpikeNextPlatform(ranObstacleDir);                    
                    spawnSpikeNextPlatformCount--;
                    
                    spikeNextPlatformPos = new Vector3(spikeNextPlatformPos.x + spikeSpawnDir*vars.nextXPos,
                               spikeNextPlatformPos.y + vars.nextSpikeformYPos, 0);
                }               
                spawnSpikeNextPlatformCount = Random.Range(3, 5);//钉子后续平台个数         
            }
        }
        /// <summary>
        /// 随机生成砖石
        /// </summary>
        int ranSpawnDiamond = Random.Range(0,10);
        if (ranSpawnDiamond == 6 && GameManager.Instance.PlayerIsMove)
        {
            GameObject go = ObjectPool.Instance.GetDiamond();
            go.transform.position = new Vector3(platformSpawnPosition.x,
                platformSpawnPosition.y + 0.5f, 0);
            go.SetActive(true);
        }
        
        if (isLeftSpawn)//向左生成
		{
			platformSpawnPosition = new Vector3( platformSpawnPosition.x - vars.nextXPos,
			platformSpawnPosition.y + vars.nextYPos, 0);
		}else//向右生成
		{
			platformSpawnPosition = new Vector3( platformSpawnPosition.x + vars.nextXPos,
			platformSpawnPosition.y + vars.nextYPos, 0);
		} 
	}
    /// <summary>
    /// 生成普通平台
    /// </summary>
    private void SpawnNormalPlatform(int ranObstacleDir)
    {
        //Instantiate(vars.normalPlatformPre, transform);
        GameObject go = ObjectPool.Instance.GetNormalPlatform();
        go.transform.position = platformSpawnPosition; //生成平台的位置赋予
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite,fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成人物
    /// </summary>
    private void Character()
    {
        GameObject go = Instantiate(vars.characterPre);
        go.transform.position = new Vector3(0, -1.8f, 0);
      
        //go.gameObject.SetActive(false);
    }
    /// <summary>
    /// 生成钉子后续平台
    /// </summary>
    private void spawnSpikeNextPlatform(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetNormalPlatform();
        go.transform.position = spikeNextPlatformPos; //生成后续平台的位置赋予
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成通用组合平台
    /// </summary>
    private void SpawnCommonPlatformGroup(int ranObstacleDir)
    {
        //int ran = Random.Range(0,vars.commonPlatformGroup.Count);
        GameObject go = ObjectPool.Instance.GetCommonPlatformGroup();
            //Instantiate(vars.commonPlatformGroup[ran],transform);
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成草地组合平台
    /// </summary>
    private void SpawnGrassPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetGrassPlatformGroup();
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成冰雪组合平台
    /// </summary>
    private void SpawnWinterPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetWinterPlatformGroup();
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成钉子平台
    /// </summary>
    private void SpawnSpikePlatformGroup(int dir)
    {
        GameObject temp = null;
        if (dir == 0)//右生成钉子平台
        {
            spikeSpawnDir = 1;
            temp = ObjectPool.Instance.GetRightSpikePlatform();
        }
        else
        {
            spikeSpawnDir = -1;
            temp = ObjectPool.Instance.GetLeftSpikePlatform();
        }
        temp.SetActive(true);
        temp.transform.position = platformSpawnPosition;
        temp.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, dir);
    }

    /// <summary>
    /// 平台掉落时间更新
    /// </summary>
    private void UpdateFallTime()
    {
        if (GameManager.Instance.GetGameScore() > milestoneCount)
        {
            milestoneCount *= 2;//里程碑翻倍
            fallTime *= multiple;
            if (fallTime < minFallTime)
            {
                fallTime = minFallTime;
            }
        }
    }
}
