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

    public List<ChestItem> chestContents = new();

    [SerializeField]
    public GameObject pickupBoxPrefab;
    private GameObject pBox;

    private bool isOpen = false;
    private bool playerInRange = false;

    void OnEnable()
    {
        openAction.action?.Enable();
    }

    void OnDisable()
    {
        openAction.action?.Disable();
    }

    void Update()
    {
        if (playerInRange && openAction != null && openAction.action != null && openAction.action.triggered)
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        if (!isOpen)
        {
            isOpen = true;

            Debug.Log("Chest opened! Rewards given to player.");
            foreach (var chestItem in chestContents)
            {
                GameManager.Instance.playerInventory.TryGetValue(chestItem.item, out int currentQuantity);
                GameManager.Instance.playerInventory[chestItem.item] = currentQuantity + chestItem.quantity;
                Debug.Log($"Added {chestItem.quantity} of {chestItem.item.itemLabel} to player inventory.");
            }
            Destroy(gameObject);
            Destroy(pBox);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Canvas canvas = FindAnyObjectByType<Canvas>();
            pBox = Instantiate(pickupBoxPrefab, canvas.transform);
            PickUp pickupBox = pBox.GetComponent<PickUp>();
            pickupBox.Initialize("Press E to open chest");
            Debug.Log("Player inside chest trigger!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Destroy(pBox);
            Debug.Log("Player left chest trigger!");
        }
    }
}
