using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

[System.Serializable]
public class UITab
{
    public string tabName;
    public Button button;
    public GameObject tab;
}

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [SerializeField]
    private List<UITab> uiTabs;

    [SerializeField]
    private Button startGameButton;

    [SerializeField]
    private Button quitGameButton;

    [SerializeField]
    private GameObject inventoryStashContent;

    [SerializeField]
    private GameObject inventoryInvContent;

    [SerializeField]
    private GameObject inventoryItemPrefab;

    [SerializeField]
    private GameObject neededItemPrefab;

    [SerializeField]
    private GameObject neededItemsList;

    [SerializeField]
    private GameObject craftableItemsList;

    [SerializeField]
    private GameObject craftableItemPrefab;

    [SerializeField]
    private GameObject shopItemPrefab;

    [SerializeField]
    private GameObject shopItemsList;

    [SerializeField]
    private TMPro.TMP_Text selectedWorkbenchItem;

    [SerializeField]
    private TMPro.TMP_Text playerLLCText;

    [SerializeField]
    private Button craftButton;

    private WorkbenchCraftableItem selectedCraftableItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void UpdateLLCDisplay()
    {
        playerLLCText.text = $"LL-C: {GameManager.Instance.playerLLC}";
    }

    private void Start()
    {
        // With dots
        UpdateLLCDisplay();
        foreach (var uiTab in uiTabs)
        {
            uiTab.button.onClick.AddListener(() => SwitchTab(uiTab));
        }

        startGameButton.onClick.AddListener(() => { SceneManager.LoadScene("Main"); GameManager.Instance.startedGame = true; });
        craftButton.onClick.AddListener(() => CraftButton());
        #if UNITY_EDITOR
        quitGameButton.onClick.AddListener(() =>
        {
            GameManager.Instance.SaveData();
            UnityEditor.EditorApplication.isPlaying = false;
        });
        #else
        quitGameButton.onClick.AddListener(() => {
            GameManager.Instance.SaveData();
            Application.Quit();
        });
        #endif
    }

    private void CraftButton()
    {
        if (selectedWorkbenchItem.text == "Select an item to see required materials")
        {
            Debug.LogWarning("No craftable item selected!");
            return;
        }
        List<bool> hasAllItems = new();

        // Implement crafting logic here, e.g., check if the player has the required items, remove them from inventory, and add the crafted item.
        foreach (var neededItem in selectedCraftableItem.requiredItems)
        {
            if (GameManager.Instance.playerStash.ContainsKey(neededItem.item) && GameManager.Instance.playerStash[neededItem.item] >= neededItem.amount)
            {
                hasAllItems.Add(true);
            }
            else
            {
                hasAllItems.Add(false);
            }
        }

        if (hasAllItems.Contains(false))
        {
            Debug.LogWarning("Not enough materials to craft this item!");
            return;
        } else
        {
            // Remove required items from stash
            foreach (var neededItem in selectedCraftableItem.requiredItems)
            {
                GameManager.Instance.playerStash[neededItem.item] -= neededItem.amount;
                if (GameManager.Instance.playerStash[neededItem.item] <= 0)
                {
                    GameManager.Instance.playerStash.Remove(neededItem.item);
                }
            }

            // Add crafted item to stash
            if (GameManager.Instance.playerStash.ContainsKey(selectedCraftableItem.item))
            {
                GameManager.Instance.playerStash[selectedCraftableItem.item]++;
            }
            else
            {
                GameManager.Instance.playerStash[selectedCraftableItem.item] = 1;
            }

            Debug.Log($"Crafted {selectedCraftableItem.item.itemLabel}!");
        }
    }

    private void SwitchTab(UITab uiTab)
    {
        foreach (var tab in uiTabs)
        {
            tab.tab.SetActive(tab == uiTab);

            if (tab.tabName == "inventory")
            {
                LoadInventoryTab();
            } else if (tab.tabName == "workbench")
            {
                LoadWorkbenchTab();
            } else if (tab.tabName == "shop")
            {
                LoadShopTab();
            }
        }
    }

    private void LoadShopTab()
    {
        foreach (Transform child in shopItemsList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var shopItem in GameManager.Instance.shopItems)
        {
            var itemGO = Instantiate(shopItemPrefab, shopItemsList.transform);
            var shopItemUI = itemGO.GetComponent<ShopItem>();
            shopItemUI.Initialize(shopItem);
        }
    }

    private void LoadWorkbenchTab()
    {
        foreach (Transform child in craftableItemsList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var craftable in GameManager.Instance.craftableItems)
        {
            var itemGO = Instantiate(craftableItemPrefab, craftableItemsList.transform);
            var craftableItemUI = itemGO.GetComponent<CraftableItem>();
            craftableItemUI.Initialize(craftable.item);
            var craftableItemButton = itemGO.GetComponent<Button>();
            craftableItemButton.onClick.AddListener(() => {
                selectedWorkbenchItem.text = $"Needed Items for: {craftable.item.itemLabel}";
                // Clear existing needed items
                foreach (Transform child in neededItemsList.transform)
                {
                    Destroy(child.gameObject);
                }

                // Load needed items for the selected craftable item
                foreach (var neededItem in craftable.requiredItems)
                {
                    var neededItemGO = Instantiate(neededItemPrefab, neededItemsList.transform);
                    var neededItemUI = neededItemGO.GetComponent<NeededItem>();
                    neededItemUI.Initialize(neededItem.item, neededItem.amount);
                }
                selectedCraftableItem = craftable;
            });
        }
        
    }

    public void LoadInventoryTab()
    {
        // Clear existing items
        foreach (Transform child in inventoryStashContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in inventoryInvContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Load stash items (example with all items, replace with actual stash data)
        foreach (var item in GameManager.Instance.playerStash)
        {
            var itemGO = Instantiate(inventoryItemPrefab, inventoryStashContent.transform);
            var inventoryItem = itemGO.GetComponent<InventoryItem>();
            inventoryItem.Initialize(item.Key, item.Value, true); // Example amount, replace with actual data
        }

        // Load inventory items (example with all items, replace with actual inventory data)
        foreach (var item in GameManager.Instance.playerInventory)
        {
            var itemGO = Instantiate(inventoryItemPrefab, inventoryInvContent.transform);
            var inventoryItem = itemGO.GetComponent<InventoryItem>();
            inventoryItem.Initialize(item.Key, item.Value, false); // Example amount, replace with actual data
        }
    }
}