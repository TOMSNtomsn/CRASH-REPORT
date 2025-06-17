using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKey
{
    // public static readonly List<KeyCode> JumpKeys = new() { KeyCode.Space };
    public static readonly List<KeyCode> UpKeys = new() { KeyCode.UpArrow, KeyCode.W };
    public static readonly List<KeyCode> DownKeys = new() { KeyCode.DownArrow, KeyCode.S };
    public static readonly List<KeyCode> RightKeys = new() { KeyCode.RightArrow, KeyCode.D };
    public static readonly List<KeyCode> LeftKeys = new() { KeyCode.LeftArrow, KeyCode.A };

    // public static bool IsJumpKey(KeyCode keyCode) => JumpKeys.Contains(keyCode);
    public static bool IsUpKey(KeyCode keyCode) => UpKeys.Contains(keyCode);
    public static bool IsDownKey(KeyCode keyCode) => DownKeys.Contains(keyCode);
    public static bool IsRightKey(KeyCode keyCode) => RightKeys.Contains(keyCode);
    public static bool IsLeftKey(KeyCode keyCode) => LeftKeys.Contains(keyCode);
}

public class PlayerMovementController : MonoBehaviour, IDelayedInput
{
    [SerializeField] float Speed = 0.25f;
    //[SerializeField] float rotationSpeed = 10f;
    // [SerializeField] float jumpForce = 1.75f;
    // [SerializeField] float extraGravityMultiplier = 2.0f;
    // [SerializeField] float groundCheckDistance = 0.1f;
    // [SerializeField] LayerMask groundLayerMask = ~0; // 生きてるEnemyのLayerを除くなど後で指定可能

    Vector3 velocity = Vector3.zero;

    int up; // キーを離してからのフレームを管理
    int down;
    int right;
    int left;
    const int DirectionEnableFrame = 3;

    Rigidbody rb;


    // bool onGround;
    // bool jumpRequested = false;



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // GroundCheck();
    }

    public void GetKeyDown(KeyCode keyCode)
    {
        // if (PlayerKey.IsJumpKey(keyCode))
        // {
        //     if (onGround) jumpRequested = true;
        // }
        if (PlayerKey.IsUpKey(keyCode))
        {
            Vector3 v = velocity;
            v.z = 1;
            velocity = v;
        }
        if (PlayerKey.IsDownKey(keyCode))
        {
            Vector3 v = velocity;
            v.z = -1;
            velocity = v;
        }
        if (PlayerKey.IsRightKey(keyCode))
        {
            Vector3 v = velocity;
            v.x = 1;
            velocity = v;
        }
        if (PlayerKey.IsLeftKey(keyCode))
        {
            Vector3 v = velocity;
            v.x = -1;
            velocity = v;
        }
    }

    public void GetKeyUp(KeyCode keyCode)
    {
        if (PlayerKey.IsUpKey(keyCode))
        {
            Vector3 v = velocity;
            if (v.z == 1) v.z = 0;
            velocity = v;

            up = DirectionEnableFrame;
        }
        if (PlayerKey.IsDownKey(keyCode))
        {
            Vector3 v = velocity;
            if (v.z == -1) v.z = 0;
            velocity = v;

            down = DirectionEnableFrame;
        }
        if (PlayerKey.IsRightKey(keyCode))
        {
            Vector3 v = velocity;
            if (v.x == 1) v.x = 0;
            velocity = v;

            right = DirectionEnableFrame;
        }
        if (PlayerKey.IsLeftKey(keyCode))
        {
            Vector3 v = velocity;
            if (v.x == -1) v.x = 0;
            velocity = v;

            left = DirectionEnableFrame;
        }
    }

    public void GetMouseButtonDown(int button) { }

    void FixedUpdate()
    {
        if (GameController.GameParameter.GameEnd)
        {
            return;
        }

        if (up >= 0) up--;
        if (down >= 0) down--;
        if (right >= 0) right--;
        if (left >= 0) left--;

        // rb.velocity = new Vector3(velocity.x * Speed, rb.velocity.y, velocity.z * Speed); // 念のためｙ軸は変えない
        rb.AddForce(velocity * Speed, ForceMode.VelocityChange); // 速度を変えるが、慣性を残す

        // HandleJumpInput();

        if (velocity.x == 0 && velocity.z == 0) return;

        Vector3 direction = Vector3.zero;

        if (up >= 0 || velocity.z > 0) direction += Vector3.forward; // キーが押されてなくても直前まで押されてたら反映
        if (down >= 0 || velocity.z < 0) direction += Vector3.back;
        if (right >= 0 || velocity.x > 0) direction += Vector3.right;
        if (left >= 0 || velocity.x < 0) direction += Vector3.left;

        if (direction == Vector3.zero) direction = velocity; // この処理がないとdirectionが0のとき(例えば右と左だけを押して同時に離す)におかしくなる


        // 現在の向きから目標方向への回転を滑らかにする
        // Quaternion targetRotation = Quaternion.Euler(0, Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg, 0);
        // transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed); // 10fは回転の速さ
        transform.rotation = Quaternion.Euler(0, Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg, 0); // directionの方向を向く
    }




    /* void HandleJumpInput()

    {
        if (jumpRequested)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequested = false;
        }
        // ジャンプをきびきびとさせるため、空中にいるなら下向きに加速度をかける
        if (!onGround) rb.AddForce(Vector3.up * Physics.gravity.y * (extraGravityMultiplier - 1), ForceMode.Acceleration);
    }

    void GroundCheck()
    {
        onGround = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayerMask);
    } */
}
