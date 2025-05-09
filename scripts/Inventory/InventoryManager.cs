using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using static UnityEditor.Timeline.Actions.MenuPriority;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventory;

    public Transform inventorySlotHolder;
    public Transform inventoryHotBarSlotHolder;

    public Transform cursor;
    public Vector3 offset;

    public List<bool> isFull;
    public List<Transform> slots;
    public List<Transform> hotbarSlots;
    public List<Item> allItems;

    public int currentSlot;

    public Image inventoryBackground;


    public GameObject defaultHand;
    public static GameObject currentHandObject;
    public List<GameObject> allHandObjects;

    private Transform initialSlot;

    public Transform keyItemSlotHolderUI; 
    public GameObject keyItemSlotPrefab;
    public List<Item> keyItems = new List<Item>();

    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple InventoryManagers found! Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadAllItems();
        InitializeInventory();
        SetISlotIDs();
        CheckSlots();
    }
    public GameObject GetItemHandByID(int itemID)
    {
        Debug.Log($"Checking for item ID {itemID}. Available hands:");
        foreach (GameObject hand in allHandObjects)
        {
            ItemInHand handItem = hand.GetComponent<ItemInHand>();
            if (handItem != null && handItem.itemData != null)
            {
                Debug.Log($"Hand: {hand.name}, Item ID: {handItem.itemData.ID}");
            }
        }

        if (allHandObjects == null || allHandObjects.Count == 0)
        {
            Debug.LogError("allHandObjects is empty or null!");
            return defaultHand;  
        }

        foreach (GameObject handObject in allHandObjects)
        {
            ItemInHand handItemComponent = handObject.GetComponent<ItemInHand>();

            if (handItemComponent == null)
            {
                Debug.LogWarning($"No ItemInHand component found on {handObject.name}");
                continue;
            }

            if (handItemComponent.itemData != null && handItemComponent.itemData.ID == itemID)
            {
                Debug.Log($"Found hand object for item ID {itemID}: {handObject.name}");
                return handObject;
            }
        }

        Debug.LogWarning($"No hand object found for item ID {itemID}, returning default hand.");
        return defaultHand;
    }

    private void Update()
    {
        if (!inventory.activeSelf && cursor.childCount > 0)
        {

            if (initialSlot != null)
            {
                cursor.GetChild(0).SetParent(initialSlot);
                cursor.GetChild(0).localPosition = Vector3.zero;  
                cursor.GetChild(0).gameObject.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipItemFromQuickbar(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipItemFromQuickbar(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipItemFromQuickbar(2);
        }
        if (Input.GetMouseButtonDown(0) && currentHandObject != null )
        {
            UseEquippedItem();
        }


        if (inventory.activeSelf == true)
        {
            cursor.position = Input.mousePosition + offset;
        }
        if(cursor.childCount >0)
        {
            cursor.gameObject.SetActive(true);
        }
        else cursor.gameObject.SetActive(false);
    }
    public void UseEquippedItem()
    {
        if (currentHandObject != null)
        {
            ItemInHand itemInHand = currentHandObject.GetComponent<ItemInHand>();

            if (itemInHand != null && itemInHand.itemData.isHealingItem)
            {
                PlayerHealth playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
                if (playerHealth != null && !inventory.activeInHierarchy)
                {
                    playerHealth.Heal(itemInHand.itemData.healingAmount);
                    Debug.Log($"Ai folosit {itemInHand.itemData.Name} si ai primit {itemInHand.itemData.healingAmount} HP.");

                    bool itemFoundInInventory = false;
                    InventoryItem slotItem = null;

                    for (int i = 0; i < slots.Count; i++)
                    {
                        if (slots[i].childCount > 0)
                        {
                            slotItem = slots[i].GetChild(0).GetComponent<InventoryItem>();
                            if (slotItem != null && slotItem.itemData.ID == itemInHand.itemData.ID)
                            {
                                itemFoundInInventory = true;

                                if (slotItem.amount > 1)
                                {
                                    slotItem.amount--;
                                    slotItem.UpdateUI();
                                }
                                else
                                {
                                    Destroy(slots[i].GetChild(0).gameObject); 
                                    isFull[i] = false;
                                }
                                break;
                            }
                        }
                    }

                    if (itemFoundInInventory && currentHandObject != null)
                    {
                        Debug.Log($" Eliminam obiectul din mana: {currentHandObject.name}");
                        currentHandObject.SetActive(false); 
                        currentHandObject = null;  
                        EquipItem(-1); 
                    }
                }
            }

        }
        else
        {
            Debug.LogWarning("currentHandObject este NULL, nu putem folosi item-ul.");
        }
    }

    public void EquipItemFromQuickbar(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= hotbarSlots.Count)
        {
            Debug.LogError("Slot index invalid!");
            return;
        }

        Transform slot = hotbarSlots[slotIndex];

        if (slot.childCount > 0) 
        {
            InventoryItem itemInSlot = slot.GetChild(0).GetComponent<InventoryItem>();
            if (itemInSlot != null)
            {
                EquipItem(itemInSlot.itemData.ID);
            }
            else
            {
                Debug.LogError("Itemul din slot nu are InventoryItem!");
                EquipItem(-1);
            }
        }
        else
        {
            Debug.Log("Slotul este gol, echipam mana goala.");
            EquipItem(-1);
        }
    }

    public void EquipItem(int itemID)
    {
        Debug.Log($"Echipam item ID: {itemID}");

        foreach (GameObject hand in allHandObjects)
        {
            if (hand != null)
            {
                hand.SetActive(false);
            }
        }

        if (defaultHand != null)
        {
            defaultHand.SetActive(false);
        }

        if (itemID == -1)
        {
            if (defaultHand != null)
            {
                defaultHand.SetActive(true);
                currentHandObject = defaultHand;
                Debug.Log("Am echipat mana goala.");
            }
            return;
        }

        foreach (GameObject handObject in allHandObjects)
        {
            if (handObject != null)
            {
                ItemInHand handScript = handObject.GetComponent<ItemInHand>();
                if (handScript != null && handScript.itemData.ID == itemID)
                {
                    handObject.SetActive(true);
                    currentHandObject = handObject;
                    Debug.Log($"Am echipat {handObject.name} (ID: {itemID})");
                    return;
                }
            }
        }

        if (defaultHand != null)
        {
            defaultHand.SetActive(true);
            currentHandObject = defaultHand;
            Debug.Log("Nu am gasit obiectul, revenim la mana goala.");
        }
    }

    void InitializeInventory()
    {
        for (int i = 0; i < inventorySlotHolder.childCount; i++)
        {
            slots.Add(inventorySlotHolder.GetChild(i));
            isFull.Add(false);
        }
        for (int i = 0; i < inventoryHotBarSlotHolder.childCount; i++)
        {
            slots.Add(inventoryHotBarSlotHolder.GetChild(i));
            hotbarSlots.Add(inventoryHotBarSlotHolder.GetChild(i));
            isFull.Add(false);
        }
    }

    void SetISlotIDs()
    {
        for(int i = 0;i < slots.Count;i++)
        {
            if(slots[i].GetComponent<Slot>() != null)
            {
                slots[i].GetComponent<Slot>().ID = i;
            }
        }
    }

    void LoadAllItems()
    {
        allItems = new List<Item>(Resources.LoadAll<Item>(""));
        Debug.Log("Loaded " + allItems.Count + " items.");
    }

    public void CheckSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            InventoryItem item = slots[i].GetComponentInChildren<InventoryItem>();
            isFull[i] = (item != null && item.amount > 0); 
        }
    }

    public void PickUpDropInventory()
    {
        if (currentSlot < 0 || currentSlot >= slots.Count)
        {
            Debug.LogError($"Invalid slot index: {currentSlot}");
            return;
        }

        if (slots[currentSlot].childCount > 0 && cursor.childCount < 1)
        {
            initialSlot = slots[currentSlot];

            Transform slotItem = slots[currentSlot].GetChild(0);
            if (slotItem != null)
            {
                Instantiate(slotItem.gameObject, cursor);
                Destroy(slotItem.gameObject);
            }
        }

        else if (cursor.childCount > 0 && slots[currentSlot].childCount < 1)
        {
            Transform cursorItem = cursor.GetChild(0);
            if (cursorItem != null)
            {
                InventoryItem invItem = cursorItem.GetComponent<InventoryItem>();
                if (invItem != null)
                {

                    bool isHotbarSlot = false;
                    foreach (Transform hotbarSlot in hotbarSlots)
                    {
                        if (hotbarSlot == slots[currentSlot])
                        {
                            isHotbarSlot = true;
                            break;
                        }
                    }

                    if (isHotbarSlot && !invItem.itemData.QuickBarItem)
                    {
                        Debug.LogWarning($" Itemul '{invItem.itemData.Name}' NU poate fi plasat in quickbar.");
                        return;
                    }


                    GameObject newItem = Instantiate(cursorItem.gameObject, slots[currentSlot]);
                    Destroy(cursorItem.gameObject);
                }
            }
        }


        else if (slots[currentSlot].childCount > 0 && cursor.childCount > 0)
        {
            Transform slotItemTransform = slots[currentSlot].GetChild(0);
            Transform cursorItemTransform = cursor.GetChild(0);

            if (slotItemTransform != null && cursorItemTransform != null)
            {
                var slotItem = slotItemTransform.GetComponent<InventoryItem>();
                var cursorItem = cursorItemTransform.GetComponent<InventoryItem>();

                if (slotItem != null && cursorItem != null && slotItem.itemData.ID == cursorItem.itemData.ID)
                {
                    int totalAmount = slotItem.amount + cursorItem.amount;
                    if (totalAmount > slotItem.itemData.maxStack)
                    {
                        cursorItem.amount = totalAmount - slotItem.itemData.maxStack;
                        slotItem.amount = slotItem.itemData.maxStack;
                    }
                    else
                    {
                        slotItem.amount = totalAmount;
                        Destroy(cursorItemTransform.gameObject);
                    }

                    TextMeshProUGUI slotText = slots[currentSlot].GetComponentInChildren<TextMeshProUGUI>();
                    if (slotText != null)
                    {
                        slotText.text = slotItem.amount > 1 ? slotItem.amount.ToString() : "";
                    }
                }
            }
        }

        CheckSlots();
    }

    public bool AddItemToInventory(int itemID, int amount)
    {

        Item itemData = FindItemByID(itemID);
        if (itemData == null) return false;

        if (itemData.keyItem)
        {
            if (!keyItems.Contains(itemData))
            {
                keyItems.Add(itemData);

                KeyItemInventoryUI ui = Object.FindFirstObjectByType<KeyItemInventoryUI>();
                if (ui != null)
                {
                    ui.AddKeyItem(itemData); 
                }
            }
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].childCount > 0)
            {
                InventoryItem existingItem = slots[i].GetChild(0).GetComponent<InventoryItem>();

                if (existingItem != null && existingItem.itemData.ID == itemID)
                {
                    int totalAmount = existingItem.amount + amount;
                    if (totalAmount > existingItem.itemData.maxStack)
                    {
                        int extra = totalAmount - existingItem.itemData.maxStack;
                        existingItem.amount = existingItem.itemData.maxStack;
                        amount = extra;
                    }
                    else
                    {
                        existingItem.amount = totalAmount;
                        existingItem.UpdateUI(); 
                        return true;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (!isFull[i])
            {
                GameObject itemPrefab = Resources.Load<GameObject>($"Items/{itemData.Name}");
                if (itemPrefab == null)
                {
                    Debug.LogError($"Prefab not found for {itemData.Name} in Resources/Items/");
                    return false;
                }

                GameObject newItemUI = Instantiate(itemPrefab, slots[i]);

                if (newItemUI.GetComponent<InventoryItem>() == null)
                {
                    newItemUI.AddComponent<InventoryItem>();
                }

                InventoryItem newInventoryItem = newItemUI.GetComponent<InventoryItem>();

                newInventoryItem.itemData = itemData;
                newInventoryItem.amount = amount;

                newInventoryItem.UpdateUI();

                CheckSlots();

                return true;
            }
        }

        Debug.Log("Inventarul este plin!");
        return false;
    }

    private Item FindItemByID(int id)
    {
        foreach (Item item in allItems)
        {
            if (item.ID == id)
            {
                Debug.Log($"Item gasit: {item.Name} (ID: {id})");
                return item;
            }
        }
        Debug.LogError($"Item cu ID-ul {id} nu a fost gasit în allItems!");
        return null;
    }
}

