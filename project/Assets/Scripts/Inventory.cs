using System.Collections.Generic;

public class Inventory
{
    public Dictionary<string, int> items = new Dictionary<string, int>();

    public void AddItem(string itemName, int amount = 1)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName] += amount;
        }
        else
        {
            items.Add(itemName, amount);
        }
    }
}