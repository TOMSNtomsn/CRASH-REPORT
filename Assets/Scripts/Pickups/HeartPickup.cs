using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPickup : Pickup
{
    [SerializeField] int healAmount = 1;

    protected override void OnPickup(Transform player)
    {
        SoundManager.Instance.PlaySE("Heart");
        player.GetComponent<PlayerLifeController>().ChangeLife(healAmount);
    }
}


