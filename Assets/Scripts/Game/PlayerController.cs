using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {
	/// <summary>
	/// 是否向左移动，反正向右
	/// </summary>
	private bool isMoveLeft = false;
    /// <summary>
    /// 是否正在跳跃
    /// </summary>
    private bool isJumping = false;
    /// <summary>
    /// 移动后计时平台掉落
    /// </summary>
    public bool isMove = false;
    private Vector3 nextPlatformLeft,nextPlatformRight,deathPos;
	private ManagerVars vars;
    /// <summary>
    /// 人物的刚体
    /// </summary>
    private Rigidbody2D my_Body;
    /// <summary>
    /// 射线起点
    /// </summary>
    public Transform rayDown,rayLeft,rayRight;
    /// <summary>
    /// 射线检测的Layer层度
    /// </summary>
    public LayerMask platformLayer,obstacleLayer;
    /// <summary>
    /// 人物Renderer中，Sorting Layer（Player）层级
    /// </summary>
    private SpriteRenderer spriteRenderer;
    /// <summary>
    /// 音乐资源
    /// </summary>
    private AudioSource m_AudioSource;

    private void Awake()
	{
        EventCenter.AddListener<int>(EventDefine.ChangeSkin, ChangeSkin);

		vars = ManagerVars.GetManagerVars();
        spriteRenderer = GetComponent<SpriteRenderer>();
        my_Body = GetComponent<Rigidbody2D>();
        m_AudioSource = GetComponent<AudioSource>();

    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<int>(EventDefine.ChangeSkin, ChangeSkin);
    }
    private void Start()
    {
        ChangeSkin(GameManager.Instance.GetCurrentSelectedSkin());
    }
    /// <summary>
    /// 点击事件平台异常处理
    /// </summary>
    private int count;
    private bool IsPointerOverGameObject(Vector2 mousePosition)
    {
        //创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //向点击位置发射一条射线，检测是否点击的UI
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults.Count > 0;
    }

    private void Update()
    {
        /// <summary>
        /// 暂停效果原理:如果点击的是UI,那么就忽略的掉
        /// 是否点击了一个EventSystem对象（UI)?
        /// 是，if返回true, return 忽略掉点击行为.
        /// </summary>

        if (IsPointerOverGameObject(Input.mousePosition)) return;
        if (GameManager.Instance.IsGameStarted == false || GameManager.Instance.IsGameOver == true || GameManager.Instance.IsGamePause)
            return;
		if(Input.GetMouseButtonDown(0) && isJumping == false && nextPlatformLeft !=Vector3.zero)//触摸屏检测
		{
            if (isMove == false)
            {
                isMove = true;
                EventCenter.Broadcast(EventDefine.PlayerMove);
            }
            m_AudioSource.PlayOneShot(vars.jumpClip);
            EventCenter.Broadcast(EventDefine.DecidePath);//下一个
            isJumping = true;
            Vector3 mousePos = Input.mousePosition;
			//点击左边屏幕
			if(mousePos.x <=Screen.width/2)
			{
				isMoveLeft = true;
			}//点击右边屏幕
			else if(mousePos.x > Screen.width/2)
			{
				isMoveLeft = false;				
			}
			Jump();
		}
        /// <summary>
        /// 死亡二：障碍物接触死
        /// 左右射线不为空，且碰到物体标签为Obstacle
        /// </summary>
        if (isJumping && IsRayObstacle() && GameManager.Instance.IsGameOver == false)
        {
            m_AudioSource.PlayOneShot(vars.hitClip);
            deathPos = new Vector3(transform.position.x,transform.position.y - 0.7f,transform.position.z);
            GameObject go = ObjectPool.Instance.GetDeathEffect();
            go.transform.position = deathPos;
            go.SetActive(true);
            GameManager.Instance.IsGameOver = true;
            spriteRenderer.enabled = false;//玩家不显示Destroy(gameObject);唤醒不了结束页面？
            StartCoroutine(DealyShowGameOverPanel());//线程调用结束面板
        }
        /// <summary>
        /// 死亡一：掉落死亡
        /// 人物的刚体的Y轴速度为负，即掉落死亡
        /// </summary>
        if (my_Body.velocity.y < 0 && IsRayPlatform() ==false && GameManager.Instance.IsGameOver == false)
        {
            m_AudioSource.PlayOneShot(vars.fallClip);
            spriteRenderer.sortingLayerName = "Default";//被遮挡
            GetComponent<BoxCollider2D>().enabled = false;//掉落
            GameManager.Instance.IsGameOver = true;
            StartCoroutine(DealyShowGameOverPanel());
        }
        /// <summary>
        /// 死亡三：平台一起掉落死亡
        /// 人物掉落超过摄像机-6的距离，也开启隐藏
        /// </summary>
        if (transform.position.y - Camera.main.transform.position.y < -6)
        {
            m_AudioSource.PlayOneShot(vars.fallClip);
            GameManager.Instance.IsGameOver = true;
            GetComponent<BoxCollider2D>().enabled = false;//掉落 
            //线程调用结束面板
            StartCoroutine(DealyShowGameOverPanel());
        }

    }

    IEnumerator DealyShowGameOverPanel()
    {
        yield return new WaitForSeconds(1.5f);
        EventCenter.Broadcast(EventDefine.HideGamePanel);
        EventCenter.Broadcast(EventDefine.ShowGameOverPanel);
    }
    /// <summary>
    /// 修改皮肤
    /// </summary>
    private void ChangeSkin(int skinIndex)
    {
        spriteRenderer.sprite = vars.characterSkinSpriteList[skinIndex];
    }

    private GameObject lastHitGo = null;
    /// <summary>
    /// 判断射线是否射到平台
    /// </summary>
    private bool IsRayPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(rayDown.position,Vector2.down,1f,platformLayer);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Platform")
            {
                if (lastHitGo != hit.collider.gameObject)
                {
                    if (lastHitGo == null)//游戏开始
                    {
                        lastHitGo = hit.collider.gameObject;
                        return true;
                    }
                    EventCenter.Broadcast(EventDefine.AddScore);
                    lastHitGo = hit.collider.gameObject;
                }
                return true;
            }
        }
         return false;
    }
    /// <summary>
    /// 判断射线是否射到障碍物
    /// </summary>
    private bool IsRayObstacle()
    {
        if (GameManager.Instance.IsGameOver) return true;//安全检测，游戏结束了则不继续射线检测

        RaycastHit2D leftHit = Physics2D.Raycast(rayLeft.position, Vector2.left, 0.08f, obstacleLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rayRight.position, Vector2.right, 0.08f, obstacleLayer);
        if (leftHit.collider != null)
        {
            if (leftHit.collider.tag == "Obstacle")
            {
                return true;
            }
        }
        if (rightHit.collider != null)
        {
            if (rightHit.collider.tag == "Obstacle")
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 跳跃控制
    /// </summary>
    public void Jump()
	{

        if (isMoveLeft)
        {
            transform.localScale = new Vector3(-1, 1, 1);//Scale控制任务左右
            transform.DOMoveX(nextPlatformLeft.x, 0.2f);//位置，时间
            transform.DOMoveY(nextPlatformLeft.y + 0.8f, 0.15f);
        }
        else
        {
            transform.DOMoveX(nextPlatformRight.x, 0.2f);
            transform.DOMoveY(nextPlatformRight.y + 0.8f, 0.15f);
            transform.localScale = Vector3.one;
        }
       
	}
    /// <summary>
    /// 跳跃的碰撞穿过检查
    /// isTrigger碰撞
    /// (条件:都有Conlider,一方Rigidbody,一方Conlider is Trigger)
    /// (需要两个Conlider)
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)//
	{
		if(collision.tag == "Platform")//Platform指定一个标签
		{
            isJumping = false;
		    Vector3 currentPlatformPos = collision.gameObject.transform.position;
		    nextPlatformLeft = new Vector3(currentPlatformPos.x - vars.nextXPos, 
			    currentPlatformPos.y + vars.nextYPos, 0);
		    nextPlatformRight = new Vector3(currentPlatformPos.x + vars.nextXPos, 
			    currentPlatformPos.y + vars.nextYPos, 0);
		}
	}
    /// <summary>
    /// 吃到砖石的碰撞检查 
    /// OnCollsionEnter2D(Collider2D collision) 
    /// 条件两物体Collider的isTrigger都不勾选，
    /// 都有Collider、Rigidbody.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)//
    {
        if (collision.collider.tag == "Pickup")
        {
            m_AudioSource.PlayOneShot(vars.diamondClip);
            EventCenter.Broadcast(EventDefine.AddDiamond);
            //吃到砖石
            collision.gameObject.SetActive(false);
        }
    }
}






 
