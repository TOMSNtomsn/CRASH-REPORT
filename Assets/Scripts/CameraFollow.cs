using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 6.8f, -5);
    private Transform player;

    void Start()
    {
        
        // "Player"タグを持つGameObjectのTransformを取得
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Playerタグのオブジェクトが見つかりません．");
        }
    }

    void LateUpdate()
    {
        // プレイヤの位置にオフセットを加えた位置にカメラを配置
        transform.position = player.position + offset;

        // カメラをプレイヤの方向に向ける
        transform.LookAt(player);
    }
}
