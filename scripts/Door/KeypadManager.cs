using UnityEngine;

public class KeypadManager : MonoBehaviour
{
    public static KeypadManager Instance;

    private KeypadController3D activeKeypad;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OpenKeypad(Door door)
    {
        activeKeypad = FindKeypadForDoor(door);

        if (activeKeypad != null)
        {
            activeKeypad.connectedDoor = door;
            activeKeypad.ActivateKeypad(); 
        }
        else
        {
            Debug.LogWarning("Nu am gasit un KeypadController3D pentru usa data.");
        }
    }

    private KeypadController3D FindKeypadForDoor(Door door)
    {
        KeypadController3D[] keypads = FindObjectsOfType<KeypadController3D>();
        foreach (var kp in keypads)
        {
            if (kp.connectedDoor == door)
                return kp;
        }

        return null;
    }
}