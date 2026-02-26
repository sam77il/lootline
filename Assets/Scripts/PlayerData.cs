using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public List<InventorySaveData> inventory = new();
    public List<InventorySaveData> stash = new();
    public int coins;
}

[System.Serializable]
public class InventorySaveData
{
    public string itemID;
    public int amount;
}