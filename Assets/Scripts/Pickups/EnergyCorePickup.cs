using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyCorePickup : Pickup
{
    protected override void OnPickup(Transform player)
    {
        SoundManager.Instance.PlaySE("GetCore");
        LogManager.Instance.Register(LogType.EnergyCore);
        player.GetComponent<PlayerLaserController>().ChangeLaserMode(LaserMode.PowerUp);
    }

}
