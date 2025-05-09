using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour , IPointerClickHandler
{
    public Item itemData;
    public Text amountText;
    public int amount;

    private void Awake()
    {
        if (amountText == null)
        {
            amountText = GetComponentInChildren<Text>();
        }

        if (amountText == null)
        {
            Debug.LogError($"InventoryItem: amountText is NULL on {gameObject.name}!", this);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (itemData == null)
        {
            Debug.LogError("InventoryItem: itemData is NULL!", this);
            return;
        }

        if (amountText != null)
        {
            if (amount > 1)
            {
                amountText.gameObject.SetActive(true);
                amountText.text = amount.ToString();
            }
            else
            {
                amountText.gameObject.SetActive(false); 
            }
        }
    }


    public void SetAmount(int newAmount)
    {
        amount = newAmount;
        UpdateUI();  
    }


    public void DecreaseAmount(int value)
    {
        amount -= value;
        if (amount < 0) amount = 0; 
        UpdateUI(); 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ItemContextMenu.Instance.OpenContextMenu(itemData, transform.position, this);
        }
    }

    void OnMouseDown()
    {
        if (ItemContextMenu.Instance != null && ItemContextMenu.Instance.IsWaitingForSecondItem)
        {
            ItemContextMenu.Instance.TryCombineWith(
                ItemContextMenu.Instance.FirstItemToCombine,
                ItemContextMenu.Instance.FirstInventoryItem,
                this.itemData,
                this
            );
        }
    }
}