using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int playerHealth = 100;
    public int playerShield = 50;
    public Dictionary<ItemObj, int> playerInventory = new();
    public Dictionary<ItemObj, int> playerStash = new();

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private List<ItemObj> allItems;

    private LLManager llManager;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            // Find all spawn points in the scene
            llManager = FindAnyObjectByType<LLManager>();

            if (llManager != null)
            {
                if (llManager.spawnPoints.Count > 0)
                {
                    GameObject randomSpawn = llManager.spawnPoints[Random.Range(0, llManager.spawnPoints.Count)];
                    Instantiate(playerPrefab, randomSpawn.transform.position, randomSpawn.transform.rotation);
                    Debug.Log($"Player spawned at {randomSpawn.transform.position}");
                }
                else
                {
                    Debug.LogWarning("LLManager not found in the scene. Spawn points will not be available.");
                }
            }
        }
    }

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

        foreach (var item in allItems)
        {
            playerInventory[item] = 3;
            playerStash[item] = 2;
        }
    }
}
