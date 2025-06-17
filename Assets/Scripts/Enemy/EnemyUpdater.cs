using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUpdater : MonoBehaviour
{
    BaseEnemyController enemyController;

    void Start()
    {
        enemyController = GetComponent<BaseEnemyController>();
        enemyController.SetUp();
    }

    void Update()
    {
        if (!GameController.GameParameter.gameStart) return;
        if (GameController.GameParameter.GameEnd) return;

        if (transform.position.y <= -10) enemyController.Killed(false); // ステージ下に落ちたら死ぬ (ラビットが分裂時に落ちることがある)

        enemyController.EnemyUpdate();
    }

    void FixedUpdate()
    {
        if (!GameController.GameParameter.gameStart) return;
        if (GameController.GameParameter.GameEnd) return;

        enemyController.EnemyFixedUpdate();
    }
}
