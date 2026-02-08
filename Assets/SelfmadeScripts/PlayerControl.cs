using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField, Header("移動速度")]
    private float _moveSpeed;
    private Vector2 _inputDirection;

    //〈ジャンプ〉
    [SerializeField, Header("ジャンプ力")]
    private float _jumpPower;


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
        _rigid.linearVelocity = new Vector2(_inputDirection.x * _moveSpeed, _rigid.linearVelocity.y);
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
        _rigid.gravityScale = 1f;

    }
}
