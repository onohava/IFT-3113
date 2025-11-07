using UnityEngine;

public class CheckpointLine : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Color inactiveColor = new Color(0.5f, 1f, 0.5f, 0.2f); // Weiß, halbtransparent
    [SerializeField] private Color activeColor = new Color(0.5f, 1f, 0.5f, 0.6f); 
    [SerializeField] private float fadeSpeed = 2f;
    
    [Header("Checkpoint Settings")]
    [SerializeField] private bool useTag = true;
    [SerializeField] private string playerTag = "Player";
    
    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;
    private bool isFading = false;
    private float fadeProgress = 0f;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            Debug.LogError($"CheckpointLine '{gameObject.name}': Kein SpriteRenderer gefunden!");
        }
        else
        {
            // Setze initiale Farbe
            spriteRenderer.color = inactiveColor;
        }
    }
    
    private void Update()
    {
        // Smooth Fade-Effekt beim Aktivieren
        if (isFading && spriteRenderer != null)
        {
            fadeProgress += Time.deltaTime * fadeSpeed;
            spriteRenderer.color = Color.Lerp(inactiveColor, activeColor, fadeProgress);
            
            if (fadeProgress >= 1f)
            {
                isFading = false;
                spriteRenderer.color = activeColor;
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nur beim ersten Mal aktivieren
        //if (isActivated) return;
        
        // Prüfe ob es der Player ist
        bool isPlayer = false;
        
        if (useTag)
        {
            isPlayer = collision.CompareTag(playerTag);
        }
        else
        {
            isPlayer = collision.GetComponent<PlayerMovementInputSystem>() != null;
        }
        
        if (isPlayer)
        {
            ActivateCheckpoint(collision.gameObject);
        }
    }
    
    private void ActivateCheckpoint(GameObject player)
    {
        // Markiere als aktiviert
        isActivated = true;
        
        // Starte Farbwechsel
        isFading = true;
        fadeProgress = 0f;
        
        // Informiere den Player über den neuen Checkpoint
        PlayerMovementInputSystem playerMovement = player.GetComponentInParent<PlayerMovementInputSystem>();
        if (playerMovement != null)
        {
            playerMovement.SetCheckpoint(transform.position, player);
            Debug.Log($"Checkpoint '{gameObject.name}' aktiviert an Position: {transform.position}");
        }
        else
        {
            Debug.LogWarning($"CheckpointLine '{gameObject.name}': PlayerMovementInputSystem nicht gefunden!");
        }
    }
    
    // Optional: Zum Debugging - zeigt den Status in der Scene View
    private void OnDrawGizmos()
    {
        Gizmos.color = isActivated ? Color.green : Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.2f, 1f, 0f));
    }
}
