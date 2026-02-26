using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CraftableItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text itemLabel;

    private ItemObj item;

    public void Initialize(ItemObj craftableItem)
    {
        Debug.Log($"Initializing CraftableItem with {craftableItem.itemLabel}");
        item = craftableItem;
        LoadData();
    }

    private void LoadData()
    {
        if (item != null)
        {
            itemLabel.text = item.itemLabel;
        }
        else
        {
            Debug.LogWarning("Craftable item is not set for CraftableItem component.");
        }
    }
}
