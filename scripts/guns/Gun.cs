using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI; 

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 1f;

    public Camera fpsCam;
    public Image gunImage;
    public GameObject soundSourceObject;
    public Sprite[] gunFireAnimationSprites;
    public GameObject impactEffect;
    public GameObject enemyImpactEffect;


    public GameObject inventoryScreen1;
    public GameObject inventoryScreen2;

    private float nextTimeToFire = 0f;
    private Coroutine m_CoroutineAnim;
    private AudioSource audioSource;

    public int magazineSize = 12;
    public int currentAmmo;
    public int totalAmmo; 
    public KeyCode reloadKey = KeyCode.R;
    public float reloadTime = 2f;
    private bool isReloading = false;
    private InventoryManager inventoryManager;
    public int bulletItemID = 4;

    void Start()
    {
        currentAmmo = magazineSize;
        if (soundSourceObject != null)
            audioSource = soundSourceObject.GetComponent<AudioSource>();
        inventoryManager = Object.FindFirstObjectByType<InventoryManager>();

        SyncAmmoFromInventory();  
    }

    void Update()
    {
        if (isReloading)
            return;

        SyncAmmoFromInventory();

        if (Input.GetKeyDown(reloadKey))
        {
            StartCoroutine(Reload());
            return;
        }

        if (!inventoryScreen1.activeSelf && !inventoryScreen2.activeSelf)
        {
            if (currentAmmo <= 0)
            {
                Debug.Log("Nu mai ai gloante! Apasa R pentru reload.");
                return;
            }
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
                StartGunAnimation();
                PlayGunshotSound();
            }
        }
    }


    /*void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            enemy Enemy = hit.transform.GetComponent<enemy>();

            if (Enemy != null)
            {
                Enemy.TakeDamage(damage);
                GameObject impactGO = Instantiate(enemyImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 0.2f);
            }
            else
            {
                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }*/

    void Shoot()
    {
        currentAmmo--;

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            enemy Enemy = hit.transform.GetComponent<enemy>();

            if (Enemy != null)
            {
                Enemy.TakeDamage(damage);
                GameObject impactGO = Instantiate(enemyImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 0.2f);
            }
            else
            {
                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }
    IEnumerator Reload()
    {
        if (isReloading || currentAmmo == magazineSize || totalAmmo <= 0)
            yield break;

        Debug.Log("Se reincarca...");
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        int needed = magazineSize - currentAmmo;
        int ammoToReload = Mathf.Min(needed, totalAmmo);

        totalAmmo -= ammoToReload;
        currentAmmo += ammoToReload;

        Debug.Log($"Reload complet: {currentAmmo}/{magazineSize}, Total ramase: {totalAmmo}");

        for (int i = 0; i < inventoryManager.slots.Count; i++)
        {
            Transform slot = inventoryManager.slots[i];
            if (slot.childCount > 0)
            {
                InventoryItem invItem = slot.GetChild(0).GetComponent<InventoryItem>();
                if (invItem != null && invItem.itemData.ID == bulletItemID)
                {
                    invItem.amount -= ammoToReload;
                    invItem.UpdateUI();  
                    break;
                }
            }
        }

        isReloading = false;
    }

    void StartGunAnimation()
    {
        if (gunFireAnimationSprites.Length > 0 && m_CoroutineAnim == null)
            m_CoroutineAnim = StartCoroutine(PlayGunFireAnimation());
    }

    void PlayGunshotSound()
    {
        if (audioSource != null)
            audioSource.Play();
    }

    IEnumerator PlayGunFireAnimation()
    {
        int spriteIndex = 0;
        float waitTime = 0.06f;

        while (spriteIndex < gunFireAnimationSprites.Length)
        {
            gunImage.sprite = gunFireAnimationSprites[spriteIndex];
            spriteIndex++;
            yield return new WaitForSeconds(waitTime);
        }

        gunImage.sprite = gunFireAnimationSprites[0];
        m_CoroutineAnim = null;
    }

    void SyncAmmoFromInventory()
    {
        inventoryManager = Object.FindFirstObjectByType<InventoryManager>();
        if (inventoryManager == null) return;

        foreach (Transform slot in inventoryManager.slots)
        {
            if (slot.childCount > 0)
            {
                InventoryItem item = slot.GetChild(0).GetComponent<InventoryItem>();
                if (item != null && item.itemData.ID == bulletItemID)
                {
                    totalAmmo = item.amount;
                    return;
                }
            }
        }
    }

}