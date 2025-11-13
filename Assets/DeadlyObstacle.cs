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
        
        if (useTag && collision.CompareTag(playerTag))
        {
            PlayerMovementInputSystem player = collision.GetComponentInParent<PlayerMovementInputSystem>();
            if (player != null)
            {
                player.Die();
            }
        }
        else if (!useTag)
        {
            PlayerMovementInputSystem player = collision.GetComponentInParent<PlayerMovementInputSystem>();
            if (player != null)
            {
                player.Die();
            }
        }
    }
    
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
