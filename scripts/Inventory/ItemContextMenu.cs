using UnityEngine;
using UnityEngine.UI;

public class ItemContextMenu : MonoBehaviour
{
    public static ItemContextMenu Instance;

    public GameObject panel;
    public Button combineButton;
    public Button deleteButton;

    public RecipeDatabase recipeDatabase;

    private Item currentItem;
    private InventoryItem currentInventoryItem;

    private bool isWaitingForSecondItem = false;
    private Item firstItemToCombine;
    private InventoryItem firstInventoryItem;

    public bool IsWaitingForSecondItem => isWaitingForSecondItem;
    public Item FirstItemToCombine => firstItemToCombine;
    public InventoryItem FirstInventoryItem => firstInventoryItem;

    public bool IsCombining => isWaitingForSecondItem;

    public Vector3 contextOffset = new Vector3(150f, -30f, 0f);

    void Start()
    {
        deleteButton.onClick.AddListener(OnDelete);
        combineButton.onClick.AddListener(OnCombine);
    }

    void Update()
    {
        if (panel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(panel.GetComponent<RectTransform>(), Input.mousePosition))
            {
                Close();
            }
        }

        if (isWaitingForSecondItem && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;

            foreach (Transform slot in InventoryManager.Instance.slots)
            {
                if (slot.childCount == 0) continue;

                InventoryItem selectedItem = slot.GetChild(0).GetComponent<InventoryItem>();
                RectTransform itemRect = selectedItem.GetComponent<RectTransform>();

                if (selectedItem != null &&
                    selectedItem != firstInventoryItem &&
                    RectTransformUtility.RectangleContainsScreenPoint(itemRect, mousePos))
                {
                    TryCombineItems(firstItemToCombine, firstInventoryItem, selectedItem.itemData, selectedItem);
                    break;
                }
            }

            isWaitingForSecondItem = false;
            ResetItemHighlights();
        }
    }

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void OpenContextMenu(Item item, Vector3 position, InventoryItem invItem)
    {
        currentItem = item;
        currentInventoryItem = invItem;

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0f;

        RectTransform panelRectTransform = panel.GetComponent<RectTransform>();

        if (panelRectTransform != null)
        {
            Vector2 finalPosition = mousePosition + contextOffset;

            panelRectTransform.position = finalPosition;

            panel.SetActive(true);
        }
        else
        {
            Debug.LogError("RectTransform-ul pentru panel nu a fost gasit!");
        }

        deleteButton.interactable = item.isDeletable;
        combineButton.interactable = CanCombine(item);
    }

    public bool CanCombine(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("Itemul transmis în CanCombine este null.");
            return false;
        }

        if (InventoryManager.Instance == null || RecipeDatabase.Instance == null)
        {
            Debug.LogWarning("Managerii nu sunt initializati.");
            return false;
        }

        foreach (var slot in InventoryManager.Instance.slots)
        {
            if (slot == null || slot.childCount == 0)
                continue;

            Transform child = slot.GetChild(0);
            if (child == null)
                continue;

            InventoryItem invItem = child.GetComponent<InventoryItem>();
            if (invItem == null || invItem.itemData == null)
                continue;

            if (invItem.itemData.ID != item.ID)
            {
                var result = RecipeDatabase.Instance.GetCombinationResult(item.ID, invItem.itemData.ID);
                if (result.HasValue)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void OnDelete()
    {
        if (currentItem != null && currentItem.isDeletable)
        {
            Destroy(currentInventoryItem.gameObject);
        }

        Close();
    }

    public void OnCombine()
    {
        isWaitingForSecondItem = true;
        firstItemToCombine = currentItem;
        firstInventoryItem = currentInventoryItem;

        HighlightValidCombinationTargets(firstItemToCombine);
        Close(); 
    }

    private void HighlightValidCombinationTargets(Item baseItem)
    {
        foreach (Transform slot in InventoryManager.Instance.slots)
        {
            if (slot.childCount == 0) continue;

            InventoryItem invItem = slot.GetChild(0).GetComponent<InventoryItem>();
            Image img = slot.GetChild(0).GetComponent<Image>();

            if (invItem == null || img == null) continue;

            bool canCombine = RecipeDatabase.Instance.GetCombinationResult(baseItem.ID, invItem.itemData.ID).HasValue
                              && baseItem.ID != invItem.itemData.ID;

            img.color = canCombine ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f); 
        }
    }

    public void TryCombineWith(Item first, InventoryItem firstInv, Item second, InventoryItem secondInv)
    {
        var result = RecipeDatabase.Instance.GetCombinationResult(first.ID, second.ID);

        if (result.HasValue)
        {
            int resultID = result.Value;

            InventoryManager.Instance.AddItemToInventory(resultID, 1);

            Destroy(firstInv.gameObject);
            Destroy(secondInv.gameObject);

            Debug.Log($"Combinat: {first.Name} + {second.Name} -> {resultID}");
        }
        else
        {
            Debug.Log("Combinatie invalida.");
        }
    }

    private void ResetItemHighlights()
    {
        foreach (Transform slot in InventoryManager.Instance.slots)
        {
            if (slot.childCount == 0) continue;

            Image img = slot.GetChild(0).GetComponent<Image>();
            if (img != null)
            {
                img.color = Color.white;
            }
        }
    }
    private void ResetCombinationState()
    {
        isWaitingForSecondItem = false;
        firstItemToCombine = null;
        firstInventoryItem = null; 

        foreach (Transform slot in InventoryManager.Instance.slots)
        {
            if (slot.childCount == 0) continue;

            CanvasGroup cg = slot.GetChild(0).GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 1f;
            }
        }
    }

    private void TryCombineItems(Item first, InventoryItem firstInv, Item second, InventoryItem secondInv)
    {
        var result = RecipeDatabase.Instance.GetCombinationResult(first.ID, second.ID);

        if (result.HasValue)
        {
            int resultID = result.Value;
            InventoryManager.Instance.AddItemToInventory(resultID, 1);

            Destroy(firstInv.gameObject);
            Destroy(secondInv.gameObject);

            Debug.Log($"Combinat: {first.Name} + {second.Name} -> {resultID}");
        }
        else
        {
            Debug.Log("Combinatie invalida.");
        }
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}
