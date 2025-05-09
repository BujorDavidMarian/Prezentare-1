using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactLayer;

    private IInteractable currentInteractable;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (interactable != currentInteractable)
                {
                    currentInteractable = interactable;
                    InteractionUIManager.Instance.Show(interactable.GetDescription());
                }
            }
            else
            {
                ClearUI();
            }
        }
        else
        {
            ClearUI();
        }

        if (Input.GetKeyDown(interactKey) && currentInteractable != null)
        {
            currentInteractable.Interact();
            InteractionUIManager.Instance.Hide();
        }
    }

    void ClearUI()
    {
        if (currentInteractable != null)
        {
            currentInteractable = null;
            InteractionUIManager.Instance.Hide();
        }
    }
}