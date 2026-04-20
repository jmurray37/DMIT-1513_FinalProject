using UnityEngine;

public class EnemyModelSwapper : MonoBehaviour
{
    [Header("Normal Visuals")]
    public GameObject normalModelRoot;
    public Renderer[] normalRenderers;

    [Header("Monster Can Visuals")]
    public Transform visualRoot;
    public GameObject monsterCanPrefab;
    public Vector3 canLocalPosition = Vector3.zero;
    public Vector3 canLocalRotation = Vector3.zero;
    public Vector3 canLocalScale = Vector3.one;

    [Header("Options")]
    public bool disableWholeNormalModelRoot = false;

    private GameObject spawnedCanInstance;

    void Start()
    {
        if (!TitleScreenController.IsScaredOfMonsterEnabled())
        {
            return;
        }

        ApplyMonsterCanMode();
    }

    void ApplyMonsterCanMode()
    {
        if (disableWholeNormalModelRoot)
        {
            if (normalModelRoot != null)
            {
                normalModelRoot.SetActive(false);
            }
        }
        else
        {
            SetNormalRenderersVisible(false);
        }

        if (monsterCanPrefab == null)
        {
            Debug.LogWarning("EnemyModelSwapper on " + gameObject.name + " is missing a monsterCanPrefab.");
            return;
        }

        Transform parentToUse = visualRoot != null ? visualRoot : transform;

        spawnedCanInstance = Instantiate(monsterCanPrefab, parentToUse);
        spawnedCanInstance.transform.localPosition = canLocalPosition;
        spawnedCanInstance.transform.localEulerAngles = canLocalRotation;
        spawnedCanInstance.transform.localScale = canLocalScale;
    }

    void SetNormalRenderersVisible(bool visible)
    {
        if (normalRenderers == null || normalRenderers.Length == 0)
        {
            return;
        }

        for (int i = 0; i < normalRenderers.Length; i++)
        {
            if (normalRenderers[i] != null)
            {
                normalRenderers[i].enabled = visible;
            }
        }
    }
}