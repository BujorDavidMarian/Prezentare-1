using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    public int ID;

    public InventoryManager manager;

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InventoryManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            manager.currentSlot = ID;
            manager.PickUpDropInventory();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (transform.childCount > 0)
            {
                InventoryItem invItem = transform.GetChild(0).GetComponent<InventoryItem>();

                if (invItem != null)
                {
                    ItemContextMenu.Instance.OpenContextMenu(invItem.itemData, Input.mousePosition, invItem);
                    Debug.Log($"Click dreapta pe slotul cu ID {ID}, item: {invItem.itemData.Name}");
                }
            }
        }
    }
}