using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class LLManager : MonoBehaviour
{
    public List<GameObject> spawnPoints;

    [Header("Input")]
    public InputActionReference inventoryAction;

    [SerializeField]
    private GameObject inventory;

    [SerializeField]
    private GameObject inventoryContent;

    [SerializeField]
    private GameObject inventoryItemPrefab;

    private Player player;

    void OnEnable()
    {
        inventoryAction?.action?.Enable();
    }

    void OnDisable()
    {
        inventoryAction?.action?.Disable();
    }

    void Update()
    {
        if (inventoryAction != null && inventoryAction.action != null && inventoryAction.action.triggered)
        {
            ToggleInventory();
        }

        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
        }
    }

    private void ToggleInventory()
    {
        if (inventory != null)
        {
            bool isActive = inventory.activeSelf;
            inventory.SetActive(!isActive);
            SetCursorState(isActive);
            player.allowMovement = isActive;
            player.allowLook = isActive;
            if (!isActive)
            {
                LoadInventoryTab();
            }
        }
    }
    
    public void LoadInventoryTab()
    {
        // Clear existing items
        foreach (Transform child in inventoryContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Load inventory items
        foreach (var kvp in GameManager.Instance.playerInventory)
        {
            var itemObj = kvp.Key;
            var amount = kvp.Value;
            var itemGO = Instantiate(inventoryItemPrefab, inventoryContent.transform);
            itemGO.GetComponent<InventoryItem>().Initialize(itemObj, amount, false);
        }
    }

    private bool cursorLocked;
    private void SetCursorState(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        cursorLocked = locked;
    }
}
