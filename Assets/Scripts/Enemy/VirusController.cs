using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusController : BaseEnemyController
{
    [SerializeField] float speed;

    public override void EnemyFixedUpdate()
    {
        if (CanSeePlayer())
        {
            transform.position += speed * DirectionToPlayer;
            transform.forward = DirectionToPlayer;
        }
    }
}
