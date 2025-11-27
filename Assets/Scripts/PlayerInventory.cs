using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Item> Items;

    public void AddItem(Item item)
    {
        if (Items.Contains(item))
        {
            return;
        }

        UIManager.Instance.SetItems(item, Items.Count);
        Items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        if (item == null || !Items.Contains(item))
        {
            return;
        }

        Items.Remove(item);
        UIManager.Instance.RefreshItems(Items);
    }
}
