using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class DomeEffectInfo
{
    public float Scale;
    public int FrameRate;
    public float InputDelay;

    public DomeEffectInfo(float scale, int frameRate, float inputDelay)
    {
        Scale = scale; FrameRate = frameRate; InputDelay = inputDelay;
    }
}

public class DomeController : BaseEnemyController
{
    [SerializeField] float ExpansionSpeed = 0.005f;
    [SerializeField] float ShrinkOnDamage = 3;

    [SerializeField]
    List<DomeEffectInfo> EffectInfos = new()
    {
        new DomeEffectInfo( 5, 40, 0.00f),
        new DomeEffectInfo(10, 30, 0.10f),
        new DomeEffectInfo(15, 20, 0.15f),
        new DomeEffectInfo(20, 15, 0.20f),
        new DomeEffectInfo(25, 10, 0.25f),
    };

    int oldPhase = -1;

    public override void EnemyFixedUpdate()
    {
        transform.localScale += ExpansionSpeed * Vector3.one;

        float currentScale = transform.localScale.x;
        int currentPhase = 0;

        for (int i = EffectInfos.Count - 1; i >= 0; i--)
        {
            if (currentScale >= EffectInfos[i].Scale)
            {
                currentPhase = i + 1;
                break;
            }
        }

        if (currentPhase == oldPhase) return;

        if (currentPhase == 0)
        {
            GameController.GameParameter.SetDefaultFrameRate();
            GameController.GameParameter.SetDelay(0);
        }
        else
        {
            GameController.GameParameter.SetFrameRate(EffectInfos[currentPhase - 1].FrameRate);
            GameController.GameParameter.SetDelay(EffectInfos[currentPhase - 1].InputDelay);

            if (oldPhase < currentPhase) LogManager.Instance.Register(LogType.Dome);
        }

        oldPhase = currentPhase;

        Debug.Log($"frame rate: {Application.targetFrameRate}, input delay: {GameController.GameParameter.Delay}");
    }

    protected override void OnAttacked(int amount)
    {
        float currentScale = transform.localScale.x;
        float nextScale = currentScale - ShrinkOnDamage;

        if (currentScale > 10 && nextScale <= 0) // scaleが10より大きい時は即死しない
        {
            nextScale = 1f;
        }

        if (0 < nextScale && nextScale < 1f) // 死んでない時のscaleは最小で1
        {
            nextScale = 1f;
        }

        transform.localScale = nextScale * Vector3.one;

        if (nextScale <= 0) Killed();
    }
}
