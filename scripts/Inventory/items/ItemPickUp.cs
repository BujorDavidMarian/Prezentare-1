using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static UnityEditor.Progress;

public class ItemPickup : MonoBehaviour
{
    public Item itemData;  // The item data
    public int amount = 1;
    private bool isPlayerNearby = false;

    public GameObject handWithItemObject;  // The GameObject containing the hand with the item (weapon)
    public GameObject defaultHandObject;   // The GameObject containing the default hand image
    public Text pickupText;                // The text that shows pickup instructions

    private GameObject currentHandObject;// To keep track of the currently active hand GameObject

    public static InventoryManager Instance;

    private void Start()
    {
        if (pickupText != null)
        {
            pickupText.gameObject.SetActive(false);  // Hide pickup text initially
        }

        if (InventoryManager.currentHandObject == null && defaultHandObject != null)
        {
            InventoryManager.currentHandObject = defaultHandObject;
            InventoryManager.currentHandObject.SetActive(true);
        }

        currentHandObject = InventoryManager.currentHandObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            InteractionUIManager.Instance.Show($"Press E to get the {itemData.Name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            InteractionUIManager.Instance.Hide();
        }
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            InventoryManager inventory = FindFirstObjectByType<InventoryManager>();

            if (inventory == null)
            {
                Debug.LogError("InventoryManager not found in the scene!");
                return;
            }

            if (itemData.keyItem)
            {
                if (inventory != null && !inventory.keyItems.Contains(itemData))
                {
                    inventory.keyItems.Add(itemData); 
                }

                KeyItemInventoryUI keyItemUI = FindObjectOfType<KeyItemInventoryUI>();
                if (keyItemUI != null)
                {
                    keyItemUI.AddKeyItem(itemData);
                    Debug.Log($"Key item {itemData.Name} adaugat în UI-ul pentru key items.");
                }

                InteractionUIManager.Instance.Hide();
                Destroy(gameObject);
            }

            else if (inventory.AddItemToInventory(itemData.ID, amount))
            {
                Debug.Log($" {itemData.Name} (x{amount}) added to inventory!");

                if (pickupText != null)
                {
                    pickupText.gameObject.SetActive(false);
                }

                SwitchHandWithItem();


                InteractionUIManager.Instance.Hide();

                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Item not added! Inventory might be full.");
            }
        }


    }

    private void SwitchHandWithItem()
    {
        InventoryManager inventory = FindFirstObjectByType<InventoryManager>();

        if (inventory == null)
        {
            Debug.LogError("InventoryManager not found!");
            return;
        }

        GameObject newHandObject = inventory.GetItemHandByID(itemData.ID);

        if (newHandObject == null)
        {
            Debug.LogError($"No hand object found for item ID {itemData.ID}");
            return;
        }

        if (InventoryManager.currentHandObject != null)
        {
            Debug.Log($"Deactivating current hand: {InventoryManager.currentHandObject.name}");
            InventoryManager.currentHandObject.SetActive(false);
        }

        Debug.Log($"Activating new hand object: {newHandObject.name}");
        newHandObject.SetActive(true);

        InventoryManager.currentHandObject = newHandObject;
        Debug.Log($"New currentHandObject set to: {InventoryManager.currentHandObject.name}");
    }

    public string GetDescription()
    {
        return $"Pick up {itemData.Name}";
    }
}