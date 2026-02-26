using System.Collections.Generic;
using UnityEngine;

public enum HealType
{
    Health,
    Shield
}

[CreateAssetMenu(fileName = "New Heal Item", menuName = "Inventory/Heal Item")]
public class HealItem : ItemObj
{
    public HealType healType;
    public int healAmount;
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class WeaponItem : ItemObj
{
    public int damage;
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ItemObj
{
    // You can add common item properties here if needed
}

[System.Serializable]
public class WorkbenchCraftableItem
{
    public ItemObj item;
    public List<WorkbenchNeededItem> requiredItems;
}

[System.Serializable]
public class PurchasableItem
{
    public ItemObj item;
    public int price;
}

[System.Serializable]
public class WorkbenchNeededItem
{
    public ItemObj item;
    public int amount;
}

public class ItemObj : ScriptableObject
{
    public string itemLabel;
    public string itemId;
    public Sprite itemIcon;
}