using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoorEndGame : MonoBehaviour, IInteractable
{
    [Header("Requirements")]
    public string requiredKeyId = "ExitKey";

    [Header("Scenes")]
    public string titleSceneName = "Title";

    [Header("Optional Audio")]
    public AudioSource audioSource;
    public AudioClip lockedSound;
    public AudioClip escapeSound;
    [Range(0f, 1f)]
    public float lockedVolume = 1f;
    [Range(0f, 1f)]
    public float escapeVolume = 1f;

    private bool hasEscaped = false;

    public void Interact(PlayerInteractor interactor)
    {
        if (hasEscaped)
        {
            return;
        }

        if (interactor == null || interactor.inventory == null)
        {
            return;
        }

        if (!interactor.inventory.HasKey(requiredKeyId))
        {
            if (audioSource != null && lockedSound != null)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(lockedSound, lockedVolume);
            }

            Debug.Log("Exit door is locked.");
            return;
        }

        hasEscaped = true;
        EndGame();
    }

    public string GetInteractText(PlayerInteractor interactor)
    {
        if (hasEscaped)
        {
            return "";
        }

        if (interactor != null && interactor.inventory != null && interactor.inventory.HasKey(requiredKeyId))
        {
            return "Escape";
        }

        return "Exit door is locked";
    }

    void EndGame()
    {
        if (audioSource != null && escapeSound != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(escapeSound, escapeVolume);
        }

        // IMPORTANT: reset time/audio before scene load
        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene(titleSceneName);

        Debug.Log("Player escaped → loading title");
    }
}