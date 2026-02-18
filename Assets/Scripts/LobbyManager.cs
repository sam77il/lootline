using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

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

    private void Start()
    {
        foreach (var uiTab in uiTabs)
        {
            uiTab.button.onClick.AddListener(() => SwitchTab(uiTab));
        }

        startGameButton.onClick.AddListener(() => SceneManager.LoadScene("Main"));
        #if UNITY_EDITOR
        quitGameButton.onClick.AddListener(() => UnityEditor.EditorApplication.isPlaying = false);
        #else
        quitGameButton.onClick.AddListener(() => Application.Quit());
        #endif
    }

    private void SwitchTab(UITab uiTab)
    {
        foreach (var tab in uiTabs)
        {
            tab.tab.SetActive(tab == uiTab);

            if (tab.tabName == "inventory")
            {
                LoadInventoryTab();
            }
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