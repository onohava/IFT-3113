using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementInputSystem : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    
    // Components
    private Rigidbody2D rb;
    private Vector3 startPosition;
    private Vector3 currentRespawnPosition;
    private float minXPosition; // Minimale X-Position (linke Grenze)
    
    // Input Values
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isDead = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        currentRespawnPosition = startPosition; // Initial ist die Respawn-Position die Startposition
        minXPosition = startPosition.x; // Initial kann der Player nicht weiter links als die Startposition
    }
    
    private void Update()
    {
        // Check if player is on the ground
        CheckGround();
    }
    
    private void FixedUpdate()
    {
        // Keine Bewegung wenn der Player tot ist
        if (isDead) return;
        
        // Apply horizontal movement (A = links, D = rechts)
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        
        // Verhindere Bewegung nach links über den letzten Checkpoint hinaus
        if (transform.position.x < minXPosition)
        {
            transform.position = new Vector3(minXPosition, transform.position.y, transform.position.z);
            rb.linearVelocity = new Vector2(Mathf.Max(0, rb.linearVelocity.x), rb.linearVelocity.y); // Stoppe linksseitige Geschwindigkeit
        }
    }
    
    // Called by Unity Input System (PlayerInput component)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    // Called by Unity Input System (PlayerInput component)
    public void OnJump(InputValue value)
    {
        // Kein Springen wenn der Player tot ist
        if (isDead) return;
        
        // Only jump if on the ground and button is pressed
        if (isGrounded && value.isPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    
    private void CheckGround()
    {
        // Check if there's ground below the player
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            // Fallback: simple raycast downward from player position
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);
        }
    }
    
    // Optional: Visualize ground check in editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        
        // Zeige die linke Grenze (Checkpoint-Barriere) im Editor
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Vector3 barrierStart = new Vector3(minXPosition, transform.position.y - 5f, transform.position.z);
            Vector3 barrierEnd = new Vector3(minXPosition, transform.position.y + 5f, transform.position.z);
            Gizmos.DrawLine(barrierStart, barrierEnd);
        }
    }
    
    // Wird von DeadlyObstacle aufgerufen wenn der Player stirbt
    public void Die()
    {
        // Verhindere mehrfaches Sterben
        if (isDead) return;
        
        // Player als tot markieren
        isDead = true;
        
        // Input komplett deaktivieren
        moveInput = Vector2.zero;
        
        // Alle Bewegung stoppen
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        
        // Verzögerter Respawn
        StartCoroutine(RespawnAfterDelay(0.5f));
        
        Debug.Log("Player ist gestorben!");
    }
    
    // Wird von CheckpointLine aufgerufen um einen neuen Checkpoint zu setzen
    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        // Übernimmt nur die X-Position vom Checkpoint, Y-Position bleibt wie am Start
        currentRespawnPosition = new Vector3(checkpointPosition.x, startPosition.y, startPosition.z);
        
        // Setze neue linke Grenze - Player kann nicht mehr weiter links als dieser Checkpoint
        minXPosition = checkpointPosition.x;
        
        Debug.Log($"Neuer Checkpoint gesetzt bei X: {checkpointPosition.x}, Y: {startPosition.y} (Linke Grenze: {minXPosition})");
    }
    
    private System.Collections.IEnumerator RespawnAfterDelay(float delay)
    {
        // Player unsichtbar/transparent machen
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0.3f;
            spriteRenderer.color = color;
        }
        
        // Warten vor dem Respawn
        yield return new WaitForSeconds(delay);
        
        // Zurücksetzen zur aktuellen Respawn-Position (Checkpoint oder Start)
        transform.position = currentRespawnPosition;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        
        // Player wieder sichtbar machen
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
        
        // Player ist wieder lebendig und steuerbar
        isDead = false;
        
        Debug.Log("Player ist respawnt und wieder steuerbar!");
    }
}
