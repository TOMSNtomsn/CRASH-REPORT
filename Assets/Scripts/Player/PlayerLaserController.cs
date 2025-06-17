using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LaserMode
{
    Default,
    PowerUp,
    Burst,
}

public class PlayerLaserController : MonoBehaviour, IDelayedInput
{
    [SerializeField] LaserMode startingLaserMode = LaserMode.Default;
    [SerializeField] Laser laserDefaultPrefab;
    [SerializeField] Laser laserPowerUpPrefab;
    [SerializeField] Laser laserBurstPrefab;
    [SerializeField] float powerUpDuration = 15f;
    [SerializeField] float burstDuration = 15f;
    [SerializeField] Transform laserSpawnPoint;

    Dictionary<LaserMode, LaserData> laserMap;

    LaserMode currentLaserMode;
    float timer;

    Action<LaserMode> onLaserModeChanged = (LaserMode mode) => { };
    Action<float> onTimerChanged = (float f) => { };

    struct LaserData
    {
        public Laser laserPrefab;
        public float duration;
    }

    void Awake()
    {
        laserMap = new(){
            { LaserMode.Default, new LaserData{laserPrefab = laserDefaultPrefab, duration = Mathf.Infinity }},
            { LaserMode.PowerUp, new LaserData{laserPrefab = laserPowerUpPrefab, duration = powerUpDuration }},
            { LaserMode.Burst, new LaserData{laserPrefab = laserBurstPrefab, duration = burstDuration }},
        };

    }

    void Start()
    {
        ChangeLaserMode(startingLaserMode);
    }

    void Update()
    {
        if (currentLaserMode == LaserMode.Default) return;

        timer += Time.deltaTime;
        onTimerChanged.Invoke(timer / laserMap[currentLaserMode].duration);

        if (timer >= laserMap[currentLaserMode].duration)
        {
            ChangeLaserMode(LaserMode.Default);
        }
    }

    public void GetMouseButtonDown(int button)
    {
        if (button == 0) InstantiateLaser();
    }

    void InstantiateLaser()
    {
        SoundManager.Instance.PlaySE("Shot");
        Instantiate(laserMap[currentLaserMode].laserPrefab, laserSpawnPoint.position, transform.rotation);
    }

    public void ChangeLaserMode(LaserMode newMode)
    {
        currentLaserMode = newMode;
        timer = 0;

        onLaserModeChanged.Invoke(currentLaserMode);
    }

    // 以下は使ってない
    public void GetKeyDown(KeyCode keyCode)
    {
    }

    public void GetKeyUp(KeyCode keyCode)
    {
    }

    public void AddOnLaserModeChanged(Action<LaserMode> action)
    {
        onLaserModeChanged += action;
    }

    public void AddOnTimerChanged(Action<float> action)
    {
        onTimerChanged += action;
    }
}
