using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstCorePickup : Pickup
{
    protected override void OnPickup(Transform player)
    {
        SoundManager.Instance.PlaySE("GetCore");
        LogManager.Instance.Register(LogType.BurstCore);
        player.GetComponent<PlayerLaserController>().ChangeLaserMode(LaserMode.Burst);
    }
}
