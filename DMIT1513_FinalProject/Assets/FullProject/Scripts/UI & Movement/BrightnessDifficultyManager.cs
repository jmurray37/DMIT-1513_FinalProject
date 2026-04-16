using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessDifficultyManager : MonoBehaviour
{
    public static BrightnessDifficultyManager Instance { get; private set; }

    [Header("Master Value")]
    [Range(0f, 1f)]
    public float brightnessValue = 0.5f;

    [Header("Lighting")]
    public float minAmbientIntensity = 0.5f;
    public float maxAmbientIntensity = 3f;
    public Color darkAmbientColor = Color.black;
    public Color brightAmbientColor = Color.white;

    [Header("Scene Lights")]
    public bool autoFindLights = true;
    public bool useTaggedLightsOnly = false;
    public string brightnessLightTag = "AffectBrightness";
    public Light[] sceneLights;
    public float minSceneLightMultiplier = 0.6f;
    public float maxSceneLightMultiplier = 2.2f;

    [Header("Fill Light")]
    public Light fillLight;
    public float minFillIntensity = 0f;
    public float maxFillIntensity = 0.5f;

    [Header("Shadows")]
    public float minShadowStrength = 1f;
    public float maxShadowStrength = 0.2f;
    public LightShadows shadowMode = LightShadows.Soft;

    [Header("Fog")]
    public bool controlFog = true;
    public float darkFogDensity = 0.045f;
    public float brightFogDensity = 0.005f;

    [Header("Post Processing")]
    public Volume globalVolume;
    public float minExposure = 0f;
    public float maxExposure = 1f;
    public float darkContrast = 0f;
    public float brightContrast = -8f;
    public float darkSaturation = 0f;
    public float brightSaturation = -8f;

    [Header("Scare System")]
    public ScareEventController scareController;
    public float minScareInterval = 8f;
    public float maxScareInterval = 35f;
    public float minScareChance = 0.15f;
    public float maxScareChance = 0.85f;

    [Header("Monster Difficulty")]
    public List<MonsterAISettingsReceiver> monsters = new List<MonsterAISettingsReceiver>();

    [Header("Monster Speed")]
    public float minMonsterSpeed = 2f;
    public float maxMonsterSpeed = 5f;

    [Header("Monster Detection")]
    public float minDetectionRange = 6f;
    public float maxDetectionRange = 18f;

    [Header("Monster Chase")]
    public float minChaseTime = 4f;
    public float maxChaseTime = 12f;

    [Header("Monster Attack")]
    public float maxAttackCooldown = 2.5f;
    public float minAttackCooldown = 0.75f;

    [Header("Monster Search")]
    public float minSearchTime = 2f;
    public float maxSearchTime = 8f;

    private ColorAdjustments colorAdjustments;
    private readonly Dictionary<Light, float> baseLightIntensities = new Dictionary<Light, float>();

    public float FearIntensity => 1f - brightnessValue;
    public float MonsterAggression => brightnessValue;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        RefreshLights();
        CacheVolumeOverrides();
        ApplyAllSettings();
    }

    float GetAdjustedBrightness()
    {
        return Mathf.Pow(brightnessValue, 2f);
    }

    [ContextMenu("Refresh Lights")]
    public void RefreshLights()
    {
        baseLightIntensities.Clear();

        if (autoFindLights)
        {
            Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            List<Light> filteredLights = new List<Light>();

            for (int i = 0; i < allLights.Length; i++)
            {
                Light currentLight = allLights[i];

                if (currentLight == null)
                {
                    continue;
                }

                if (currentLight == fillLight)
                {
                    continue;
                }

                if (useTaggedLightsOnly)
                {
                    if (currentLight.CompareTag(brightnessLightTag))
                    {
                        filteredLights.Add(currentLight);
                    }
                }
                else
                {
                    filteredLights.Add(currentLight);
                }
            }

            sceneLights = filteredLights.ToArray();
        }

        if (sceneLights == null)
        {
            return;
        }

        for (int i = 0; i < sceneLights.Length; i++)
        {
            Light currentLight = sceneLights[i];

            if (currentLight != null && !baseLightIntensities.ContainsKey(currentLight))
            {
                baseLightIntensities.Add(currentLight, currentLight.intensity);
            }
        }
    }

    void CacheVolumeOverrides()
    {
        colorAdjustments = null;

        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out colorAdjustments);
        }
    }

    public void SetBrightnessDifficulty(float newValue)
    {
        brightnessValue = Mathf.Clamp01(newValue);
        ApplyAllSettings();
    }

    [ContextMenu("Apply All Settings")]
    public void ApplyAllSettings()
    {
        if (autoFindLights && (sceneLights == null || sceneLights.Length == 0))
        {
            RefreshLights();
        }

        if (colorAdjustments == null)
        {
            CacheVolumeOverrides();
        }

        ApplyLighting();
        ApplyScares();
        ApplyMonsters();
    }

    void ApplyLighting()
    {
        float t = GetAdjustedBrightness();

        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientIntensity = Mathf.Lerp(minAmbientIntensity, maxAmbientIntensity, t);
        RenderSettings.ambientLight = Color.Lerp(darkAmbientColor, brightAmbientColor, t);

        if (controlFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogDensity = Mathf.Lerp(darkFogDensity, brightFogDensity, t);
        }

        if (fillLight != null)
        {
            fillLight.intensity = Mathf.Lerp(minFillIntensity, maxFillIntensity, t);
            fillLight.shadows = LightShadows.None;
        }

        if (sceneLights != null && sceneLights.Length > 0)
        {
            float lightMultiplier = Mathf.Lerp(minSceneLightMultiplier, maxSceneLightMultiplier, t);
            float shadowStrength = Mathf.Lerp(minShadowStrength, maxShadowStrength, t);

            for (int i = 0; i < sceneLights.Length; i++)
            {
                Light currentLight = sceneLights[i];

                if (currentLight == null)
                {
                    continue;
                }

                if (!baseLightIntensities.ContainsKey(currentLight))
                {
                    baseLightIntensities[currentLight] = currentLight.intensity;
                }

                currentLight.intensity = baseLightIntensities[currentLight] * lightMultiplier;
                currentLight.shadows = shadowMode;
                currentLight.shadowStrength = shadowStrength;
            }
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = Mathf.Lerp(minExposure, maxExposure, t);
            colorAdjustments.contrast.value = Mathf.Lerp(darkContrast, brightContrast, t);
            colorAdjustments.saturation.value = Mathf.Lerp(darkSaturation, brightSaturation, t);
        }
    }

    void ApplyScares()
    {
        if (scareController == null)
        {
            return;
        }

        float darkness = FearIntensity;

        scareController.scareChance = Mathf.Lerp(minScareChance, maxScareChance, darkness);
        scareController.timeBetweenScareChecks = Mathf.Lerp(maxScareInterval, minScareInterval, darkness);
        scareController.scareIntensity = darkness;
    }

    void ApplyMonsters()
    {
        if (monsters == null || monsters.Count == 0)
        {
            return;
        }

        float aggression = MonsterAggression;

        float moveSpeed = Mathf.Lerp(minMonsterSpeed, maxMonsterSpeed, aggression);
        float detectionRange = Mathf.Lerp(minDetectionRange, maxDetectionRange, aggression);
        float chaseTime = Mathf.Lerp(minChaseTime, maxChaseTime, aggression);
        float attackCooldown = Mathf.Lerp(maxAttackCooldown, minAttackCooldown, aggression);
        float searchTime = Mathf.Lerp(minSearchTime, maxSearchTime, aggression);

        for (int i = 0; i < monsters.Count; i++)
        {
            if (monsters[i] != null)
            {
                monsters[i].ApplyDifficultySettings(moveSpeed, detectionRange, chaseTime, attackCooldown, searchTime);
            }
        }
    }
}