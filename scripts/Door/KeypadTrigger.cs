using UnityEngine;

public class KeypadTrigger : MonoBehaviour
{
    public KeypadController3D keypad;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            keypad.ActivateKeypad();
        }
    }
}