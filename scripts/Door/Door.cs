using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public enum DoorAccessType { Unlocked, LockedWithKey, LockedWithCode }
    public DoorAccessType accessType = DoorAccessType.Unlocked;

    float targetYRotation;
    float defaultYRotation = 0f;
    float timer = 0f;

    public float smooth = 5f;
    public bool autoClose = true;
    public Transform pivot;

    public int requiredKeyID = -1;

    bool isOpen;
    public bool IsLocked = false;
    bool isPermanentlyUnlocked = false;

    public KeypadController3D keypadController;

    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        defaultYRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        pivot.rotation = Quaternion.Lerp(pivot.rotation, Quaternion.Euler(0f, defaultYRotation + targetYRotation, 0f), smooth * Time.deltaTime);

        timer -= Time.deltaTime;

        if (timer <= 0f && isOpen && autoClose)
        {
            ToggleDoor(player.position);
        }
    }

    public void ToggleDoor(Vector3 pos)
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            Vector3 dir = (pos - transform.position);
            targetYRotation = Mathf.Sign(Vector3.Dot(transform.right, dir)) * 90f;
            timer = 5f;
        }
        else
        {
            targetYRotation = 0f;
        }
    }

    public void Open(Vector3 pos)
    {
        if (!isOpen)
        {
            ToggleDoor(pos);
        }
    }

    public void Close(Vector3 pos)
    {
        if (isOpen)
        {
            ToggleDoor(pos);
        }
    }

    public bool TryUnlock(Item selectedKey)
    {
        if (accessType == DoorAccessType.LockedWithKey && selectedKey != null && selectedKey.ID == requiredKeyID)
        {
            IsLocked = false;
            accessType = DoorAccessType.Unlocked;
            ToggleDoor(player.position);
            return true;
        }

        Debug.Log("Incorrect key!");
        return false;
    }

    public void SetUnlockedPermanently()
    {
        accessType = DoorAccessType.Unlocked;
        isPermanentlyUnlocked = true;
    }

    public void Interact()
    {
        
        if (accessType == DoorAccessType.LockedWithCode && keypadController != null)
        {
            InteractionUIManager.Instance.Show("Door needs keypad code to unlock");
        }
        else if (IsLocked && accessType != DoorAccessType.LockedWithCode) 
        {
            KeyItemUIManager.Instance.ShowKeyItemSelection(this); 
        }
        else
        {
            ToggleDoor(player.position); 
        }
    }

    public string GetDescription()
    {
        if (!isOpen && keypadController != null && accessType == DoorAccessType.LockedWithCode)
            return "Door needs keypad code to unlock";
        if (!isOpen && requiredKeyID != -1 && accessType == DoorAccessType.LockedWithKey)
            return "Door is locked. Requires a key";
        return isOpen ? "Press E to close the door" : "Press E to open the door";
    }
}