using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWarper : MonoBehaviour
{
    [SerializeField] Transform tyuusin; // 中心点（ワープの軸）
    [SerializeField] float warpModifier = 0.99f; // ワープの倍率（必要に応じて調整）

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("WarpWall"))
        {
            // 衝突点を取得
            Vector3 hitPoint = collision.contacts[0].point;

            // 中心からヒットポイントへのベクトル
            Vector3 direction = hitPoint - tyuusin.position;

            direction *= warpModifier; // ワープの倍率を適用

            // 逆側の位置を計算（中心点から方向ベクトルを反転）
            Vector3 warpedPosition = tyuusin.position - direction;

            // プレイヤーの位置をワープ
            transform.position = new Vector3(warpedPosition.x, transform.position.y, warpedPosition.z);

            // オプション：方向も変えたい場合
            // transform.forward = -transform.forward;

            Debug.Log("ワープしました: " + warpedPosition);
        }
    }
}
