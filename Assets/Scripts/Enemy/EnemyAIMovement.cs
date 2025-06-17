using UnityEngine;
using UnityEngine.AI;

// ステートの基底クラス
public abstract class EnemyState
{
    protected EnemyAIMovement enemy;

    public EnemyState(EnemyAIMovement enemy) { this.enemy = enemy; }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}

// 巡回状態
public class PatrolState : EnemyState
{
    public PatrolState(EnemyAIMovement enemy) : base(enemy) { }

    public override void Enter()
    {
        // Debug.Log("巡回開始");
        enemy.Agent.isStopped = false;
        enemy.Agent.speed = enemy.PatrolSpeed;
    }

    public override void Update()
    {
        enemy.Agent.SetDestination(enemy.PatrolPoints[enemy.CurrentPatrolIndex].position);
        if (enemy.CanSeePlayer())
        {
            enemy.ChangeState(new SurprisedState(enemy));
        }
        if (!enemy.Agent.pathPending && enemy.Agent.remainingDistance < 0.5f)
        {
            enemy.CurrentPatrolIndex = (enemy.CurrentPatrolIndex + 1) % enemy.PatrolPoints.Length; // PatrolPointが最後に来たら最初に戻る
        }
    }

    public override void Exit()
    {
        // Debug.Log("巡回終了");
    }
}

// 敵発見時のびっくり状態
public class SurprisedState : EnemyState
{
    public SurprisedState(EnemyAIMovement enemy) : base(enemy) { }

    float surpriseTimer;
    GameObject exclamationSymbol;

    public override void Enter()
    {
        exclamationSymbol = GameObject.Instantiate(enemy.ExclamationSymbolPrefab, enemy.ActiveSymbol);
        enemy.Agent.isStopped = true;
    }

    public override void Update()
    {
        surpriseTimer += Time.deltaTime;
        if (surpriseTimer <= enemy.SurpriseDuration) return;
        enemy.ChangeState(new ChaseState(enemy));
    }

    public override void Exit()
    {
        GameObject.Destroy(exclamationSymbol);
    }
}

// 追跡状態
public class ChaseState : EnemyState
{
    public ChaseState(EnemyAIMovement enemy) : base(enemy) { }

    public override void Enter()
    {
        // Debug.Log("追跡開始");
        enemy.Agent.isStopped = false;
        enemy.Agent.speed = enemy.ChaseSpeed;
    }

    public override void Update()
    {
        enemy.Agent.SetDestination(enemy.Player.position);

        if (enemy.CanSeePlayer()) return;

        enemy.ChangeState(new AlertState(enemy));


        // if (Vector3.Distance(enemy.transform.position, enemy.Player.position) < 2f)
        // {
        //     enemy.ChangeState(new AttackState(enemy));
        // }
    }

    public override void Exit()
    {
        // Debug.Log("追跡終了");

    }
}


// 警戒状態
public class AlertState : EnemyState
{
    public AlertState(EnemyAIMovement enemy) : base(enemy) { }

    GameObject questionSymbol;
    float alertTimer;

    public override void Enter()
    {
        // Debug.Log("警戒開始");
        questionSymbol = GameObject.Instantiate(enemy.QuestionSymbolPrefab, enemy.ActiveSymbol);
        enemy.Agent.isStopped = true;
    }

    public override void Update()
    {
        if (enemy.CanSeePlayer())
        {
            enemy.ChangeState(new SurprisedState(enemy));
        }
        alertTimer += Time.deltaTime;
        // TODO : 首を横に振るような動き
        if (alertTimer <= enemy.AlertDuration) return;
        enemy.ChangeState(new PatrolState(enemy));
    }

    public override void Exit()
    {
        // Debug.Log("警戒終了");
        GameObject.Destroy(questionSymbol);
    }
}

// 攻撃状態（いまのところまだこのStateには移動しない）
public class AttackState : EnemyState
{
    public AttackState(EnemyAIMovement enemy) : base(enemy) { }

    public override void Enter()
    {
        // Debug.Log("攻撃開始");
        enemy.Agent.isStopped = true;
    }

    public override void Update() { }

    public override void Exit()
    {
        // Debug.Log("攻撃終了");
    }
}

// メインAIクラス
public class EnemyAIMovement : MonoBehaviour
{
    [Tooltip("敵のPatrol状態でのスピード")]
    [SerializeField] float patrolSpeed = 2.5f;
    [Tooltip("敵のChase状態でのスピード")]
    [SerializeField] float chaseSpeed = 4f;
    [Tooltip("敵の視野の中央角")]
    [SerializeField] float fieldOfViewAngle = 90f;
    [Tooltip("敵の視野の半径")]
    [SerializeField] float sightRange = 10f;
    [Tooltip("敵の巡回ポイント")]
    [SerializeField] Transform[] patrolPoints;
    [Tooltip("敵がPlayerを発見してから驚いてストップしている時間")]
    [SerializeField] float surpriseDuration = 0.5f;
    [Tooltip("敵がPlayerを見失った後、警戒状態を維持する時間")]
    [SerializeField] float alertDuration = 3f;
    [Tooltip("Physics.Raycastが当たるLayer")]
    [SerializeField] LayerMask interctionLayer;
    [Header("以下変更は不要")]
    [SerializeField] Transform player;
    [SerializeField] Transform activeSymbol;
    [SerializeField] GameObject exclamationSymbolPrefab;
    [SerializeField] GameObject questionSymbolPrefab;


    private EnemyState currentState;
    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;

    public float PatrolSpeed => patrolSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float FieldOfViewAngle => fieldOfViewAngle;
    public float SightRange => sightRange;
    public Transform[] PatrolPoints => patrolPoints;
    public float SurpriseDuration => surpriseDuration;
    public float AlertDuration => alertDuration;
    public Transform ActiveSymbol => activeSymbol;
    public GameObject ExclamationSymbolPrefab => exclamationSymbolPrefab;
    public GameObject QuestionSymbolPrefab => questionSymbolPrefab;
    public NavMeshAgent Agent => agent;
    public Transform Player => player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ChangeState(new PatrolState(this));
    }

    void Update()
    {
        currentState.Update();
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    void StartChase()
    {
        ChangeState(new ChaseState(this));
    }

    // currentPatrolIndex のプロパティ
    public int CurrentPatrolIndex
    {
        get => currentPatrolIndex;  // 現在のインデックスを取得
        set => currentPatrolIndex = Mathf.Clamp(value, 0, patrolPoints.Length - 1);  // インデックスを設定（範囲チェックも同時に）
    }

    // プレイヤーを視界内で認識できるかチェック
    public bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized; // Enemyから見たPlayerの方向を正規化してゲット
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle > FieldOfViewAngle * 0.5f) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
        if (distanceToPlayer > SightRange) return false;

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distanceToPlayer, interctionLayer, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.CompareTag("Player");
        }
        return true;
    }
}
