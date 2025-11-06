using UnityEngine;

public class Finish : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool useTag = true;
    [SerializeField] private string playerTag = "Player";
    
    [Header("Visual Feedback")]
    [SerializeField] private bool changeColorOnActivation = true;
    [SerializeField] private Color completedColor = new Color(1f, 0.84f, 0f, 1f);
    
    private bool levelCompleted = false;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (levelCompleted) return;
        
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
            CompleteLevel();
        }
    }
    
    private void CompleteLevel()
    {
        levelCompleted = true;
        
        if (changeColorOnActivation && spriteRenderer != null)
        {
            spriteRenderer.color = completedColor;
        }
        
        Debug.Log("Level Complete! Finish reached.");
        
        LevelCompleteManager manager = FindObjectOfType<LevelCompleteManager>(true);
        if (manager != null)
        {
            manager.ShowLevelComplete();
        }
        else
        {
            Debug.LogWarning("Finish: No LevelCompleteManager found in scene!");
        }
    }
}
