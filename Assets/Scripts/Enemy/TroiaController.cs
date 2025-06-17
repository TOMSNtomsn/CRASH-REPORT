using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroiaController : BaseEnemyController
{
    enum Phase
    {
        Standby,
        Encount,
        Alert,
        Detect,
        Dash
    }

    [SerializeField] float encountTime;
    [SerializeField] float fieldOfViewAngleAfterEncount;
    [SerializeField] float sightRadiusAfterEncount;
    [SerializeField] float dashSpeed;
    [SerializeField] float fadeSpeed;
    [SerializeField] float alertSpeed;
    [SerializeField] float timeToDash;
    [SerializeField] GameObject horse;
    [SerializeField] GameObject[] pickups;
    [SerializeField] GameObject particle;

    Phase phase = Phase.Standby;
    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    public override void EnemyFixedUpdate()
    {
        if (phase == Phase.Standby)
        {
            if (CanSeePlayer())
            {
                StartCoroutine(Encount());
            }
        }
        else if (phase == Phase.Alert)
        {
            transform.rotation *= Quaternion.Euler(0, alertSpeed, 0);

            if (CanSeePlayer())
            {
                StartCoroutine(DetectCoroutine());
            }
        }
        else if (phase == Phase.Dash)
        {
            WhileDash();
        }
    }

    IEnumerator Encount()
    {
        phase = Phase.Encount;
        particle.SetActive(true);
        SoundManager.Instance.PlaySE("TrojanHorseAppear");
        LogManager.Instance.Register(LogType.Troia);

        yield return new WaitForSeconds(1.25f);

        fieldOfViewAngle = fieldOfViewAngleAfterEncount;
        sightRadius = sightRadiusAfterEncount;
        transform.forward = DirectionToPlayerOnlyXZ;

        body.useGravity = true;
        horse.SetActive(true);

        foreach (GameObject pickup in pickups)
        {
            pickup.SetActive(false);
        }

        yield return new WaitForSeconds(encountTime);

        Alert();
    }

    void Alert()
    {
        phase = Phase.Alert;
        alertSpeed *= 1 - 2 * Random.Range(0, 2);
    }

    IEnumerator DetectCoroutine()
    {
        phase = Phase.Detect;

        yield return new WaitForSeconds(timeToDash);

        phase = Phase.Dash;

        body.velocity = dashSpeed * DirectionToPlayerOnlyXZ;
        transform.forward = body.velocity;
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
}
