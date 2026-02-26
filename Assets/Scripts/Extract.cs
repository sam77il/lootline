using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Extract : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference openAction;

    [Header("UI")]
    [SerializeField] 
    private GameObject pickupBoxPrefab;

    private GameObject interactionBox;   // "Press E"
    private GameObject extractingBox;    // Countdown UI
    private Canvas cachedCanvas;

    private bool playerInRange = false;
    private bool isExtracting = false;

    private int countdownTime = 60;

    private Coroutine extractRoutine;

    private void Awake()
    {
        cachedCanvas = FindAnyObjectByType<Canvas>();

        if (cachedCanvas == null)
        {
            Debug.LogError("No Canvas found in scene!");
        }
    }

    private void OnEnable()
    {
        if (openAction != null && openAction.action != null)
            openAction.action.Enable();
    }

    private void OnDisable()
    {
        if (openAction != null && openAction.action != null)
            openAction.action.Disable();
    }

    private void Update()
    {
        if (!playerInRange) return;
        if (isExtracting) return;
        if (openAction?.action == null) return;

        if (openAction.action.WasPressedThisFrame())
        {
            StartExtraction();
        }
    }

    private void StartExtraction()
    {
        if (isExtracting) return;

        isExtracting = true;

        if (interactionBox != null)
            Destroy(interactionBox);

        extractRoutine = StartCoroutine(ExtractCountdown());
    }

    private IEnumerator ExtractCountdown()
    {
        if (cachedCanvas == null)
            yield break;

        extractingBox = Instantiate(pickupBoxPrefab, cachedCanvas.transform);
        PickUp pickUp = extractingBox.GetComponent<PickUp>();

        for (int i = countdownTime; i > 0; i--)
        {
            if (!playerInRange)   // Cancel if player leaves
            {
                CancelExtraction();
                yield break;
            }

            pickUp.Initialize($"Extracting... {i} seconds");
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("Extraction complete!");

        GameManager.Instance.startedGame = false;
        GameManager.Instance.SaveData();

        SceneManager.LoadScene(0);
    }

    private void CancelExtraction()
    {
        isExtracting = false;

        if (extractRoutine != null)
            StopCoroutine(extractRoutine);

        if (extractingBox != null)
            Destroy(extractingBox);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isExtracting) return;
        if (cachedCanvas == null) return;

        playerInRange = true;

        if (interactionBox == null)
        {
            interactionBox = Instantiate(pickupBoxPrefab, cachedCanvas.transform);
            PickUp pickupBox = interactionBox.GetComponent<PickUp>();
            pickupBox.Initialize("Press E to extract");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (interactionBox != null)
            Destroy(interactionBox);

        if (isExtracting)
            CancelExtraction();
    }
}