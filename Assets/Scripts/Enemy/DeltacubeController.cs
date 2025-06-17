using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeltacubeController : BaseEnemyController
{
    enum Phase
    {
        Standby, // 擬態
        Detect, // 検知
        Dash, // 体当たり
        Alert1, // 体当たり後に周りを見る
        Alert2, // 別方向を見る
        Alert3, // 元の向きに戻る
    }

    [SerializeField] float dashSpeed;
    [SerializeField] float fadeSpeed;
    [SerializeField] float timeToDash; // ビックリマークが出てからダッシュするまでの時間
    [SerializeField] float alertAngle; // 周りを見渡す角度
    [SerializeField] float alertSpeed; // 周りを見渡すスピード
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject waitForm;
    [SerializeField] BoxCollider dashForm;
    [SerializeField] GameObject cubePrefab;

    Phase phase = Phase.Standby;
    Quaternion directionOnStop;
    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    public override void EnemyFixedUpdate()
    {
        switch (phase)
        {
            case Phase.Standby:
                if (CanSeePlayer()) StartCoroutine(DetectCoroutine());
                break;

            case Phase.Detect: break;

            case Phase.Dash:
                WhileDash();
                break;

            case Phase.Alert1:
            case Phase.Alert2:
            case Phase.Alert3:
                WhileAlert();
                if (CanSeePlayer()) StartCoroutine(DetectCoroutine());
                break;
        }
    }

    IEnumerator DetectCoroutine()
    {
        phase = Phase.Detect;
        SoundManager.Instance.PlaySE("DeltaCubeNotice");
        exclamation.SetActive(true);

        yield return new WaitForSeconds(timeToDash);

        phase = Phase.Dash;
        body.constraints &= ~RigidbodyConstraints.FreezePositionX; // FreezePositionXを解除
        body.constraints &= ~RigidbodyConstraints.FreezePositionZ; // FreezePositionZを解除
        exclamation.SetActive(false);
        waitForm.SetActive(false);
        dashForm.gameObject.SetActive(true);
        dashForm.isTrigger = true;

        body.velocity = dashSpeed * DirectionToPlayerOnlyXZ;
        transform.forward = body.velocity;

        yield return new WaitForSeconds(0.5f);

        dashForm.isTrigger = false;
    }

    void WhileDash()
    {
        Vector3 v = body.velocity;
        float speed = v.magnitude;
        speed = Mathf.Max(speed - fadeSpeed, 0);

        body.velocity = speed * v.normalized;

        if (speed == 0)
        {
            Alert();
        }
    }

    void Alert()
    {
        phase = Phase.Alert1;
        directionOnStop = transform.rotation;
        alertSpeed *= 1 - 2 * Random.Range(0, 2); // 最初にどっち方向を見るかはランダム
    }

    void WhileAlert()
    {
        float delta = alertSpeed;
        if (phase == Phase.Alert2) delta *= -1;

        transform.rotation *= Quaternion.Euler(0, delta, 0);

        if (phase == Phase.Alert1 || phase == Phase.Alert2)
        {
            if (Quaternion.Angle(directionOnStop, transform.rotation) > alertAngle)
            {
                if (phase == Phase.Alert1) phase = Phase.Alert2;
                else if (phase == Phase.Alert2) phase = Phase.Alert3;
            }
        }
        else if (phase == Phase.Alert3)
        {
            if (Quaternion.Angle(directionOnStop, transform.rotation) <= Mathf.Abs(2 * delta))
            {
                transform.rotation = directionOnStop;
                StartCoroutine(StandbyCoroutine());
            }
        }
    }

    IEnumerator StandbyCoroutine()
    {
        yield return new WaitForSeconds(timeToDash);

        phase = Phase.Standby;
        body.constraints |= RigidbodyConstraints.FreezePositionX; // FreezePositionXをセット
        body.constraints |= RigidbodyConstraints.FreezePositionZ; // FreezePositionZをセット
        waitForm.SetActive(true);
        dashForm.gameObject.SetActive(false);
    }

    protected override void OnKilled()
    {
        GameObject cube = Instantiate(cubePrefab, transform.position, transform.rotation);
        cube.transform.SetParent(transform.parent);

        Destroy(gameObject);
    }
}
