using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public float currentHealth = 3f;

    [Header("Regeneration")]
    public bool canRegenerate = true;
    public float regenDelayAfterHit = 5f;
    public float regenPerSecond = 0.35f;

    [Header("UI")]
    public GameObject deathScreenUI;

    [Header("Optional Audio")]
    public AudioSource audioSource;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    [Range(0f, 1f)]
    public float hurtVolume = 1f;
    [Range(0f, 1f)]
    public float deathVolume = 1f;

    [Header("Optional Player Control")]
    public FirstPersonMovement movementScript;
    public FirstPersonLook lookScript;

    private bool isDead = false;
    private float lastHitTime = -999f;

    public float HealthPercent
    {
        get
        {
            if (maxHealth <= 0)
            {
                return 0f;
            }

            return Mathf.Clamp01(currentHealth / maxHealth);
        }
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        HandleRegeneration();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        if (damage <= 0)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth < 0f)
        {
            currentHealth = 0f;
        }

        lastHitTime = Time.time;

        if (audioSource != null && hurtSound != null && currentHealth > 0f)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(hurtSound, hurtVolume);
        }

        Debug.Log("Player took damage. Health: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void HandleRegeneration()
    {
        if (!canRegenerate)
        {
            return;
        }

        if (currentHealth >= maxHealth)
        {
            return;
        }

        if (Time.time < lastHitTime + regenDelayAfterHit)
        {
            return;
        }

        currentHealth += regenPerSecond * Time.deltaTime;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        currentHealth = 0f;

        if (audioSource != null && deathSound != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(deathSound, deathVolume);
        }

        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        if (lookScript != null)
        {
            lookScript.enabled = false;
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AudioListener.pause = true;

        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);
        }

        Debug.Log("Player died.");
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToTitle(string titleSceneName)
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(titleSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Application.Quit();
    }
}