using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class person : MonoBehaviour
{
    
    private Rigidbody rb;
    public int force = 5;
    public float jumpForce = 0.1f; // 跳起的力
    public float additionalGravity = 100f; // 额外的重力，加快下落速度
    public float crouchScaleFactor = 0.5f; // 下蹲时的缩放因子
    private Vector3 originalScale; // 原始的缩放大小
    public Camera mainCamera; // 引用主相机
    private int jumpCount = 0; // 记录跳跃次数
    public int maxJumpCount = 2; // 最大跳跃次数
    public Inventory inventory;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // 如果未手动指定相机，则使用主相机
        }
        originalScale = transform.localScale; // 记录原始缩放大小

        // 初始化 inventory
        if (inventory == null)
        {
            inventory = new Inventory();
        }
        

        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");  //取得键盘的水平输入
        float v = Input.GetAxis("Vertical");
        // 获取相机的前向和右向向量
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        // 去除相机向量的垂直分量，确保小球只在水平面上移动
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // 计算基于相机的移动方向
        Vector3 moveDirection = (cameraForward * v + cameraRight * h).normalized;

        // 对刚体施加力
        rb.AddForce(moveDirection * force);

        // 处理跳起动作
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
        }

        // 处理下蹲动作
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y * crouchScaleFactor, originalScale.z);
        }
        else
        {
            transform.localScale = originalScale;
        }

        // 加快下落速度
        if (rb.velocity.y < 0)
        {
            rb.AddForce(Vector3.down * additionalGravity);
        }
    }

    // 当小球碰撞到地面时，重置跳跃次数
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("碰撞发生，碰撞对象标签: " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("ground"))
        {
            //Debug.Log("小球碰撞到地面，重置跳跃次数。");
            jumpCount = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reward"))
        {
            string itemName = other.gameObject.name;
            //Debug.Log("碰撞到的物品名称: " + itemName);
            inventory.AddItem(itemName);
            //Debug.Log("添加后 " + itemName + " 的数量: " + inventory.items[itemName]);
            Destroy(other.gameObject);
        }
    }
    
    
}
