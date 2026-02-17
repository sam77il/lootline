using UnityEngine;

public enum HealType
{
    Health,
    Shield
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemObj : ScriptableObject
{
    public string itemLabel;
    public string itemId;
    public Sprite itemIcon;

    [Header("Consumable Properties")]
    [Tooltip("Only used if itemType is Consumable")]
    public HealType healType;
    public int healAmount;
    [Header("Weapon Properties")]
    [Tooltip("Only used if itemType is Weapon")]
    public int damage;
}