using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;

    public GameObject canvas;               
    public GameObject handWithClock;          
    public GameObject deathScreen;            
    public AudioSource deathSound;
    public TextMeshProUGUI deathText;


    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.L))
        {
            TakeDamage(25);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        healthBar.SetHealth(currentHealth);

        Debug.Log($"Player a luat {damage} damage. Viata ramasa: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {

        handWithClock.SetActive(true);

        foreach (Transform child in canvas.transform)
        {
            if (child.gameObject != handWithClock)
            {
                child.gameObject.SetActive(false);
            }
        }

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(3f);

        deathScreen.SetActive(true);

        if (deathSound != null)
        {
            deathSound.Play();
        }

        StartCoroutine(FadeInText(deathText));
    }

    private IEnumerator FadeInText(TextMeshProUGUI text)
    {
        float time = 0f;
        Color startColor = text.color;
        startColor.a = 0f;  
        text.color = startColor;

        while (time < 1f)
        {
            time += Time.deltaTime / 2f; 
            startColor.a = Mathf.Lerp(0f, 1f, time);
            text.color = startColor;
            yield return null;
        }

        startColor.a = 1f;
        text.color = startColor;
    }

    public void Heal(int healingAmount)
    {
        currentHealth += healingAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healthBar.SetHealth(currentHealth);

        Debug.Log($"Vindecare: {healingAmount} HP. Viata curenta: {currentHealth}/{maxHealth}");
    }
}
