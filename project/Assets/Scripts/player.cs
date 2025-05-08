using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public Inventory inventory;
    private Rigidbody rb;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        // 初始化 inventory
        if (inventory == null)
        {
            inventory = new Inventory();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
