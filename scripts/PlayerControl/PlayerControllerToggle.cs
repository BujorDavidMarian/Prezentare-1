using UnityEngine;

public class PlayerControlToggle : MonoBehaviour
{
    public MonoBehaviour playerController;
    public bool isControlEnabled = true;
    private float lastToggleTime = 0f;
    public float toggleCooldown = 0.50f;

    public bool IsControlEnabled()
    {
        return isControlEnabled;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Time.time - lastToggleTime >= toggleCooldown)
        {
            isControlEnabled = !isControlEnabled;
            lastToggleTime = Time.time;

            if (playerController != null)
            {
                playerController.enabled = isControlEnabled;
            }
            Cursor.visible = !isControlEnabled;
            Cursor.lockState = isControlEnabled ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    public void ToggleControl(bool enable)
    {
        isControlEnabled = enable;

        if (playerController != null)
            playerController.enabled = isControlEnabled;

        Cursor.visible = !isControlEnabled;
        Cursor.lockState = isControlEnabled ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
