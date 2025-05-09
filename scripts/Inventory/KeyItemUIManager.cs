using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static UnityEditor.Progress;

public class KeyItemUIManager : MonoBehaviour
{
    public static KeyItemUIManager Instance;

    public GameObject panel;
    public Transform keyItemSlotHolder;
    public GameObject keyItemSlotPrefab;
    public Button exitButton;
    public PlayerControlToggle controlToggle;
    public Sprite defaultKeyIcon;

    private Door currentDoor;

    void Awake()
    {
        Instance = this;

        if (exitButton != null)
            exitButton.onClick.AddListener(HidePanel);

        if (panel != null)
            panel.SetActive(false);
    }

    public void ShowKeyItemSelection(Door door)
    {
        currentDoor = door;
        ClearSlots();

        panel.SetActive(true);

        if (controlToggle != null && controlToggle.IsControlEnabled())
        {
            controlToggle.ToggleControl(false);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        InventoryManager inventory = FindFirstObjectByType<InventoryManager>();

        if (keyItemSlotPrefab == null)
            Debug.LogError("KeyItemSlotPrefab is not assigned!");

        if (keyItemSlotHolder == null)
            Debug.LogError("KeyItemSlotHolder is not assigned!");

        if (panel == null)
            Debug.LogError("Panel is not assigned!");

        if (exitButton == null)
            Debug.LogWarning("ExitButton is not assigned!");

        if (controlToggle == null)
            Debug.LogWarning("ControlToggle is not assigned!");


        inventory = FindFirstObjectByType<InventoryManager>();

        foreach (Item keyItem in inventory.keyItems)
        {
            GameObject slotUI = Instantiate(keyItemSlotPrefab, keyItemSlotHolder);
            slotUI.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = keyItem.Name;

            Item itemCopy = keyItem;
            Button btn = slotUI.GetComponent<Button>();
            btn.onClick.AddListener(() => OnKeyItemSelected(itemCopy));

            Image icon = slotUI.transform.Find("Image").GetComponent<Image>();
            if (icon != null)
            {
                if (keyItem.icon != null)
                {
                    icon.sprite = keyItem.icon;
                }
                else
                {
                    Debug.LogWarning($"Itemul {keyItem.Name} nu are icon, folosim default.");
                    icon.sprite = defaultKeyIcon;
                }
            }
        }
    }

    void OnKeyItemSelected(Item selectedKey)
    {
        if (currentDoor.TryUnlock(selectedKey))
        {
            Debug.Log("Door unlocked!");

            InventoryManager inventory = FindFirstObjectByType<InventoryManager>();
            if (inventory != null && inventory.keyItems.Contains(selectedKey))
            {
                inventory.keyItems.Remove(selectedKey);
            }

            currentDoor.SetUnlockedPermanently();
            HidePanel();
        }
        else
        {
            Debug.Log("That key doesn't work!");
        }
    }

    public void HidePanel()
    {
        panel.SetActive(false);
        ClearSlots();

        if (controlToggle != null && !controlToggle.IsControlEnabled())
        {
            controlToggle.ToggleControl(true);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void ClearSlots()
    {
        foreach (Transform child in keyItemSlotHolder)
        {
            Destroy(child.gameObject);
        }
    }
}