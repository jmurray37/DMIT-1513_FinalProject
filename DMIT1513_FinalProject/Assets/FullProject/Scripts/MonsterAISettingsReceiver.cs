using UnityEngine;

public class MonsterAISettingsReceiver : MonoBehaviour
{
    public float moveSpeed;
    public float detectionRange;
    public float chaseTime;
    public float attackCooldown;
    public float searchTime;

    public void ApplyDifficultySettings(
        float newMoveSpeed,
        float newDetectionRange,
        float newChaseTime,
        float newAttackCooldown,
        float newSearchTime)
    {
        moveSpeed = newMoveSpeed;
        detectionRange = newDetectionRange;
        chaseTime = newChaseTime;
        attackCooldown = newAttackCooldown;
        searchTime = newSearchTime;
    }
}