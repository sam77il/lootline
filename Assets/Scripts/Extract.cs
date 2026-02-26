using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Extract : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference openAction;

    [SerializeField]
    public GameObject pickupBoxPrefab;
    private GameObject pBox;
    private GameObject extractingBox;

    private bool isOpen = false;
    private bool playerInRange = false;
    private int countdownTime = 60; // 1 Minute


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
            InteractExtract();
        }
    }

    private void InteractExtract()
    {
        if (!isOpen)
        {
            isOpen = true;
            StartCoroutine(ExtractCountdown());
            Destroy(pBox);
        }
    }

    private IEnumerator ExtractCountdown()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        extractingBox = Instantiate(pickupBoxPrefab, canvas.transform);
        PickUp pickUp = extractingBox.GetComponent<PickUp>();
        pickUp.Initialize("Extracting...");
        for (int i = countdownTime; i > 0; i--)
        {
            pickUp.Initialize($"Extracting... {i} seconds");
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("Zeit abgelaufen!");
        SceneManager.LoadScene(0);
        GameManager.Instance.startedGame = false;
        GameManager.Instance.SaveData();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            playerInRange = true;
            Canvas canvas = FindAnyObjectByType<Canvas>();
            pBox = Instantiate(pickupBoxPrefab, canvas.transform);
            PickUp pickupBox = pBox.GetComponent<PickUp>();
            pickupBox.Initialize("Press E to extract");
            Debug.Log("Player inside extract trigger!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            playerInRange = false;
            Destroy(pBox);
            Debug.Log("Player left extract trigger!");
        }
    }
}
