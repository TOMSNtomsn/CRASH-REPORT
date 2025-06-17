using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    NotOpened,
    Virus,
    Deltacube,    
    Octopus,
    Troia,
    Rabbit,
    Zip,
    Dome,
    Keylogger
}

public class BaseEnemyController : MonoBehaviour
{
    [SerializeField] EnemyInfo enemyInfo;
    [SerializeField] int startLife;
    [SerializeField] protected float fieldOfViewAngle; // 視野の角度
    [SerializeField] protected float sightRadius; // 視野が届く半径
    [SerializeField] protected LayerMask interctionLayer; // Everythingにしとけば今の所問題ない

    int life;
    GameController gameController;

    public EnemyInfo EnemyInfo => enemyInfo;

    protected Transform player;
    protected Vector3 DirectionToPlayer { get { return (player.position - transform.position).normalized; } } // playerの方向，正規化済み
    protected Vector3 DirectionToPlayerOnlyXZ { get { var v = DirectionToPlayer; v.y = 0; return v.normalized; } } // playerの方向のy成分を0にしたもの，正規化済み


    public void SetUp() // EnemyUpdater により実行される
    {
        life = startLife;

        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.Register(this);
    }

    public virtual void EnemyUpdate() { } // EnemyUpdater により実行される
    public virtual void EnemyFixedUpdate() { } // EnemyUpdater により実行される


    public void SetPlayer(Transform player)
    {
        this.player = player;
    }

    protected bool CanSeePlayer() // Playerを視認できているか EnemyAIMovementから拝借
    {
        float angle = Vector3.Angle(transform.forward, DirectionToPlayer);

        if (angle > fieldOfViewAngle * 0.5f) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > sightRadius) return false;

        if (Physics.Raycast(transform.position, DirectionToPlayer, out RaycastHit hit, distanceToPlayer, interctionLayer, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.CompareTag("Player");
        }
        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        AttackerToEnemy attackerToEnemy = other.transform.GetComponent<AttackerToEnemy>();

        if (attackerToEnemy) Attacked(attackerToEnemy.attack);
    }

    public void Attacked(int amount) // 攻撃を処理受けた時の共通処理
    {
        SoundManager.Instance.PlaySE("Damage");
        OnAttacked(amount);
    }

    protected virtual void OnAttacked(int amount) // 攻撃を処理受けた時の固有処理
    {
        if (life <= 0) return;

        life -= amount;

        if (life <= 0)
        {
            Killed();
        }
    }

    public void Killed(bool log = true) // 死んだ時の共通処理
    {
        gameController.EnemyKilled(enemyInfo.japaneseName, log);
        OnKilled();
    }

    protected virtual void OnKilled() // 死んだ時の固有処理
    {
        Destroy(gameObject);
    }
}
