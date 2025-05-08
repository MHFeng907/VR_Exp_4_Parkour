using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class mixamo_1 : MonoBehaviour
{
    private Animator m_Anim;
    private bool m_IsSwitching = false;
    private bool m_IsFastMode = false;
    private bool m_IsMoving = false; // 控制是否允许移动
    
    public float sideMoveSpeed = 3f;    // Z轴左右移动速度
    private float moveDirection = 0f;
    private Vector3 forwardDirection = Vector3.right; // X轴正向

    public Inventory inventory; // 背包
    
    private int jumpCount = 0; // 当前跳跃次数
    public int maxJumpCount = 2; // 最大跳跃次数

    private float verticalSpeed = 0f; // 当前垂直速度
    public float jumpForce = 8f; // 每次跳跃的速度
    public float gravity = 20f; // 重力加速度

    private CharacterController controller; // 人物控制器 本质是刚体

    // 计时器和UI显示相关变量
    public TextMeshProUGUI countdownText;
    private float countdownTime = 300f;  // 初始倒计时（300秒）
    private bool isGameStarted = false;  // 判断游戏是否开始

    // 碰撞障碍
    private float lastBHitTime = -5f; // 上次碰撞B物体的时间
    private float bHitCooldown = 1.5f; // 冷却时间

    private float currentSpeed = 0f; // 当前X轴移动速度
    public float acceleration = 2f;  // 加速度

    private float runInitialSpeed = 2f;    // 初始Run速度
    private float fastInitialSpeed = 8f;  // 初始Fast速度
    private float runMaxSpeed = 8f;        // 最大Run速度
    private float fastMaxSpeed = 20f;      // 最大Fast速度

    // 新增碰撞器参数
    private float originalHeight;
    private Vector3 originalCenter;
    [SerializeField] private float slideHeight = 0.8f;
    [SerializeField] private Vector3 slideCenter = new Vector3(0, 0.4f, 0);
    //倒计时ui
    [Header("扣时特效")]
    [SerializeField] private Color hitColor = new Color(1, 0.2f, 0.2f, 1);
    [SerializeField][Range(0.1f, 1f)] private float flashDuration = 0.3f;
    [SerializeField][Range(1f, 1.5f)] private float punchScale = 1.2f;
    [SerializeField][Range(0f, 5f)] private float shakeIntensity = 2f;
    private Color originalColor;

    public TextMeshProUGUI coinText;      //定义Text公共变量，准备关联金币积分Text对象
    private int score = 0;
    // 开始、结束按钮
    [Header("UI Controls")]
    public Button startButton;
    public Button endButton;
    public GameObject gameOverPanel; // 游戏结束界面
    [Header("UI References")]
    public TextMeshProUGUI finalScoreText; // 直接拖拽赋值
                                           // 在类变量区新增
    [Header("Game Over UI")]
    public Button restartButton;
    public Button exitButton;

    [Header("背景音乐设置")]
    public AudioClip bgmIntro;        // 开场音乐
    public AudioClip bgmGameplay;     // 游戏中音乐
    private AudioSource audioSource;  // 播放器组件

    [Header("音效设置")]
    public AudioClip rewardSound; // 拾取奖励音效
    public AudioClip hitSound;    // 撞击障碍音效

    void Start()
    {
        m_Anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        m_Anim.SetBool("Stand", true); // 初始状态 站立
        // 初始化碰撞器参数
        originalHeight = controller.height;
        originalCenter = controller.center;
        originalColor = countdownText.color;//初始化计时器颜色
        // 初始化背包
        if (inventory == null)
        {
            inventory = new Inventory();
        }

        // 初始化计时器文本
        if (countdownText != null)
        {
            countdownText.text = "Time: 5:00";
        }
        // StartCoroutine(DebugSpeed());  // 检测速度

        // 初始化UI状态
        startButton.gameObject.SetActive(true);
        endButton.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f; // 确保游戏时间正常流动
        // 绑定按钮事件
        restartButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(ExitGame);

        audioSource = GetComponent<AudioSource>();

        if (bgmIntro != null)
        {
            audioSource.clip = bgmIntro;
            audioSource.loop = true;
            audioSource.Play();  // 播放初始背景音乐
        }
    }
    // 新增游戏重置方法
    public void RestartGame()
    {
        // 重置所有游戏状态
        Time.timeScale = 1f;
        isGameStarted = true;
        m_IsMoving = true;
        currentSpeed = runInitialSpeed;
        score = 0;
        countdownTime = 300f;

        // 重置UI
        gameOverPanel.SetActive(false);
        startButton.gameObject.SetActive(false);
        endButton.gameObject.SetActive(true);

        // 重置角色状态
        SetAllBoolsFalse();
        m_Anim.SetBool("Run", true);
        transform.position = Vector3.zero; // 根据需要设置重生点

        // 更新UI显示
        coinText.text = "0";
        UpdateCountdownText();
        if (bgmGameplay != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = bgmGameplay;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // 新增退出游戏方法
    public void ExitGame()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    // 新增按钮控制方法
    public void StartGame()
    {
        isGameStarted = true;
        startButton.gameObject.SetActive(false);
        endButton.gameObject.SetActive(true);

        // 如果当前是站立状态，切换到跑步
        if (m_Anim.GetBool("Stand"))
        {
            StartCoroutine(ToggleRunAndStandWithLock());
        }
        
        if (bgmGameplay != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = bgmGameplay;
            audioSource.loop = true;
            audioSource.Play();
        }

    }
    public void EndGame()
    {
        GameOver();
        if (bgmIntro != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = bgmIntro;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    private void GameOver()
    {
        // 停止所有游戏逻辑
        Time.timeScale = 0f;
        isGameStarted = false;
        m_IsMoving = false;

        // 显示结束界面
        gameOverPanel.SetActive(true);
        endButton.gameObject.SetActive(false);

        // 更新结算界面数据（可根据需要扩展）
        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + score.ToString();
        }
        else
        {
            Debug.LogError("FinalScoreText 未赋值！");
        }
    }


    void Update()
    {
        // 在现有地面检测前添加防卡地逻辑
        if (controller.height != originalHeight && controller.isGrounded)
        {
            controller.Move(Vector3.up * 0.1f);
        }
        // 处理倒计时
        
        if (isGameStarted && countdownTime > 0f)
        {
            countdownTime -= Time.deltaTime;
            UpdateCountdownText();
        }
        else if (isGameStarted && countdownTime <= 0f)
        {
            GameOver();
            isGameStarted = false; // 确保只触发一次
        }
        else if (countdownTime <= 0f)
        {
            // 时间结束，跳转到结算页面  // 这里待补充
            GameOver();
        }
        // ===== 新增：只有游戏开始后才处理键盘输入 =====
        if (!isGameStarted) return;

        // 是否处于动画切换中
        if (m_IsSwitching) return;

        // 1. 控制是否切换移动模式（Run / Fast）
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (shiftPressed && !m_IsFastMode && m_Anim.GetBool("Run"))
        {
            StartCoroutine(SwitchToFastMode());
        }
        else if (!shiftPressed && m_IsFastMode)
        {
            StartCoroutine(SwitchFromFastMode());
        }

        // 2. 跳跃逻辑
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && jumpCount < maxJumpCount)
        {
            jumpCount++;
            verticalSpeed = jumpForce;
            StartCoroutine(ActivateJump());
        }

        // 3. 下蹲/滑行
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(ActivateSlide());
        }

        // 4. 空格键切换跑/停
        if (Input.GetKeyDown(KeyCode.Space) && !m_IsFastMode && 
            !m_Anim.GetBool("Jump") && !m_Anim.GetBool("Slide"))
        {
            StartCoroutine(ToggleRunAndStandWithLock());
        }

        // 5. 左右方向输入（Z轴方向）
        moveDirection = 0f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            moveDirection = 1f; // 左移（Z负向）
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            moveDirection = -1f; // 右移（Z正向）
        }

        // 6. 地面检测 & 重力应用
        if (controller.isGrounded)
        {
            if (verticalSpeed < 0)
            verticalSpeed = -1f;
            jumpCount = 0;
        }
        else
        {
            verticalSpeed -= gravity * Time.deltaTime;
        }

        // 7. 计算最终移动向量
        Vector3 move = Vector3.zero;

        if (m_IsMoving)
        {
            float targetSpeed = m_IsFastMode ? fastMaxSpeed : runMaxSpeed;
            float initialSpeed = m_IsFastMode ? fastInitialSpeed : runInitialSpeed;

            // 如果当前速度小于目标速度，则线性增加
            if (currentSpeed < targetSpeed)
            {
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, targetSpeed);
            }

            Vector3 forwardMove = forwardDirection * currentSpeed;
            Vector3 sideMove = new Vector3(0, 0, moveDirection * sideMoveSpeed);

            move += forwardMove + sideMove;
        }
        move.y = verticalSpeed; // 加上垂直速度
        controller.Move(move * Time.deltaTime);
    }
    // 新增调试可视化
    void OnDrawGizmos()
    {
        if (controller != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + controller.center,
                new Vector3(controller.radius * 2, controller.height, controller.radius * 2));
        }
    }
    // 按下空格键时，开始计时
    IEnumerator ToggleRunAndStandWithLock()
    {
        m_IsSwitching = true;

        if (m_Anim.GetBool("Stand"))
        {
            SetAllBoolsFalse();
            m_Anim.SetBool("Run", true);
            m_IsMoving = true;
            currentSpeed = runInitialSpeed;
        }
        else
        {
            SetAllBoolsFalse();
            m_Anim.SetBool("Stand", true);
            m_IsMoving = false;
            currentSpeed = 0;
        }

        yield return new WaitForSeconds(0.3f);
        m_IsSwitching = false;
    }
    IEnumerator FlashTimerEffect()
    {
        // 记录原始状态
        Color originalTextColor = countdownText.color;
        Vector3 originalScale = countdownText.transform.localScale;
        Vector3 originalPosition = countdownText.rectTransform.localPosition;

        // 同时启动多个特效
        float timer = 0f;
        while (timer < flashDuration)
        {
            // 颜色渐变
            countdownText.color = Color.Lerp(hitColor, originalColor, timer / flashDuration);

            // 缩放动画
            float scaleProgress = Mathf.PingPong(timer * 2, 1f);
            countdownText.transform.localScale = originalScale * Mathf.Lerp(1f, punchScale, scaleProgress);

            // 位置抖动
            float shakeX = Mathf.Sin(timer * 50) * shakeIntensity;
            float shakeY = Mathf.Cos(timer * 45) * shakeIntensity;
            countdownText.rectTransform.localPosition = originalPosition + new Vector3(shakeX, shakeY, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        // 恢复原始状态
        countdownText.color = originalColor;
        countdownText.transform.localScale = originalScale;
        countdownText.rectTransform.localPosition = originalPosition;
    }
    // 更新倒计时文本显示
    private void UpdateCountdownText()
    {
        int minutes = Mathf.FloorToInt(countdownTime / 60f);
        int seconds = Mathf.FloorToInt(countdownTime % 60f);
        countdownText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }


    // 以下与动画处理机相关
    IEnumerator SwitchToFastMode()
    {
        m_IsSwitching = true;
        m_IsFastMode = true;
        
        SetAllBoolsFalse();
        m_Anim.SetBool("Fast", true);
        
        yield return new WaitForSeconds(0.3f);
        m_IsSwitching = false;
    }

    IEnumerator SwitchFromFastMode()
    {
        m_IsSwitching = true;
        m_IsFastMode = false;
        
        SetAllBoolsFalse();
        m_Anim.SetBool("Run", true);
        
        yield return new WaitForSeconds(0.3f);
        m_IsSwitching = false;
        currentSpeed = runInitialSpeed;
    }

    IEnumerator ActivateJump()
    {
        m_IsSwitching = true;

        bool wasFastMode = m_IsFastMode;

        SetAllBoolsFalse();
        m_Anim.SetBool("Jump", true);

        yield return new WaitForSeconds(0.3f);
        m_IsSwitching = false;

        yield return new WaitForSeconds(0.3f);

        if (!m_IsSwitching)
        {
            SetAllBoolsFalse();
            if (wasFastMode)
                m_Anim.SetBool("Fast", true);
            else if (!m_Anim.GetBool("Stand"))
                m_Anim.SetBool("Run", true);
            else
                m_Anim.SetBool("Stand", true);
        }
    }

    // 修改后的ActivateSlide协程
    IEnumerator ActivateSlide()
    {
        m_IsSwitching = true;

        bool wasFastMode = m_IsFastMode;
        float preSpeed = currentSpeed;

        // 调整碰撞器参数
        controller.height = slideHeight;
        controller.center = slideCenter;
        currentSpeed *= 1.2f;

        SetAllBoolsFalse();
        m_Anim.SetBool("Slide", true);

        yield return new WaitForSeconds(0.3f);
        m_IsSwitching = false;

        yield return new WaitForSeconds(0.7f);

        // 恢复碰撞器参数
        controller.height = originalHeight;
        controller.center = originalCenter;
        currentSpeed = preSpeed;

        if (!m_IsSwitching)
        {
            SetAllBoolsFalse();
            if (wasFastMode)
                m_Anim.SetBool("Fast", true);
            else if (!m_Anim.GetBool("Stand"))
                m_Anim.SetBool("Run", true);
            else
                m_Anim.SetBool("Stand", true);
        }
    }

    // 辅助函数：将所有动画bool参数设为false
    private void SetAllBoolsFalse()
    {
        m_Anim.SetBool("Run", false);
        m_Anim.SetBool("Stand", false);
        m_Anim.SetBool("Fast", false);
        m_Anim.SetBool("Jump", false);
        m_Anim.SetBool("Slide", false);
    }

    // 金币拾取检测 Reward
    private void OnTriggerEnter(Collider other)
    {
        // 处理奖励拾取逻辑
        if (other.CompareTag("Reward"))
        {
            string itemName = other.gameObject.name;

            if (inventory != null)
            {
                inventory.AddItem(itemName);
                if (itemName == "onegold")
                {
                    score = score + 1;//未知原因加分会*2，单个金币
                   
                }
                if (itemName == "goldcoins")
                {
                    score = score + 100;//金币堆加分100
                    
                }
                if (itemName == "silvercoins")
                {
                    score = score + 50;//银币堆加分50
                    
                }
                if (itemName == "boxPresent1level")
                {
                    score = score + 5;//金色箱子加10
                    
                }
                if (itemName == "boxPresent2level")
                {
                    score = score + 2;//紫色箱子加4
                   
                }
                if (itemName == "boxPresent3level")
                {
                    score = score + 3;//蓝色箱子加6
                   
                }
                coinText.text = score.ToString();
                Debug.Log($"拾取物品: {itemName}");
            }
            
            if (rewardSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(rewardSound, 0.4f);
            }
            Destroy(other.gameObject);
        }
    }

    // 场景碰撞检测
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("B"))
        {
            //Debug.Log("与B物体发生碰撞!");
            float currentTime = Time.time;

            if (currentTime - lastBHitTime > bHitCooldown)
            {
                lastBHitTime = currentTime;
                countdownTime = Mathf.Max(0f, countdownTime - 5f);
                Debug.Log("碰撞到 B，倒计时减少5秒！当前倒计时: " + countdownTime);
                StartCoroutine(FlashTimerEffect()); // 触发特效
                currentSpeed = m_IsFastMode ? fastInitialSpeed : runInitialSpeed;
                if (hitSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(hitSound, 4.0f);
                }
            }
            else
            {
                //Debug.Log("B碰撞触发冷却中，未扣时间");
            }
            
        }
        else
        {
            // Debug.Log("与非B物体发生碰撞: " + hit.gameObject.name);
        }
    }

    // 速度检测
    IEnumerator DebugSpeed()
    {
        while (true)
        {
            Debug.Log("当前速度 currentSpeed: " + currentSpeed.ToString("F2"));

            // 如果你还想输出综合的移动向量（含方向）
            Vector3 totalVelocity = forwardDirection * currentSpeed + new Vector3(0, 0, moveDirection * sideMoveSpeed);
            Debug.Log("总移动向量（不含重力）: " + totalVelocity.ToString("F2"));

            yield return new WaitForSeconds(10f);
        }
    }
}