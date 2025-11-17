using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class PlayerMovementInputSystem : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck1;
    [SerializeField] private Transform groundCheck2;
                     private float groundCheckRadius = 0.04f;
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
    private float originalDamping;

    // Input Values
    private Vector2 moveInput1;
    private Vector2 moveInput2;
    private float verticalInput1;
    private float verticalInput2;
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
    private bool isJumpWindowActive1 = false;
    private bool isJumpWindowActive2 = false;

    private bool movePressed1;
    private bool jumpPressed1;
    private bool jumpHold1;
    private bool movePressed2;
    private bool jumpPressed2;
    private bool jumpHold2;

    private void Awake()
    {      
        rb1 = player1.GetComponent<Rigidbody2D>();
        originalGravity1 = rb1.gravityScale;
        startPosition1 = player1.transform.position;
        currentRespawnPosition1 = startPosition1;

        rb2 = player2.GetComponent<Rigidbody2D>();
        originalGravity2 = rb2.gravityScale;
        startPosition2 = player2.transform.position;
        currentRespawnPosition2 = startPosition2;

        originalDamping = rb1.linearDamping;
    }

    private void Update()
    {
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

        if(Keyboard.current.wKey.wasPressedThisFrame)
            jumpPressed1 = true;
        if(Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
            movePressed1 = true;
        jumpHold1 = Keyboard.current.wKey.isPressed;

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            jumpPressed2 = true;
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
            movePressed2 = true;
        jumpHold2 = Keyboard.current.upArrowKey.isPressed;
    }

    private void FixedUpdate()
    {
        CheckGround(player1);
        CheckGround(player2);
        if (isDead1 || isDead2) return;

        // --- Player 1 Jump Logic ---

        if (isGrounded1 || isClimbing1 && isJumpWindowActive1)
        {
            StopCoroutine(JumpHoldWindow1());
            isJumpWindowActive1 = false;
        }
        if (isJumpWindowActive1 && jumpHold1)
        {
            rb1.AddForceY(jumpForce*3.5f, ForceMode2D.Force);
        }

        if (jumpPressed1)
        {
            if (isGrounded1 && !isClimbing1 && !isOnRope1)
            {
                rb1.AddForceY(jumpForce / 2, ForceMode2D.Impulse);
                StartCoroutine(JumpHoldWindow1());
            }
            else if (isOnRope1)
            {
                LetGoRope(player1, grabbedRope1);
                rb1.AddForceY(jumpForce / 2, ForceMode2D.Impulse);
                StartCoroutine(JumpHoldWindow1());
            }
            jumpPressed1 = false;
        }

        // --- Player 1 Movement Logic ---
        // This adds a little impulse at the beginning of movement
        if (movePressed1)
        {
            rb1.linearVelocityX = moveInput1.x;
            movePressed1 = false;
        }

        if (isClimbing1)
        {
            rb1.gravityScale = 0f;
            rb1.AddForceY(verticalInput1 * jumpForce, ForceMode2D.Force);
            rb1.AddForceX(moveInput1.x * moveSpeed/10, ForceMode2D.Force);
        }
        else if (isOnRope1)
        {
            grabbedRope1.GetComponent<Rope>().ControlRope(moveInput1.x);
        }
        else
        {
            rb1.gravityScale = originalGravity1;
            rb1.AddForceX(moveInput1.x * moveSpeed, ForceMode2D.Force);
        }


        // --- Player 2 Jump Logic ---
        if (isGrounded2 && isJumpWindowActive2)
        {
            StopCoroutine(JumpHoldWindow2());
            isJumpWindowActive2 = false;
        }
        if (isJumpWindowActive2 && jumpHold2)
        {
            rb2.AddForceY(jumpForce, ForceMode2D.Force);
        }

        if (jumpPressed2)
        {
            if (isGrounded2 && !isClimbing2 && !isOnRope2)
            {
                rb2.AddForceY(jumpForce / 5, ForceMode2D.Impulse);
                StartCoroutine(JumpHoldWindow2());
            }
            else if (isOnRope2)
            {
                LetGoRope(player2, grabbedRope2);
                rb2.AddForceY(jumpForce / 5, ForceMode2D.Impulse);
                StartCoroutine(JumpHoldWindow2());
            }
            jumpPressed2 = false;
        }

        // --- Player 2 Movement Logic ---
        if (movePressed2)
        {
            rb2.linearVelocityX = moveInput2.x;
            movePressed2 = false;
        }

        if (isClimbing2)
        {
            rb2.gravityScale = 0f;
            rb2.AddForceY(verticalInput2 * jumpForce/2, ForceMode2D.Force);
            rb2.AddForceX(moveInput2.x * moveSpeed / 10, ForceMode2D.Force);
        }
        else if (isOnRope2)
        {
            grabbedRope2.GetComponent<Rope>().ControlRope(moveInput2.x);
        }
        else 
        {
            rb2.gravityScale = originalGravity2;
            rb2.AddForceX(moveInput2.x * moveSpeed/3, ForceMode2D.Force);
        }
    }

    private void CheckGround(GameObject player)
    {
        if (player == player1)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(groundCheck1.position, new Vector2(0.12f, 0.02f), 0, groundLayer);

            if (hits != null)
            {
                bool groundFound = false;
                foreach (Collider2D col in hits)
                {
                    if (col.gameObject != player1)
                    {
                        groundFound = true;
                        break;
                    }
                }
                isGrounded1 = groundFound;
            }
        }
        else if (player == player2)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(groundCheck2.position, new Vector2(0.12f, 0.02f), 0, groundLayer);
            if (hits != null)
            {
                bool groundFound = false;
                foreach (Collider2D col in hits)
                {
                    if (col.gameObject != player2)
                    {
                        groundFound = true;
                        break;
                    }
                }
                isGrounded2 = groundFound;
            }
        }
    }

    public void GrabRope(GameObject Player, GameObject rope)
    {
        if (Player == player1)
        {
            isOnRope1 = true;
            grabbedRope1 = rope;
            rb1.linearDamping = 0;
        }

        if(Player == player2)
        {
            isOnRope2 = true;
            grabbedRope2 = rope;
            rb2.linearDamping = 0;
        }
    }

    public void LetGoRope(GameObject Player, GameObject rope)
    {
        rope.GetComponent<Rope>().LetGoRope();

        if (Player == player1)
        {
            isOnRope1 = false;
            grabbedRope1 = null;
            rb1.linearDamping = originalDamping;
        }

        if(Player == player2)
        {
            isOnRope2 = false;
            grabbedRope2 = null;
            rb2.linearDamping = originalDamping;
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
        player2.transform.position = new Vector2(currentRespawnPosition2.x, currentRespawnPosition2.y + 0.16f);
        rb1.linearVelocity = Vector2.zero;
        rb1.angularVelocity = 0f;
        rb2.linearVelocity = Vector2.zero;
        rb2.angularVelocity = 0f;

        isDead1 = false;
        isDead2 = false;
    }

    private System.Collections.IEnumerator JumpHoldWindow1()
    {
        isJumpWindowActive1 = true;
        yield return new WaitForSeconds(0.25f);
        isJumpWindowActive1 = false;
    }

    private System.Collections.IEnumerator JumpHoldWindow2()
    {
        isJumpWindowActive2 = true;
        yield return new WaitForSeconds(0.25f);
        isJumpWindowActive2 = false;
    }
}