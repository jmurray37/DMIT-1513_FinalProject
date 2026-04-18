using System.Collections;
using UnityEngine;

public class MorgueKeyJumpScare : MonoBehaviour
{
    [Header("Key Setup")]
    public KeyPickup keyPickup;
    public string requiredKeyId = "MorgueKey";

    [Header("Player References")]
    public Transform player;
    public Camera playerCamera;

    [Header("Hallucination")]
    public GameObject hallucinationPrefab;
    public float spawnDistance = 2.2f;
    public float verticalOffset = -0.5f;
    public float visibleTime = 0.4f;
    public bool facePlayer = true;

    [Header("Optional Audio")]
    public AudioSource scareAudioSource;
    public AudioClip scareClip;
    [Range(0f, 1f)]
    public float scareVolume = 1f;

    private bool hasTriggered = false;
    private bool previousKeyState = false;

    void Start()
    {
        if (keyPickup == null)
        {
            keyPickup = GetComponent<KeyPickup>();
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (player == null && playerCamera != null)
        {
            player = playerCamera.transform.root;
        }
    }

    void Update()
    {
        if (hasTriggered)
        {
            return;
        }

        if (player == null)
        {
            return;
        }

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            return;
        }

        bool hasKeyNow = inventory.HasKey(requiredKeyId);

        if (!previousKeyState && hasKeyNow)
        {
            previousKeyState = true;
            hasTriggered = true;
            TriggerJumpScare();
            return;
        }

        previousKeyState = hasKeyNow;
    }

    void TriggerJumpScare()
    {
        if (hallucinationPrefab == null || playerCamera == null)
        {
            return;
        }

        Vector3 spawnPosition =
            playerCamera.transform.position +
            (playerCamera.transform.forward * spawnDistance) +
            (Vector3.up * verticalOffset);

        GameObject hallucinationInstance = Instantiate(hallucinationPrefab, spawnPosition, Quaternion.identity);

        if (facePlayer && playerCamera != null)
        {
            Vector3 lookTarget = playerCamera.transform.position;
            lookTarget.y = hallucinationInstance.transform.position.y;
            hallucinationInstance.transform.LookAt(lookTarget);
        }

        Animator animator = hallucinationInstance.GetComponentInChildren<Animator>();

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        if (scareAudioSource != null && scareClip != null)
        {
            scareAudioSource.PlayOneShot(scareClip, scareVolume);
        }

        StartCoroutine(DestroyAfterDelay(hallucinationInstance, visibleTime));
    }

    IEnumerator DestroyAfterDelay(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (target != null)
        {
            Destroy(target);
        }
    }
}