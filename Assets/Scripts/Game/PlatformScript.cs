using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 挂载到 Platform组合中
/// </summary>
public class PlatformScript : MonoBehaviour {

    public SpriteRenderer[] spriteRenderers;//挂载到平台组合上，将组合中需要渲染的平台拖入
    public GameObject obstacle;//障碍物
    private bool startTimer = false;//计时器开关
    private float fallTime;
    private Rigidbody2D my_Body;//平台刚体

    private void Awake()
    {
        my_Body = GetComponent<Rigidbody2D>();
}

    /// <summary>
    /// 平台生成与显示
    /// </summary>
    /// <param name="sprite">传入不同类型贴图selectSprite（fire/ice/normal/grass）</param>
    /// <param name="falltime">平台掉落时间</param>
    /// <param name="obstacleDir">障碍物生成平台的左？右？</param>
    public void Init(Sprite sprite,float falltime ,int obstacleDir)//
    {
        my_Body.bodyType = RigidbodyType2D.Static;
        this.fallTime = falltime;
        startTimer = true;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = sprite;//传给各自Platform.sprite
        }

        if (obstacleDir == 0)//朝右边，默认朝左边
        {
            /// <summary>
            ///obstacle 为空 : 区分普通平台/钉子平台与平台组；
            ///普通平台与钉子平台不控制障碍物方向变化：故不指定obstacle挂载
            /// </summary>
            if (obstacle != null)
            {
                obstacle.transform.localPosition = new Vector3(-obstacle.transform.localPosition.x,
                    obstacle.transform.localPosition.y, 0);
            }        
        }
    }

    private void Update()
    {
        if (startTimer && GameManager.Instance.PlayerIsMove)
        {
            fallTime -= Time.deltaTime;//帧数间隔时间，按秒减
            if (fallTime < 0)
            {
                //掉落效果
                startTimer = false;
                if (my_Body.bodyType != RigidbodyType2D.Dynamic)
                {
                    my_Body.bodyType = RigidbodyType2D.Dynamic;
                    StartCoroutine(DealyHide());
                }
            }
        }
        /// <summary>
        /// 为掉落的平台超过摄像机-6的距离，也开启隐藏
        /// </summary>
        if (transform.position.y - Camera.main.transform.position.y < -6)
        {
            StartCoroutine(DealyHide());
        }
    }
    /// <summary>
    /// 掉落的平台计时隐藏，在ObjectPool中复用
    /// </summary>
    /// <returns></returns>
    private IEnumerator DealyHide()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
    
}
