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
    
    private Rigidbody2D rb;
    private Vector3 startPosition;
    private Vector3 currentRespawnPosition;
    private float minXPosition;
    
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isDead = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        currentRespawnPosition = startPosition;
        minXPosition = startPosition.x;
    }
    
    private void Update()
    {
        CheckGround();
    }
    
    private void FixedUpdate()
    {
        if (isDead) return;
        
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        
        if (transform.position.x < minXPosition)
        {
            transform.position = new Vector3(minXPosition, transform.position.y, transform.position.z);
            rb.linearVelocity = new Vector2(Mathf.Max(0, rb.linearVelocity.x), rb.linearVelocity.y);
        }
    }
    
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    public void OnJump(InputValue value)
    {
        if (isDead) return;
        
        if (isGrounded && value.isPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    
    private void CheckGround()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Vector3 barrierStart = new Vector3(minXPosition, transform.position.y - 5f, transform.position.z);
            Vector3 barrierEnd = new Vector3(minXPosition, transform.position.y + 5f, transform.position.z);
            Gizmos.DrawLine(barrierStart, barrierEnd);
        }
    }
    
    public void Die()
    {
        if (isDead) return;
        
        isDead = true;
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        
        StartCoroutine(RespawnAfterDelay(0.5f));
        
        Debug.Log("Player died!");
    }
    
    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        currentRespawnPosition = new Vector3(checkpointPosition.x, startPosition.y, startPosition.z);
        minXPosition = checkpointPosition.x;
        
        Debug.Log($"New checkpoint set at X: {checkpointPosition.x}, Y: {startPosition.y} (Left boundary: {minXPosition})");
    }
    
    private System.Collections.IEnumerator RespawnAfterDelay(float delay)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0.3f;
            spriteRenderer.color = color;
        }
        
        yield return new WaitForSeconds(delay);
        
        transform.position = currentRespawnPosition;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
        
        isDead = false;
        
        Debug.Log("Player respawned and is controllable again!");
    }
}
