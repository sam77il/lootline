using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int playerHealth = 100;
    public int playerShield = 50;
    public Dictionary<ItemObj, int> playerInventory = new();
    public Dictionary<ItemObj, int> playerStash = new();

    [SerializeField]
    private List<ItemObj> allItems;

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
