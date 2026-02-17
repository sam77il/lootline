using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField]
    private Image itemIcon;

    [SerializeField]
    private TMPro.TMP_Text amountText;

    public ItemObj Item { get; private set; }
    public int Amount { get; private set; }
    public bool IsInStash { get; set; }

    public void Initialize(ItemObj item, int amount, bool isInStash = false)
    {
        Amount = amount;
        IsInStash = isInStash;
        Item = item;
        itemIcon.sprite = item.itemIcon;
        UpdateAmountText();

        gameObject.GetComponent<Button>().onClick.AddListener(() => OnItemClicked());
    }

    private void OnItemClicked()
    {
        Debug.Log($"Clicked on {Item.itemLabel} in {(IsInStash ? "Stash" : "Inventory")}");
        if (IsInStash)
        {
            // Move item from stash to inventory
            GameManager.Instance.playerStash.Remove(Item); // Remove from stash
            if (GameManager.Instance.playerInventory.ContainsKey(Item))
            {
                GameManager.Instance.playerInventory[Item] += Amount; // Add to inventory
            }
            else
            {
                GameManager.Instance.playerInventory[Item] = Amount; // Add new item to inventory
            }
        }
        else
        {
            // Move item from inventory to stash
            GameManager.Instance.playerInventory.Remove(Item); // Remove from inventory
            if (GameManager.Instance.playerStash.ContainsKey(Item))
            {
                GameManager.Instance.playerStash[Item] += Amount; // Add to stash
            }
            else
            {
                GameManager.Instance.playerStash[Item] = Amount; // Add new item to stash
            }
        }

        LobbyManager.Instance.LoadInventoryTab(); // Refresh UI
        UpdateAmountText();
    }

    private void UpdateAmountText()
    {
        amountText.text = $"{Amount}x";
    }
}
