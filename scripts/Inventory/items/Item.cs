using UnityEngine;
using System.Collections.Generic;
using System.Collections;


[CreateAssetMenu (fileName = "Item", menuName = "Unity Inventory", order = 0)]
public class Item : ScriptableObject
{
    public int ID;
    public string Name;
    public Sprite icon;
    public int maxStack;
    public bool isHealingItem = false;
    public bool isWeapon = false;
    public bool QuickBarItem = true;
    public bool keyItem = false;
    public bool isDeletable;
    public string description;
    public int healingAmount;
}
