using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public mixamo_1 player;
    public Button inventoryButton;
    public GameObject inventoryPanel;
    public List<Image> itemIcons;
    public List<TMP_Text> itemCounts;

    private void Start()
    {
        inventoryButton.onClick.AddListener(ToggleInventory);
        inventoryPanel.SetActive(false);
    }

    private void ToggleInventory()
    {
        //Debug.Log("触发 ToggleInventory 方法，调用栈信息：" + System.Environment.StackTrace);
        //Debug.Log("当前是否按下空格键: " + Input.GetKeyDown(KeyCode.Space));
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        UpdateInventoryUI();
    }

    private void UpdateInventoryUI()
{
    // 定义固定的物品名称列表，按照你希望的显示顺序排列
    string[] itemNames = { "onegold", "goldcoins", "silvercoins", "boxPresent1level", "boxPresent2level", "boxPresent3level" };

    int index = 0;
    foreach (string itemName in itemNames)
    {
        //Debug.Log("尝试查找物品: " + itemName);
        if (player.inventory.items.TryGetValue(itemName, out int count))
        {
            //Debug.Log("找到物品 " + itemName + "，数量: " + count);
            if (index < itemIcons.Count && index < itemCounts.Count)
            {
                // 这里可以根据itemName设置对应的图标
                // itemIcons[index].sprite = GetItemSprite(itemName);
                itemCounts[index].text = count.ToString();
            }
        }
        else
        {
            //Debug.Log("未找到物品 " + itemName);
            if (index < itemCounts.Count)
            {
                itemCounts[index].text = "0";
            }
        }
        index++;
    }
}
}    