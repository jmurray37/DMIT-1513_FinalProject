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

    [Header("Audio Scares")]
    public List<AudioSource> whisperSounds = new List<AudioSource>();
    public List<AudioSource> bangSounds = new List<AudioSource>();

    [Header("Visual Scares")]
    public List<GameObject> flickerObjects = new List<GameObject>();
    public List<GameObject> apparitionObjects = new List<GameObject>();

    Coroutine scareLoop;

    void OnEnable()
    {
        scareLoop = StartCoroutine(ScareLoop());
    }

    void OnDisable()
    {
        if (scareLoop != null)
        {
            StopCoroutine(scareLoop);
        }
    }

    IEnumerator ScareLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenScareChecks);

            float roll = Random.value;
            if (roll <= scareChance)
            {
                TriggerRandomScare();
            }
        }
    }

    public void TriggerRandomScare()
    {
        int scareType = Random.Range(0, 4);

        if (scareType == 0)
        {
            PlayRandomAudio(whisperSounds);
        }
        else if (scareType == 1)
        {
            PlayRandomAudio(bangSounds);
        }
        else if (scareType == 2)
        {
            TriggerRandomVisual(flickerObjects, 0.25f + scareIntensity);
        }
        else
        {
            TriggerRandomVisual(apparitionObjects, 0.5f + scareIntensity);
        }
    }

    void PlayRandomAudio(List<AudioSource> pool)
    {
        if (pool == null || pool.Count == 0)
        {
            return;
        }

        int index = Random.Range(0, pool.Count);
        if (pool[index] != null)
        {
            pool[index].Play();
        }
    }

    void TriggerRandomVisual(List<GameObject> pool, float activeTime)
    {
        if (pool == null || pool.Count == 0)
        {
            return;
        }

        int index = Random.Range(0, pool.Count);
        if (pool[index] != null)
        {
            StartCoroutine(FlashObject(pool[index], activeTime));
        }
    }

    IEnumerator FlashObject(GameObject target, float time)
    {
        target.SetActive(true);
        yield return new WaitForSeconds(time);
        target.SetActive(false);
    }
}