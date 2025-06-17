using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 倒した敵の数を種類ごとにカウントする
/// </summary>
public class EnemyManager : MonoBehaviour
{
    // シングルトンインスタンス
    public static EnemyManager Instance { get; private set; }

    // 敵の種類ごとの撃破数を保持する辞書
    private Dictionary<EnemyType, int> enemyKillCount;

    // Awakeメソッドでシングルトンの初期化
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 敵の撃破数を初期化
        enemyKillCount = new Dictionary<EnemyType, int>
        {
            //{ EnemyType.Red, 0 },
            //{ EnemyType.Yellow, 0 }
        };
    }

    // 敵の種類に応じて撃破数をインクリメントするメソッド
    public void IncrementEnemyKillCount(EnemyType enemyType)
    {
        if (enemyKillCount.ContainsKey(enemyType))
        {
            enemyKillCount[enemyType]++;
        }
    }

    // 特定の敵の撃破数を取得するメソッド
    public int GetEnemyKillCount(EnemyType enemyType)
    {
        return enemyKillCount.ContainsKey(enemyType) ? enemyKillCount[enemyType] : 0;
    }
}
