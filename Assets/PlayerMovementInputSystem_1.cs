using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class PlayerMovementInputSystem : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck1;
    [SerializeField] private Transform groundCheck2;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    public GameObject player1;
    public GameObject player2;

    [Header("Climbing Settings")]
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private LayerMask climableLayer;
    [SerializeField] private float climbCheckRadius = 0.4f;

    // Components
    private Rigidbody2D rb1;
    private Rigidbody2D rb2;
    private float originalGravity1;
    private float originalGravity2;
    private Vector3 startPosition1;
    private Vector3 startPosition2;
    private Vector3 currentRespawnPosition1;
    private Vector3 currentRespawnPosition2;

    // Input Values
    private Vector2 moveInput1;
    private Vector2 moveInput2;
    private float verticalInput1;
    private float verticalInput2;
    private PlayerInput playerInput;
    private bool isGrounded1;
    private bool isGrounded2;
    private bool isDead1 = false;
    private bool isDead2 = false;
    private bool isTouchingClimable1;
    private bool isTouchingClimable2;
    private bool isClimbing1;
    private bool isClimbing2;
    private bool isOnRope1 = false;
    private bool isOnRope2 = false;
    private GameObject grabbedRope1;
    private GameObject grabbedRope2;

    private InputAction moveAction;
    private InputAction jumpAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        
        rb1 = player1.GetComponent<Rigidbody2D>();
        originalGravity1 = rb1.gravityScale;
        startPosition1 = player1.transform.position;
        currentRespawnPosition1 = startPosition1;

        rb2 = player2.GetComponent<Rigidbody2D>();
        originalGravity2 = rb2.gravityScale;
        startPosition2 = player2.transform.position;
        currentRespawnPosition2 = startPosition2;

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }

    private void Update()
    {
        CheckGround(player1);
        CheckGround(player2);

        isTouchingClimable1 = Physics2D.OverlapCircle(player1.transform.position, climbCheckRadius, climableLayer);
        isTouchingClimable2 = Physics2D.OverlapCircle(player2.transform.position, climbCheckRadius, climableLayer);

        moveInput1 = new Vector2(
            (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0), 0);
        moveInput2 = new Vector2(
            (Keyboard.current.rightArrowKey.isPressed ? 1 : 0) - (Keyboard.current.leftArrowKey.isPressed ? 1 : 0), 0);

        verticalInput1 = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);
        verticalInput2 = (Keyboard.current.upArrowKey.isPressed ? 1 : 0) - (Keyboard.current.downArrowKey.isPressed ? 1 : 0);

        if (isTouchingClimable1 && Mathf.Abs(verticalInput1) > 0.1f)
        { isClimbing1 = true; }
        else if (!isTouchingClimable1)
        { isClimbing1 = false; }

        if (isTouchingClimable2 && Mathf.Abs(verticalInput2) > 0.1f)
        { isClimbing2 = true; }
        else if (!isTouchingClimable2)
        { isClimbing2 = false; }


        // --- Player 1 Jump Logic ---
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            if (isGrounded1 && !isClimbing1 && !isOnRope1)
            {
                rb1.linearVelocity = new Vector2(rb1.linearVelocity.x, jumpForce);
            }
            else if (isOnRope1)
            {
                LetGoRope(player1, grabbedRope1);
                rb1.linearVelocity = new Vector2(rb1.linearVelocity.x, jumpForce);
            }
        }

        // --- Player 2 Jump Logic ---
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            if (isGrounded2 && !isClimbing2 && !isOnRope2)
            {
                rb2.linearVelocity = new Vector2(rb2.linearVelocity.x, jumpForce);
            }
            else if (isOnRope2)
            {
                LetGoRope(player2, grabbedRope2);
                rb2.linearVelocity = new Vector2(rb2.linearVelocity.x, jumpForce);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDead1 || isDead2) return;

        // --- Player 1 Movement Logic ---
        if (isClimbing1)
        {
            rb1.gravityScale = 0f;
            rb1.linearVelocity = new Vector2(moveInput1.x * moveSpeed, verticalInput1 * climbSpeed);
        }
        else if (isOnRope1)
        {
            grabbedRope1.GetComponent<Rope>().ControlRope(moveInput1.x);
        }
        else
        {
            rb1.gravityScale = originalGravity1;
            rb1.linearVelocity = new Vector2(moveInput1.x * moveSpeed, rb1.linearVelocity.y);
        }

        // --- Player 2 Movement Logic ---
        if (isClimbing2)
        {
            rb2.gravityScale = 0f;
            rb2.linearVelocity = new Vector2(moveInput2.x * moveSpeed, verticalInput2 * climbSpeed);
        }
        else if (isOnRope2)
        {
            grabbedRope2.GetComponent<Rope>().ControlRope(moveInput2.x);
        }
        else
        {
            rb2.gravityScale = originalGravity2;
            rb2.linearVelocity = new Vector2(moveInput2.x * moveSpeed, rb2.linearVelocity.y);
        }
    }

    private void CheckGround(GameObject player)
    {
        if (player == player1)
        {
            if (groundCheck1 != null)
            {
                isGrounded1 = Physics2D.OverlapCircle(groundCheck1.position, groundCheckRadius, groundLayer);
            }
            else
            {
                isGrounded1 = Physics2D.Raycast(player1.transform.position, Vector2.down, 0.6f, groundLayer);
            }
        }
        else if (player == player2)
        {
            if (groundCheck2 != null)
            {
                isGrounded2 = Physics2D.OverlapCircle(groundCheck2.position, groundCheckRadius, groundLayer);
            }
            else
            {
                isGrounded2 = Physics2D.Raycast(player2.transform.position, Vector2.down, 0.6f, groundLayer);
            }
        }
    }

    public void GrabRope(GameObject Player, GameObject rope)
    {
        if (Player == player1)
        {
            isOnRope1 = true;
            grabbedRope1 = rope;
        }

        if(Player == player2)
        {
            isOnRope2 = true;
            grabbedRope2 = rope;
        }
    }

    public void LetGoRope(GameObject Player, GameObject rope)
    {
        rope.GetComponent<Rope>().LetGoRope();

        if (Player == player1)
        {
            isOnRope1 = false;
            grabbedRope1 = null;
        }

        if(Player == player2)
        {
            isOnRope2 = false;
            grabbedRope2 = null;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
        }
    }

    public void Die()
    {
        if (isDead1 || isDead2) return;

        isDead1 = true;
        isDead2 = true;

        moveInput1 = Vector2.zero;
        moveInput2 = Vector2.zero;
        verticalInput1 = 0f;
        verticalInput2 = 0f;

        rb1.linearVelocity = Vector2.zero;
        rb1.angularVelocity = 0f;

        rb2.linearVelocity = Vector2.zero;
        rb2.angularVelocity = 0f;

        StartCoroutine(RespawnAfterDelay(0.5f));

        Debug.Log("Player ist gestorben!");
    }

    public void SetCheckpoint(Vector3 checkpointPosition, GameObject player)
    {
        if (player == player1)
        {
            currentRespawnPosition1 = new Vector3(checkpointPosition.x, startPosition1.y, startPosition1.z);
        }
        else if (player == player2)
        {
            currentRespawnPosition2 = new Vector3(checkpointPosition.x, startPosition2.y, startPosition2.z);
        }
    }

    private System.Collections.IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        player1.transform.position = currentRespawnPosition1;
        player2.transform.position = currentRespawnPosition2;
        rb1.linearVelocity = Vector2.zero;
        rb1.angularVelocity = 0f;
        rb2.linearVelocity = Vector2.zero;
        rb2.angularVelocity = 0f;

        isDead1 = false;
        isDead2 = false;

        Debug.Log("Player ist respawnt und wieder steuerbar!");
    }
}