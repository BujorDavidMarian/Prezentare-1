using TMPro;
using UnityEngine;

public class KeypadController3D : MonoBehaviour
{
    public Transform[] buttonPositions;
    public Transform highlight3D;
    public TextMeshPro displayText;
    public Door connectedDoor;
    public MonoBehaviour playerController;
    public Transform keypadCameraPosition;  
    public GameObject playerBody;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    public Camera targetCamera;
    private float originalFOV;

    public string interactionPrompt = "Press E to use the keypad";
    public string correctCode = "123456";

    private string input = "";
    private int selectedIndex = 0;
    private bool isActive = false;
    private float highlightScaleMultiplier = 1.2f;

    public Canvas mainCanvas;
    public GameObject player;
    public float feedbackDuration = 1.5f;
    private bool playerNearby = false;

    private Coroutine feedbackRoutine;

    public GameObject buttonClickSoundObject;
    public GameObject correctSoundObject;
    public GameObject errorSoundObject;

    void Update()
    {
        if (!isActive) return;

        if (Input.GetKeyDown(KeyCode.W)) selectedIndex = (selectedIndex - 3 + 12) % 12;
        if (Input.GetKeyDown(KeyCode.S)) selectedIndex = (selectedIndex + 3) % 12;
        if (Input.GetKeyDown(KeyCode.A)) selectedIndex = (selectedIndex - 1 + 12) % 12;
        if (Input.GetKeyDown(KeyCode.D)) selectedIndex = (selectedIndex + 1) % 12;

        MoveHighlight();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            HandleButton(selectedIndex);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeactivateKeypad();
        }

        if (!isActive && playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ActivateKeypad();
        }
    }
    void PlaySound(GameObject soundObject)
    {
        if (soundObject != null)
        {
            AudioSource source = soundObject.GetComponent<AudioSource>();
            if (source != null)
            {
                source.Play();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            if (!isActive)
            {
                InteractionUIManager.Instance.Show(interactionPrompt);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            InteractionUIManager.Instance.Hide();
        }
    }

    void MoveHighlight()
    {
        if (selectedIndex >= 0 && selectedIndex < buttonPositions.Length)
        {
            highlight3D.position = buttonPositions[selectedIndex].position;

            Vector3 targetScale = buttonPositions[selectedIndex].localScale * highlightScaleMultiplier;
            highlight3D.localScale = targetScale;
        }
    }

    void HandleButton(int index)
    {
        if (index >= 0 && index <= 8) 
        {
            AddDigit((index + 1).ToString());
        }
        else if (index == 9) 
        {
            RemoveLastDigit();
        }
        else if (index == 10) 
        {
            AddDigit("0");
        }
        else if (index == 11) 
        {
            CheckCode();
        }
    }

    void AddDigit(string digit)
    {
        if (input.Length < 6)
        {
            input += digit;
            displayText.text = input;
            PlaySound(buttonClickSoundObject);
        }
    }

    void RemoveLastDigit()
    {
        if (input.Length > 0)
        {
            input = input.Substring(0, input.Length - 1);
            displayText.text = input;
            PlaySound(buttonClickSoundObject);
        }
    }

    void CheckCode()
    {
        if (input == correctCode)
        {
            PlaySound(correctSoundObject);
            ShowFeedback("CORRECT", Color.green, true);
            connectedDoor.SetUnlockedPermanently();
            connectedDoor.Open(transform.position);
            Invoke(nameof(DeactivateKeypad), feedbackDuration);
        }
        else
        {
            PlaySound(errorSoundObject);
            ShowFeedback("ERROR", Color.red, false);
            input = "";
        }
    }

    void ShowFeedback(string message, Color color, bool flicker)
    {
        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackRoutine = StartCoroutine(ShowFeedbackRoutine(message, color, flicker));
    }

    System.Collections.IEnumerator ShowFeedbackRoutine(string message, Color color, bool flicker)
    {
        float timer = 0f;
        float flickerSpeed = 0.2f;

        while (timer < feedbackDuration)
        {
            if (flicker)
            {
                displayText.text = (Mathf.FloorToInt(timer / flickerSpeed) % 2 == 0) ? message : "";
            }
            else
            {
                displayText.text = message;
            }

            displayText.color = color;
            timer += Time.deltaTime;
            yield return null;
        }

        displayText.text = "";
        displayText.color = Color.white;
    }

    public void ActivateKeypad()
    {
        isActive = true;
        input = "";
        displayText.text = "";
        selectedIndex = 0;
        MoveHighlight();

        highlight3D.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerBody != null)
            playerBody.SetActive(false);

        if (playerController != null)
            playerController.enabled = false;

        if (targetCamera != null && keypadCameraPosition != null)
        {
            originalCamPos = targetCamera.transform.position;
            originalCamRot = targetCamera.transform.rotation;
            originalFOV = targetCamera.fieldOfView;

            targetCamera.transform.SetPositionAndRotation(keypadCameraPosition.position, keypadCameraPosition.rotation);
            targetCamera.fieldOfView = 100f;
        }

        if (mainCanvas != null)
            mainCanvas.enabled = false;

        playerNearby = false;
        InteractionUIManager.Instance.Hide();
    }

    public void DeactivateKeypad()
    {
        isActive = false;
        highlight3D.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerBody != null)
            playerBody.SetActive(true);

        if (playerController != null)
            playerController.enabled = true;

        if (targetCamera != null)
        {
            targetCamera.transform.SetPositionAndRotation(originalCamPos, originalCamRot);
            targetCamera.fieldOfView = originalFOV; 
        }

        if (mainCanvas != null)
            mainCanvas.enabled = true;

        if (playerNearby)
        {
            InteractionUIManager.Instance.Show(interactionPrompt);
        }
    }

    public string GetDescription()
    {
        return interactionPrompt;
    }
}
