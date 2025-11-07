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
    
    // Components
    private Rigidbody2D rb1;
    private Rigidbody2D rb2;
    private Vector3 startPosition1;
    private Vector3 startPosition2;
    private Vector3 currentRespawnPosition1;
    private Vector3 currentRespawnPosition2;
    //private float minXPosition; // Minimale X-Position (linke Grenze)

    // Input Values
    private Vector2 moveInput1;
    private Vector2 moveInput2;
    private PlayerInput playerInput;
    public bool jumpPressed1;
    public bool jumpPressed2;
    private bool isGrounded1;
    private bool isGrounded2;
    private bool isDead1 = false;
    private bool isDead2 = false;

    private InputAction moveAction;
    private InputAction jumpAction;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb1 = player1.GetComponent<Rigidbody2D>();
        startPosition1 = player1.transform.position;
        currentRespawnPosition1 = startPosition1;

        rb2 = player2.GetComponent<Rigidbody2D>();
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
        moveInput1 = new Vector2(
            (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0),0);   
        moveInput2 = new Vector2(
            (Keyboard.current.rightArrowKey.isPressed ? 1 : 0) - (Keyboard.current.leftArrowKey.isPressed ? 1 : 0),0);

        // Only jump if on the ground and button is pressed
        if (isGrounded1 && Keyboard.current.wKey.wasPressedThisFrame)
        {
            rb1.linearVelocity = new Vector2(rb1.linearVelocity.x, jumpForce);
        }

        if (isGrounded2 && Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            rb2.linearVelocity = new Vector2(rb2.linearVelocity.x, jumpForce);
        }
    }
    
    private void FixedUpdate()
    {
        // Keine Bewegung wenn der Player tot ist
        if (isDead1 || isDead2) return;
        
        // Apply horizontal movement (A = links, D = rechts)
        rb1.linearVelocity = new Vector2(moveInput1.x * moveSpeed, rb1.linearVelocity.y);
        rb2.linearVelocity = new Vector2(moveInput2.x * moveSpeed, rb2.linearVelocity.y);


        // Verhindere Bewegung nach links über den letzten Checkpoint hinaus
        /*if (transform.position.x < minXPosition)
        {
            transform.position = new Vector3(minXPosition, transform.position.y, transform.position.z);
            rb.linearVelocity = new Vector2(Mathf.Max(0, rb.linearVelocity.x), rb.linearVelocity.y); // Stoppe linksseitige Geschwindigkeit
        }*/
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
        /*if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }*/
        
        // Zeige die linke Grenze (Checkpoint-Barriere) im Editor
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            /*Vector3 barrierStart = new Vector3(minXPosition, transform.position.y - 5f, transform.position.z);
            Vector3 barrierEnd = new Vector3(minXPosition, transform.position.y + 5f, transform.position.z);*/
           // Gizmos.DrawLine(barrierStart, barrierEnd);
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
        /*SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0.3f;
            spriteRenderer.color = color;
        }*/
        
        // Warten vor dem Respawn
        yield return new WaitForSeconds(delay);
        
        // Zurücksetzen zur aktuellen Respawn-Position (Checkpoint oder Start)
        player1.transform.position = currentRespawnPosition1;
        player2.transform.position = currentRespawnPosition2;
        rb1.linearVelocity = Vector2.zero;
        rb1.angularVelocity = 0f;
        rb2.linearVelocity = Vector2.zero;
        rb2.angularVelocity = 0f;

        // Player wieder sichtbar machen
        /*if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }*/

        // Player ist wieder lebendig und steuerbar
        isDead1 = false;
        isDead2 = false;
        
        Debug.Log("Player ist respawnt und wieder steuerbar!");
    }
}
