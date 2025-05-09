using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickBarUI : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public QuickBarSlotUI[] uiSlots;

    private void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {
            Transform slot = inventoryManager.hotbarSlots[i];

            if (slot.childCount > 0)
            {
                InventoryItem item = slot.GetChild(0).GetComponent<InventoryItem>();
                if (item != null)
                {
                    uiSlots[i].icon.sprite = item.itemData.icon;
                    uiSlots[i].icon.enabled = true;
                    uiSlots[i].amountText.text = item.amount > 1 ? item.amount.ToString() : "";
                    continue;
                }
            }

            uiSlots[i].icon.enabled = false;
            uiSlots[i].amountText.text = "";
        }
    }
}