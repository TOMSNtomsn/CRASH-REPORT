using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitController : BaseEnemyController
{
    [SerializeField] float moveSpeed = 3f; // 移動速度
    [SerializeField] Sprite stopSprite;     // 停止スプライト
    [SerializeField] Sprite upSprite1;     // 上向きスプライト
    [SerializeField] Sprite upSprite2;     // 上向きスプライト
    [SerializeField] Sprite downSprite1;   // 下向きスプライト
    [SerializeField] Sprite downSprite2;   // 下向きスプライト
    [SerializeField] Sprite rightSprite1;  // 右向きスプライト
    [SerializeField] Sprite rightSprite2;
    [SerializeField] float splitDistance = 2f; // 分裂時の距離（Inspectorで編集可能）
    [SerializeField] float splitTime = 10f; // 分裂するまでの時間（秒）

    private bool isChasing = false; // プレイヤーを追いかけているかどうか
    private SpriteRenderer spriteRenderer; // SpriteRenderer
    private float splitTimer = 0f;
    private float spriteChangeInterval = 0.4f; // スプライトを交互に変える間隔（秒）
    private float spriteChangeTimer = 0f;
    private bool isRightSpriteAlt = false; // 切り替え判定用フラグ

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer の取得
    }

    public override void EnemyUpdate()
    {
        // プレイヤーが追いかける範囲に入ったら追いかける
        if (CanSeePlayer())
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        // 追いかけている場合、プレイヤーの方に向かって移動
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            StopChasing();
        }

        // 分裂タイマーの処理
        splitTimer += Time.deltaTime;
        if (splitTimer >= splitTime)
        {
            SplitRabbit();
            splitTimer = 0f; // タイマーをリセットして10秒ごとに分裂
        }
    }

    void ChasePlayer()
    {
        // プレイヤーのY座標ではなく、自分のY座標を使用して水平方向のみの移動にする
        Vector3 directionToPlayer = DirectionToPlayerOnlyXZ;

        transform.position += directionToPlayer * moveSpeed * Time.deltaTime;
        ChangeSprite(directionToPlayer);
    }

    // プレイヤーを追いかけていないときの処理（立ち止まる）
    void StopChasing()
    {
        // ここでは何もしない（立ち止まるだけ）
        spriteRenderer.sprite = stopSprite;
        spriteRenderer.flipX = false;
    }

    void ChangeSprite(Vector3 direction)
    {
        spriteChangeTimer += Time.deltaTime;
        if (spriteChangeTimer >= spriteChangeInterval)
        {
            isRightSpriteAlt = !isRightSpriteAlt;
            spriteChangeTimer = 0f;
        }

        // X軸の移動が優先される
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            // 左右移動
            spriteRenderer.sprite = isRightSpriteAlt ? rightSprite1 : rightSprite2;
            spriteRenderer.flipX = direction.x < 0; // 左なら反転
        }
        else
        {
            // 上下移動
            spriteRenderer.flipX = false;

            if (direction.z > 0)
            {
                spriteRenderer.sprite = isRightSpriteAlt ? upSprite1 : upSprite2;
            }
            else
            {
                spriteRenderer.sprite = isRightSpriteAlt ? downSprite1 : downSprite2;
            }
        }
    }

    void SplitRabbit()
    {
        // ランダムな方向を決める（X-Z 平面）
        Vector2 randomDir2D = Random.insideUnitCircle.normalized;
        Vector3 randomDir = new Vector3(randomDir2D.x, 0f, randomDir2D.y);

        // splitDistance 分その方向に移動した位置に生成
        Vector3 spawnPosition = transform.position + randomDir * splitDistance;

        SoundManager.Instance.PlaySE("RabbitSplit");
        LogManager.Instance.Register(LogType.Rabbit);
        Instantiate(this, spawnPosition, transform.rotation);
    }
}
