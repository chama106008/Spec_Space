using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerControl : MonoBehaviour
{
    //---変数---

    //〈汎用〉
    private Rigidbody2D _rigid;
    private BoxCollider2D _collider;
    [SerializeField, Header("地上判定")]
    private bool _onGround;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundRadius = 0.15f;
    [SerializeField] float groundCheckDistance = 0.87f;
    [SerializeField] LayerMask groundLayer = default;//一先ず初期レイヤーのみ検出、本来は地上を含むアクティブレイヤーを選択。


    //〈移動〉
    [SerializeField, Header("最高速度")]
    private float _maxSpeed = 30;
    [SerializeField, Header("加速度")]
    private float _acceleration = 80f;
    private Vector2 _inputDirection;
    [SerializeField] private float _groundResistance = 8f;
    [SerializeField] private float _airResistance = 1f;

    //〈ジャンプ〉
    [SerializeField, Header("ジャンプ力")]
    private float _jumpPower;

    //〈摩擦〉
    public PhysicsMaterial2D zeroFriction;
    public PhysicsMaterial2D normal;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _onGround = false;

    }

    // Update is called once per frame
    void Update()
    {
        _Move();
        _onGround = IsGrounded();
    }

    //接地判定
    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.CircleCast(
            groundCheck.position,
            groundRadius,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        return hit.collider != null;
    }


    //スクリプト内関数
    private void _Move()
    {
        Vector2 velocity = _rigid.linearVelocity;

        if (_onGround)
        {
            // 入力を「加速」として加える
            velocity.x += _inputDirection.x * _acceleration * Time.fixedDeltaTime;
        }
        else
        {   
            // 空中では制御を緩めに
            velocity.x += _inputDirection.x * _acceleration / _groundResistance * _airResistance * Time.fixedDeltaTime;
        }

            float resistance =
                _onGround ? _groundResistance : _airResistance;

        float factor = 1f - resistance * Time.fixedDeltaTime;
        factor = Mathf.Clamp01(factor);
        velocity.x *= factor;

        // 最高速度で補正
        if (velocity.x > 0) velocity.x = Mathf.Min(velocity.x, _maxSpeed);
        else velocity.x = Mathf.Max(velocity.x, -1 * _maxSpeed);
        if (velocity.y > 0) velocity.y = Mathf.Min(velocity.y, _maxSpeed);
        else velocity.y = Mathf.Max(velocity.y, -1 * _maxSpeed);

        _rigid.linearVelocity = velocity;

    }

    //------コントローラー処理------
    public void OnMove(InputAction.CallbackContext context)
    {
        _inputDirection = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed || !_onGround) return;
        _rigid.AddForce(Vector2.up * _jumpPower, ForceMode2D.Force);
    }

    public void OnSlipThroughOn(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        _collider.isTrigger = true;
    }
    public void OnSlipThroughOff(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        _collider.isTrigger = false;
    }

    public void OnLightWeightOn(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        _rigid.mass = 0.1f;
        _rigid.gravityScale = 0.01f;

    }
    public void OnLightWeightOff(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        _rigid.mass = 1f;
        _rigid.gravityScale = 2f;

    }

    public void OnAccelerateOn(InputAction.CallbackContext context)
    {   
        if (!context.performed) return;
        _groundResistance *= (-3);
        _airResistance *= (-3);
        _collider.sharedMaterial = zeroFriction;
    }
    public void OnAccelerateOff(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        _groundResistance = _groundResistance / (-3);
        _airResistance = _airResistance / (-3);
        _collider.sharedMaterial = normal;
    }
}
