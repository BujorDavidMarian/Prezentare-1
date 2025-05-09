using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryControl : MonoBehaviour
{
    public GameObject inventoryScreen;
    public AudioSource inventoryOpen;
    public bool isOpen = false;
    public AudioSource inventoryClose;
    public bool canClose = false;
     void Start()
    {
        inventoryScreen.SetActive(false);  
        isOpen = false;
        canClose = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && isOpen == false && canClose == false)
        {
            isOpen = true;
            inventoryOpen.Play();
            StartCoroutine(InvControl());
        }
        if (Input.GetKeyDown(KeyCode.Tab) && isOpen == true && canClose == true)
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            isOpen = false;
            inventoryClose.Play();
            StartCoroutine(InvControl());
        }
    }

    IEnumerator InvControl()
    {
        yield return new WaitForSeconds(0.50f);
        if(isOpen == true)
        {
            inventoryScreen.SetActive(true);
        }
        else
        {
            inventoryScreen.SetActive(false);

            if (ItemContextMenu.Instance != null)
            {
                ItemContextMenu.Instance.Close();
            }
        }
        yield return new WaitForSeconds(0.50f);
        if(isOpen == true)
        {
            canClose = true;
            Time.timeScale = 0;
            Cursor.visible = true;
        }
        else
        {
            canClose = false;
            Time.timeScale = 1;
            Cursor.visible = false;
        }

    }
}
