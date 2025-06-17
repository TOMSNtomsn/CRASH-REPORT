using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZipController : BaseEnemyController
{
    [SerializeField] float speed;
    [SerializeField] float timeForExplosion;
    [SerializeField] AttackerToEnemy explosionPrefab;
    [SerializeField] Animator[] attackAnimators;
    [SerializeField] Animator walkAnimator;
    [SerializeField] GameObject particle;
    [SerializeField] int explosionAttack;
    [SerializeField] SkinnedMeshRenderer head;
    [SerializeField] Material firedMaterial;
    [SerializeField] float zipOpenSEInterval = 1.0f; // ← 追加：SEの再生間隔

    float speedRate = 1;
    bool opening = false;
    Coroutine zipOpenCoroutine;

    public override void EnemyFixedUpdate()
    {
        if (speedRate > 0)
        {
            transform.position += speed * speedRate * DirectionToPlayerOnlyXZ;
            transform.forward = DirectionToPlayerOnlyXZ;
        }
    }

    IEnumerator Explosion()
    {
        float t = 0;
        float startSpeedRate = speedRate;

        while (t < 1)
        {
            speedRate = Mathf.Lerp(startSpeedRate, 0, t / 0.7f);
            walkAnimator.speed = speedRate;
            t += Time.deltaTime / timeForExplosion;
            yield return null;
        }

            // SEループ停止
        if (zipOpenCoroutine != null)
        {
        StopCoroutine(zipOpenCoroutine);
        zipOpenCoroutine = null;
        }

        AttackerToEnemy explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.attack = explosionAttack;
        SoundManager.Instance.PlaySE("ZipExplosion");
        Killed();
    }

    protected override void OnAttacked(int amount)
    {
        base.OnAttacked(amount);

        if (!opening)
        {
            particle.SetActive(true);
            head.material = firedMaterial;
            speedRate = 6;

            LogManager.Instance.Register(LogType.Zip);
            SoundManager.Instance.PlaySE("ZipOpen");

            foreach (Animator animator in attackAnimators)
            {
                animator.SetTrigger("Attack");
                animator.speed = 1.75f / timeForExplosion;
            }

            opening = true;

            // SEを0.2秒ごとに鳴らす処理開始
            zipOpenCoroutine = StartCoroutine(PlayZipOpenLoop());

            StartCoroutine(Explosion());
        }
    }

    IEnumerator PlayZipOpenLoop()
    {
        while (true)
        {
            SoundManager.Instance.PlaySE("ZipOpen");
            yield return new WaitForSeconds(zipOpenSEInterval);
        }
    }
}
