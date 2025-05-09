using UnityEngine;

[CreateAssetMenu(fileName = "NewItemRecipe", menuName = "Inventory/Item Recipe")]
public class ItemRecipe : ScriptableObject
{
    public int inputID1;
    public int inputID2;
    public int resultID;
}