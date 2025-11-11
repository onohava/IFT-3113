using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Rigidbody2D))]
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
    //private float minXPosition; // Minimale X-Position (linke Grenze)

    // Input Values
    private Vector2 moveInput1;
    private Vector2 moveInput2;
    private float verticalInput1;
    private float verticalInput2;
    private PlayerInput playerInput;
    // public bool jumpPressed1; // REMOVED
    // public bool jumpPressed2; // REMOVED
    private bool isGrounded1;
    private bool isGrounded2;
    private bool isDead1 = false;
    private bool isDead2 = false;
    private bool isTouchingClimable1;
    private bool isTouchingClimable2;
    private bool isClimbing1;
    private bool isClimbing2;

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
        // Check if player is on the ground
        CheckGround(player1);
        CheckGround(player2);

        // CHANGED
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


        // Only jump if on the ground and button is pressed
        if (isGrounded1 && Keyboard.current.wKey.wasPressedThisFrame && !isClimbing1)
        {
            rb1.linearVelocity = new Vector2(rb1.linearVelocity.x, jumpForce);
        }

        if (isGrounded2 && Keyboard.current.upArrowKey.wasPressedThisFrame && !isClimbing2)
        {
            rb2.linearVelocity = new Vector2(rb2.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        // Keine Bewegung wenn der Player tot ist
        if (isDead1 || isDead2) return;

        // Player 1 Movement
        if (isClimbing1)
        {
            rb1.gravityScale = 0f;
            rb1.linearVelocity = new Vector2(moveInput1.x * moveSpeed, verticalInput1 * climbSpeed);
        }
        else
        {
            rb1.gravityScale = originalGravity1;
            rb1.linearVelocity = new Vector2(moveInput1.x * moveSpeed, rb1.linearVelocity.y);
        }

        // Player 2 Movement
        if (isClimbing2)
        {
            rb2.gravityScale = 0f;
            rb2.linearVelocity = new Vector2(moveInput2.x * moveSpeed, verticalInput2 * climbSpeed);
        }
        else
        {
            rb2.gravityScale = originalGravity2;
            rb2.linearVelocity = new Vector2(moveInput2.x * moveSpeed, rb2.linearVelocity.y);
        }


        // Verhindere Bewegung nach links über den letzten Checkpoint hinaus
        /*if (transform.position.x < minXPosition)
V {
V V transform.position = new Vector3(minXPosition, transform.position.y, transform.position.z);
V V rb.linearVelocity = new Vector2(Mathf.Max(0, rb.linearVelocity.x), rb.linearVelocity.y); // Stoppe linksseitige Geschwindigkeit
V }*/
    }

    private void CheckGround(GameObject player)
    {
        if (player == player1)
        {
            // Check if there's ground below the player
            if (groundCheck1 != null)
            {
                isGrounded1 = Physics2D.OverlapCircle(groundCheck1.position, groundCheckRadius, groundLayer);
            }
            else
            {
                // Fallback: simple raycast downward from player position
                isGrounded1 = Physics2D.Raycast(player1.transform.position, Vector2.down, 0.6f, groundLayer);
            }
        }
        else if (player == player2)
        {
            // Check if there's ground below the player
            if (groundCheck2 != null)
            {
                isGrounded2 = Physics2D.OverlapCircle(groundCheck2.position, groundCheckRadius, groundLayer);
            }
            else
            {
                // Fallback: simple raycast downward from player position
                isGrounded2 = Physics2D.Raycast(player2.transform.position, Vector2.down, 0.6f, groundLayer);
            }
        }


    }

    // Optional: Visualize ground check in editor
    private void OnDrawGizmosSelected()
    {
        // Zeige die linke Grenze (Checkpoint-Barriere) im Editor
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
        }
    }

    // Wird von DeadlyObstacle aufgerufen wenn der Player stirbt
    public void Die()
    {
        // Verhindere mehrfaches Sterben
        if (isDead1 || isDead2) return;

        // Player als tot markieren
        isDead1 = true;
        isDead2 = true;

        // Input komplett deaktivieren
        moveInput1 = Vector2.zero;
        moveInput2 = Vector2.zero;
        verticalInput1 = 0f;
        verticalInput2 = 0f;

        // Alle Bewegung stoppen
        rb1.linearVelocity = Vector2.zero;
        rb1.angularVelocity = 0f;

        rb2.linearVelocity = Vector2.zero;
        rb2.angularVelocity = 0f;

        // Verzögerter Respawn
        StartCoroutine(RespawnAfterDelay(0.5f));

        Debug.Log("Player ist gestorben!");
    }

    // Wird von CheckpointLine aufgerufen um einen neuen Checkpoint zu setzen
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
        //minXPosition = checkpointPosition.x;

        //Debug.Log($"Neuer Checkpoint gesetzt bei X: {checkpointPosition.x}, Y: {startPosition.y} (Linke Grenze: {minXPosition})");
    }

    private System.Collections.IEnumerator RespawnAfterDelay(float delay)
    {
        // Player unsichtbar/transparent machen
        // Warten vor dem Respawn
        yield return new WaitForSeconds(delay);

        // Zurücksetzen zur aktuellen Respawn-Position (Checkpoint oder Start)
        player1.transform.position = currentRespawnPosition1;
        player2.transform.position = currentRespawnPosition2;
        rb1.linearVelocity = Vector2.zero;
        rb1.angularVelocity = 0f;
        rb2.linearVelocity = Vector2.zero;
        rb2.angularVelocity = 0f;

        // Player ist wieder lebendig und steuerbar
        isDead1 = false;
        isDead2 = false;

        Debug.Log("Player ist respawnt und wieder steuerbar!");
    }
}