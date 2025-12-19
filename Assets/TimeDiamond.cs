using UnityEngine;

public class TimeDiamond : MonoBehaviour
{
    [Header("Settings")]
    public float timeBonus = 10f; 
    
    [Tooltip("Player to pick up")]
    public GameObject allowedPlayer; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Check if the object hitting us is EXACTLY the one we assigned
        if (collision.gameObject == allowedPlayer)
        {
            // 2. Find the timer anywhere in the scene
            GameTimer timer = FindObjectOfType<GameTimer>();

            if (timer != null)
            {
                timer.AddTime(timeBonus);
                Destroy(gameObject); // Collect the item
            }
        }
    }
}