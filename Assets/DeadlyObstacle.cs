using UnityEngine;

public class DeadlyObstacle : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool useTag = true;
    [SerializeField] private string playerTag = "Player";

    [Header("Optional")]
    [SerializeField] private bool useTrigger = true;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!useTrigger) return;
        
        // Prüfe ob es der Player ist
        if (useTag && collision.CompareTag(playerTag))
        {
            // Hole das PlayerMovementInputSystem und rufe Die() auf
            PlayerMovementInputSystem player = collision.GetComponentInParent<PlayerMovementInputSystem>();
            if (player != null)
            {
                player.Die();
            }
        }
        else if (!useTag)
        {
            // Alternative: Ohne Tag-Check
            PlayerMovementInputSystem player = collision.GetComponentInParent<PlayerMovementInputSystem>();
            if (player != null)
            {
                player.Die();
            }
        }
    }
    
    // Alternative: Für Collider statt Trigger
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (useTrigger) return;
        
        if (useTag && collision.gameObject.CompareTag(playerTag))
        {
            PlayerMovementInputSystem player = collision.gameObject.GetComponentInParent<PlayerMovementInputSystem>();
            if (player != null)
            {
                player.Die();
            }
        }
        else if (!useTag)
        {
            PlayerMovementInputSystem player = collision.gameObject.GetComponentInParent<PlayerMovementInputSystem>();
            if (player != null)
            {
                player.Die();
            }
        }
    }
}
