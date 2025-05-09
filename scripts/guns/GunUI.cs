using UnityEngine;
using TMPro;

public class GunUI : MonoBehaviour
{
    public GameObject gunObject;
    public TextMeshProUGUI ammoText;
    public GameObject ammoPanel;

    private Gun gunScript;

    private void Start()
    {
        ammoPanel.SetActive(false);
        gunScript = null;
    }

    private void Update()
    {
        if (gunObject != null && gunObject.activeSelf)
        {
            if (gunScript == null)
            {
                gunScript = gunObject.GetComponent<Gun>();
                if (gunScript == null)
                {
                    Debug.LogWarning("Gun component missing on gunObject!");
                    return;
                }
            }

            if (!ammoPanel.activeSelf)
                ammoPanel.SetActive(true);

            UpdateUI();
        }
        else
        {
            if (ammoPanel.activeSelf)
                ammoPanel.SetActive(false);

            gunScript = null;
        }
    }

    private void UpdateUI()
    {
        if (gunScript != null)
        {
            ammoText.text = $"{gunScript.currentAmmo}/{gunScript.totalAmmo}";
        }
    }

    public void SetGun(GameObject newGun)
    {
        gunObject = newGun;
        gunScript = null;
        if (gunObject != null && gunObject.activeSelf)
        {
            gunScript = gunObject.GetComponent<Gun>();
            if (gunScript != null)
            {
                ammoPanel.SetActive(true);
                UpdateUI();
            }
        }
        else
        {
            ammoPanel.SetActive(false);
        }
    }
}