using UnityEngine;
using UnityEngine.UI;

public class NeededItem : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_Text itemLabel;

    [SerializeField]
    private TMPro.TMP_Text itemQuantity;

    [SerializeField]
    private Image itemIcon;

    public ItemObj requiredItem;
    public int requiredQuantity = 1;

    public void Initialize(ItemObj item, int quantity)
    {
        requiredItem = item;
        requiredQuantity = quantity;

        LoadData();
    }

    private void LoadData()
    {
        if (requiredItem != null)
        {
            itemLabel.text = requiredItem.itemLabel;
            itemIcon.sprite = requiredItem.itemIcon;
            itemQuantity.text = $"{requiredQuantity}x | Stash: {(GameManager.Instance.playerStash.ContainsKey(requiredItem) ? GameManager.Instance.playerStash[requiredItem] : 0)}x"; // Show how many the player has in stash
        }
        else
        {
            Debug.LogWarning("Required item is not set for NeededItem component.");
        }
    }
}
