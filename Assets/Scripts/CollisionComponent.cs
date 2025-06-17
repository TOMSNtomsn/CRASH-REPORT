using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 当たり判定を持つオブジェクトに原則アタッチし，プレイヤーやレーザーなどが当たった時の挙動を指定する
// リストを使って複数指定できる（複数指定するためにタグをやめた）
//「当たった敵にダメージを与える」については，攻撃力という概念があるので別コンポーネントAttackerToEnemyで行う

public enum CollisionType
{
    IsPlayer, // Playerである
    DamageToPlayer, // プレイヤーにダメージを与える
    DestroyPlayerLaser, // プレイヤーが放ったレーザーを消す
    DestroyEnemyBullet // キーロガーの弾,オクトパスの足を消す
}

public class CollisionComponent : MonoBehaviour
{
    [SerializeField] List<CollisionType> collisionTypes;

    public bool Contains(CollisionType type) => collisionTypes.Contains(type);
}