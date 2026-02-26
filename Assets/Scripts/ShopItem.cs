using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField]
    private Image itemIcon;

    [SerializeField]
    private TMPro.TMP_Text itemNameText;

    [SerializeField]
    private TMPro.TMP_Text itemPriceText;

    [SerializeField]
    private Button purchaseButton;

    public void Initialize(PurchasableItem purchasableItem)
    {
        itemIcon.sprite = purchasableItem.item.itemIcon;
        itemNameText.text = purchasableItem.item.itemLabel;
        itemPriceText.text = $"Price: {purchasableItem.price} LL-C";
        purchaseButton.onClick.AddListener(() => PurchaseItem(purchasableItem));
    }

    private void PurchaseItem(PurchasableItem purchasableItem)
    {
        // Implement purchase logic here, e.g., check if the player has enough currency, deduct currency, and add the item to the player's inventory.
        if (GameManager.Instance.playerLLC >= purchasableItem.price)
        {
            // Add item to inventory
            if (GameManager.Instance.playerStash.ContainsKey(purchasableItem.item))
            {
                GameManager.Instance.playerStash[purchasableItem.item]++;
            }
            else
            {
                GameManager.Instance.playerStash[purchasableItem.item] = 1;
            }
            Debug.Log($"Purchased {purchasableItem.item.itemLabel} for {purchasableItem.price} LL-C");
            GameManager.Instance.playerLLC -= purchasableItem.price;
            LobbyManager.Instance.UpdateLLCDisplay();
        }
    }
}
