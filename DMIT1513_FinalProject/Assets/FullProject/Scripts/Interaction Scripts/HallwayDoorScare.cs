using UnityEngine;

public class HallwayDoorScare : MonoBehaviour
{
    [Header("Target")]
    public string playerTag = "Player";

    [Header("Audio")]
    public AudioSource scratchSource;
    public AudioSource zombieSource;

    [Header("Timing")]
    public float firstDelay = 0.1f;
    public float secondDelay = 0.6f;

    [Header("Behavior")]
    public bool scratchFirst = true;
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (triggerOnce && hasTriggered)
        {
            return;
        }

        hasTriggered = true;

        if (scratchFirst)
        {
            Invoke(nameof(PlayScratch), firstDelay);
            Invoke(nameof(PlayZombie), secondDelay);
        }
        else
        {
            Invoke(nameof(PlayZombie), firstDelay);
            Invoke(nameof(PlayScratch), secondDelay);
        }
    }

    void PlayScratch()
    {
        if (scratchSource != null)
        {
            scratchSource.pitch = Random.Range(0.95f, 1.05f);
            scratchSource.Play();
        }
    }

    void PlayZombie()
    {
        if (zombieSource != null)
        {
            zombieSource.pitch = Random.Range(0.9f, 1.1f);
            zombieSource.Play();
        }
    }
}