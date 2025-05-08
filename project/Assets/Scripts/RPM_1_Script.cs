using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPM_1_Script : MonoBehaviour
{
    //动画组件
    private Animator m_Anim;
    
    private bool m_IsSwitching = false;  // 状态锁，防止短时间重复切换
    private bool m_IsFastMode = false;    // 是否处于快速模式
    public float moveSpeed = 5f;        // 移动速度
    private float moveDirection = 0f;    // 移动方向

    void Start()
    {
        m_Anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (m_IsSwitching) return;  // 如果正在切换状态，直接返回

        // 检测Shift键状态
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        
        // 处理Shift键状态变化
        if (shiftPressed && !m_IsFastMode && m_Anim.GetBool("Run"))
        {
            // 从Run切换到Fast
            StartCoroutine(SwitchToFastMode());
        }
        else if (!shiftPressed && m_IsFastMode)
        {
            // 从Fast切换回Run
            StartCoroutine(SwitchFromFastMode());
        }

        // 检测跳跃键（上箭头或W）
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(ActivateJump());
        }

        // 检测滑行键（下箭头或S） 
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(ActivateSlide());
        }
        
        // 空格键切换Run和Stand（仅在不是Fast模式且没有跳跃/滑行时）
        if (Input.GetKeyDown(KeyCode.Space) && !m_IsFastMode && 
                !m_Anim.GetBool("Jump") && !m_Anim.GetBool("Slide"))
        {
            StartCoroutine(ToggleRunAndStandWithLock());
        }

        // 处理左右移动输入
        moveDirection = 0f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            moveDirection = 1f; // 左移
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            moveDirection = -1f; // 右移
        }

        // 平滑移动角色
        if (moveDirection != 0f && (m_Anim.GetBool("Stand") || m_Anim.GetBool("Run") || m_Anim.GetBool("Fast")))
        {
            Vector3 movement = new Vector3(0, 0, moveDirection * moveSpeed * Time.deltaTime);
            transform.Translate(movement, Space.World);
        }
    }

    IEnumerator ToggleRunAndStandWithLock()
    {
        m_IsSwitching = true;

        if (m_Anim.GetBool("Stand"))
        {
            SetAllBoolsFalse();
            m_Anim.SetBool("Run", true);
        }
        else
        {
            SetAllBoolsFalse();
            m_Anim.SetBool("Stand", true);
        }

        yield return new WaitForSeconds(0.3f);
        m_IsSwitching = false;
    }

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
    }

    IEnumerator ActivateJump()
    {
        m_IsSwitching = true;
        
        // 保存当前是否为Fast模式
        bool wasFastMode = m_IsFastMode;
        
        SetAllBoolsFalse();
        m_Anim.SetBool("Jump", true);
        
        yield return new WaitForSeconds(0.3f);
        m_IsSwitching = false;
        
        // 跳跃后自动回到之前的移动状态
        yield return new WaitForSeconds(0.3f); // 跳跃动画持续时间
        if (!m_IsSwitching)
        {
            SetAllBoolsFalse();
            if (wasFastMode)
                m_Anim.SetBool("Fast", true);
            else if (!m_Anim.GetBool("Stand")) // 如果之前不是站立状态
                m_Anim.SetBool("Run", true);
            else
                m_Anim.SetBool("Stand", true);
        }
    }

    IEnumerator ActivateSlide()
    {
        m_IsSwitching = true;
        
        // 保存当前是否为Fast模式
        bool wasFastMode = m_IsFastMode;
        
        SetAllBoolsFalse();
        m_Anim.SetBool("Slide", true);
        
        yield return new WaitForSeconds(0.3f);
        m_IsSwitching = false;
        
        // 滑行后自动回到之前的移动状态
        yield return new WaitForSeconds(1.0f); // 滑行动画持续时间
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
}