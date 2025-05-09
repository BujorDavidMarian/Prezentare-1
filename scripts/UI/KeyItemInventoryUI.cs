using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class KeyItemInventoryUI : MonoBehaviour
{
    public Transform keyItemContentHolder; 
    public GameObject keyItemSlotPrefab;
    public Transform keyItemSlotHolder;

    private List<Item> displayedKeyItems = new List<Item>();

    public void AddKeyItem(Item item)
    {
        if (displayedKeyItems.Contains(item))
        {
            Debug.Log($"Itemul {item.Name} este deja afisat in UI-ul pentru key items.");
            return;
        }

        GameObject slot = Instantiate(keyItemSlotPrefab, keyItemSlotHolder);

        TextMeshProUGUI itemText = slot.GetComponentInChildren<TextMeshProUGUI>();
        if (itemText != null)
        {
            itemText.text = item.Name;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in slot prefab!");
        }

        Image icon = slot.transform.Find("Image")?.GetComponent<Image>();
        if (icon != null)
        {
            icon.sprite = item.icon;
        }
        else
        {
            Debug.LogError("Image component not found in slot prefab!");
        }
        displayedKeyItems.Add(item);
    }
}