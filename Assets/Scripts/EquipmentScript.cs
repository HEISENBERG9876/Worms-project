using System.Collections.Generic;
using UnityEngine;

public class EquipmentScript : MonoBehaviour
{
    public List<UsableItemScript> availableItems;
    public UsableItemScript currentItem;
    private int currentItemIndex = 0;

    void Start()
    {
        EquipItem(currentItemIndex);
        currentItem.Hide();
    }

    public void SwitchItem()
    {
        currentItemIndex = (currentItemIndex + 1) % availableItems.Count;
        EquipItem(currentItemIndex);
        currentItem.Show();
    }

    private void EquipItem(int index)
    {
        foreach (var availableItem in availableItems)
        {
            availableItem.Hide();
        }

        UsableItemScript item = availableItems[index];
        currentItem = item;
        item.Show();

    }

    public UsableItemScript getCurrentItem()
    {
        return availableItems[currentItemIndex];
    }

    private void OnDisable()
    {
        currentItem.Hide();
    }
}