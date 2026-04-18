using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareEventController : MonoBehaviour
{
    [Header("Scare Tuning")]
    [Range(0f, 1f)]
    public float scareChance = 0.5f;
    public float timeBetweenScareChecks = 15f;
    [Range(0f, 1f)]
    public float scareIntensity = 0.5f;
    public float minimumTimeBetweenChecks = 15f;

    [Header("Player Audio")]
    public AudioSource playerScareAudioSource;

    [Header("Audio Clips")]
    public List<AudioClip> whisperClips = new List<AudioClip>();
    public List<AudioClip> bangClips = new List<AudioClip>();

    [Header("Audio Variation")]
    public bool avoidImmediateRepeat = true;
    public Vector2 whisperPitchRange = new Vector2(0.95f, 1.05f);
    public Vector2 bangPitchRange = new Vector2(0.95f, 1.05f);
    public Vector2 whisperVolumeRange = new Vector2(0.75f, 1f);
    public Vector2 bangVolumeRange = new Vector2(0.85f, 1f);

    private Coroutine scareLoop;
    private int lastWhisperIndex = -1;
    private int lastBangIndex = -1;

    void OnEnable()
    {
        scareLoop = StartCoroutine(ScareLoop());
    }

    void OnDisable()
    {
        if (scareLoop != null)
        {
            StopCoroutine(scareLoop);
            scareLoop = null;
        }
    }

    IEnumerator ScareLoop()
    {
        while (true)
        {
            float waitTime = Mathf.Max(minimumTimeBetweenChecks, timeBetweenScareChecks);
            yield return new WaitForSeconds(waitTime);

            if (Random.value <= scareChance)
            {
                TriggerRandomScare();
            }
        }
    }

    public void TriggerRandomScare()
    {
        List<int> availableTypes = new List<int>();

        bool canPlayPlayerAudio = playerScareAudioSource != null && !playerScareAudioSource.isPlaying;

        if (HasValidClips(whisperClips) && canPlayPlayerAudio)
        {
            availableTypes.Add(0);
        }

        if (HasValidClips(bangClips) && canPlayPlayerAudio)
        {
            availableTypes.Add(1);
        }

        if (availableTypes.Count == 0)
        {
            return;
        }

        int chosenType = availableTypes[Random.Range(0, availableTypes.Count)];

        if (chosenType == 0)
        {
            PlayRandomClip(whisperClips, ref lastWhisperIndex, whisperPitchRange, whisperVolumeRange);
        }
        else
        {
            PlayRandomClip(bangClips, ref lastBangIndex, bangPitchRange, bangVolumeRange);
        }
    }

    public void TriggerWhisperScare()
    {
        PlayRandomClip(whisperClips, ref lastWhisperIndex, whisperPitchRange, whisperVolumeRange);
    }

    public void TriggerBangScare()
    {
        PlayRandomClip(bangClips, ref lastBangIndex, bangPitchRange, bangVolumeRange);
    }

    public void TriggerSpecificScare(AudioClip clip, float volume = 1f, float minPitch = 0.95f, float maxPitch = 1.05f)
    {
        if (playerScareAudioSource == null || clip == null)
        {
            return;
        }

        if (playerScareAudioSource.isPlaying)
        {
            return;
        }

        playerScareAudioSource.pitch = Random.Range(minPitch, maxPitch);
        playerScareAudioSource.PlayOneShot(clip, volume);
    }

    bool HasValidClips(List<AudioClip> pool)
    {
        if (pool == null || pool.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] != null)
            {
                return true;
            }
        }

        return false;
    }

    void PlayRandomClip(
        List<AudioClip> pool,
        ref int lastIndex,
        Vector2 pitchRange,
        Vector2 volumeRange)
    {
        if (playerScareAudioSource == null || pool == null || pool.Count == 0)
        {
            return;
        }

        if (playerScareAudioSource.isPlaying)
        {
            return;
        }

        int chosenIndex = GetRandomValidIndex(pool, lastIndex);
        if (chosenIndex < 0)
        {
            return;
        }

        AudioClip clip = pool[chosenIndex];
        if (clip == null)
        {
            return;
        }

        playerScareAudioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        playerScareAudioSource.PlayOneShot(clip, Random.Range(volumeRange.x, volumeRange.y));

        lastIndex = chosenIndex;
    }

    int GetRandomValidIndex<T>(List<T> pool, int lastIndex) where T : Object
    {
        List<int> validIndices = new List<int>();

        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] != null)
            {
                validIndices.Add(i);
            }
        }

        if (validIndices.Count == 0)
        {
            return -1;
        }

        if (!avoidImmediateRepeat || validIndices.Count == 1)
        {
            return validIndices[Random.Range(0, validIndices.Count)];
        }

        int chosenIndex = validIndices[Random.Range(0, validIndices.Count)];

        while (chosenIndex == lastIndex)
        {
            chosenIndex = validIndices[Random.Range(0, validIndices.Count)];
        }

        return chosenIndex;
    }
}