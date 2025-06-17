using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusController : BaseEnemyController
{
    // ▼ 設定：移動・探索
    [Header("足を投げる範囲円の半径")][SerializeField] private float throwRadius = 16f;

    // ▼ 設定：足を投げる
    [Header("足の飛ぶスピード")][SerializeField] private float legThrowSpeed = 10f;
    [Header("次の足が発射されるまでのクールダウン")][SerializeField] private float throwCooldown = 2f;

    // ▼ 設定：足の生成
    [Header("投げる足のスタート地点")][SerializeField] private Transform legSpawnPoint;
    [Header("足のプレハブ")][SerializeField] private OctopusLegController legPrefab;
    [Header("消す足のオブジェクトリスト")][SerializeField] private List<GameObject> legs = new List<GameObject>();

    // ▼ 状態管理
    private bool isPlayerInThrowRange = false;
    private bool isThrowing = false;
    private bool canThrow = true;

    private Animator animator;
    private const float attackAnimationDelay = 0.5f;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
    }

    public override void EnemyUpdate()
    {
        isPlayerInThrowRange = CanThrowToPlayer();

        // 投擲モード
        if (isPlayerInThrowRange && canThrow && !isThrowing)
        {
            RotateTowardsPlayer(); // プレイヤーの方向を向く処理を追加
            ThrowLeg();
        }
    }

    void ThrowLeg()
    {
        if (legs.Count == 0 || legPrefab == null)
        {
            //Debug.LogWarning("足が不足，またはプレハブ未設定");
            return;
        }

        Debug.Log("Attackトリガーをセット");
        animator.SetTrigger("Attack");

        canThrow = false;
        isThrowing = true;

        StartCoroutine(ThrowLegAfterDelay(attackAnimationDelay)); // アニメーションのタイミングに合わせて足を投げる
        StartCoroutine(ResetThrowCooldown());
    }

    IEnumerator ThrowLegAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (legs.Count == 0 || legPrefab == null)
        {
            Debug.LogWarning("足が不足，またはプレハブ未設定（遅延後）");
            yield break;
        }

        // 足を非アクティブ化＆リストから削除
        legs[0].SetActive(false);
        legs.RemoveAt(0);

        // プレイヤー方向へ発射
        Vector3 direction = (player.position - legSpawnPoint.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        OctopusLegController legInstance = Instantiate(legPrefab, legSpawnPoint.position, rotation);
        SoundManager.Instance.PlaySE("OctopusThrow");
        legInstance.Init(legThrowSpeed, direction);
    }

    IEnumerator MoveLeg(Transform legTransform, Vector3 direction)
    {
        float duration = 1f;
        float time = 0f;

        while (time < duration)
        {
            if (legTransform == null) yield break; // 参照切れ対策

            legTransform.position += direction * legThrowSpeed * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        legTransform.gameObject.SetActive(false);
    }

    IEnumerator ResetThrowCooldown()
    {
        yield return new WaitForSeconds(throwCooldown);
        canThrow = true;
        isThrowing = false;
    }

    protected bool CanThrowToPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle > 180f) return false; // 360°の半分

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > throwRadius) return false;

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distance, interctionLayer, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.CompareTag("Player");
        }

        return true;
    }

    void RotateTowardsPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f; // Y軸方向の回転を防ぐ（地面に沿った回転）

        if (direction != Vector3.zero)
        {
            transform.LookAt(transform.position + direction); // プレイヤー方向に回転
        }
    }
}
