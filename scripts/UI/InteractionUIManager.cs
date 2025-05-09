using UnityEngine;
using UnityEngine.UI;

public class InteractionUIManager : MonoBehaviour
{
    public Text interactionText;
    public static InteractionUIManager Instance;

    private void Awake()
    {
        Instance = this;
        interactionText.gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        interactionText.text = message;
        interactionText.gameObject.SetActive(true);
    }

    public void Hide()
    {
        interactionText.gameObject.SetActive(false);
    }
}