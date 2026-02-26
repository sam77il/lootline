using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class ChestItem
{
    public ItemObj item;
    public int quantity;
}

public class Chest : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference openAction;

    [Header("UI")]
    public List<ChestItem> chestContents = new();
    [SerializeField] private GameObject pickupBoxPrefab;

    private GameObject pBox;
    private Canvas cachedCanvas;

    private bool isOpen = false;
    private bool playerInRange = false;

    private void Awake()
    {
        cachedCanvas = FindAnyObjectByType<Canvas>();
        if (cachedCanvas == null)
            Debug.LogError("No Canvas found in scene!");
    }

    private void OnEnable()
    {
        openAction?.action?.Enable();
    }

    private void OnDisable()
    {
        openAction?.action?.Disable();
    }

    private void Update()
    {
        if (playerInRange && !isOpen && openAction?.action != null && openAction.action.WasPressedThisFrame())
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        if (isOpen) return;
        isOpen = true;

        Debug.Log("Chest opened! Rewards given to player.");
        foreach (var chestItem in chestContents)
        {
            GameManager.Instance.playerInventory.TryGetValue(chestItem.item, out int currentQuantity);
            GameManager.Instance.playerInventory[chestItem.item] = currentQuantity + chestItem.quantity;
            Debug.Log($"Added {chestItem.quantity} of {chestItem.item.itemLabel} to player inventory.");
        }

        if (pBox != null)
            Destroy(pBox);

        // Optional: destroy after frame to prevent update conflicts
        Destroy(gameObject, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isOpen || cachedCanvas == null) return;

        playerInRange = true;

        if (pBox == null)
        {
            pBox = Instantiate(pickupBoxPrefab, cachedCanvas.transform);
            PickUp pickupBox = pBox.GetComponent<PickUp>();
            pickupBox.Initialize("Press E to open chest");
            Debug.Log("Player inside chest trigger!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (pBox != null)
            Destroy(pBox);

        Debug.Log("Player left chest trigger!");
    }
}