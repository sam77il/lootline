using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int playerHealth = 100;
    public int playerShield = 50;
    public int playerLLC = 1000;
    public Dictionary<ItemObj, int> playerInventory = new();
    public Dictionary<ItemObj, int> playerStash = new();
    public bool startedGame = false;
    private string path;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private List<ItemObj> allItems;

    [SerializeField]
    public List<WorkbenchCraftableItem> craftableItems;

    [SerializeField]
    public List<PurchasableItem> shopItems;

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
        path = Application.persistentDataPath + "/playerdata.json";

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        LoadPlayerData();
    }

public void SaveData()
{
    PlayerData data = new();
    data.coins = playerLLC;

    foreach (var pair in playerInventory)
    {
        InventorySaveData saveData = new();
        saveData.itemID = pair.Key.itemId;
        saveData.amount = pair.Value;

        data.inventory.Add(saveData);
    }

    foreach (var pair in playerStash)
    {
        InventorySaveData saveData = new();
        saveData.itemID = pair.Key.itemId;
        saveData.amount = pair.Value;

        data.stash.Add(saveData);
    }

    string json = JsonUtility.ToJson(data, true);
    File.WriteAllText(path, json);
}

public void LoadPlayerData()
{
    if (!File.Exists(path))
        return;

    string json = File.ReadAllText(path);
    PlayerData data = JsonUtility.FromJson<PlayerData>(json);

    // Clear old data first (VERY IMPORTANT)
    playerInventory.Clear();
    playerStash.Clear();

    playerLLC = data.coins;

    // Load Inventory
    foreach (InventorySaveData entry in data.inventory)
    {
        ItemObj item = allItems.Find(i => i.itemId == entry.itemID);

        if (item != null)
        {
            playerInventory[item] = entry.amount;
        }
        else
        {
            Debug.LogWarning($"Item with ID {entry.itemID} not found in allItems list.");
        }
    }

    // Load Stash
    foreach (InventorySaveData entry in data.stash)
    {
        ItemObj item = allItems.Find(i => i.itemId == entry.itemID);

        if (item != null)
        {
            playerStash[item] = entry.amount;
        }
        else
        {
            Debug.LogWarning($"Item with ID {entry.itemID} not found in allItems list.");
        }
    }
}
}
