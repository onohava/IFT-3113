using UnityEngine;

public class Finish : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool useTag = true;
    [SerializeField] private string playerTag = "Player";
    
    [Header("Visual Feedback")]
    [SerializeField] private bool changeColorOnActivation = true;
    [SerializeField] private Color completedColor = new Color(1f, 0.84f, 0f, 1f); // Gold
    
    private bool levelCompleted = false;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nur einmal auslösen
        if (levelCompleted) return;
        
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
            CompleteLevel();
        }
    }
    
    private void CompleteLevel()
    {
        levelCompleted = true;
        
        // Visuelles Feedback
        if (changeColorOnActivation && spriteRenderer != null)
        {
            spriteRenderer.color = completedColor;
        }
        
        Debug.Log("Level Complete! Finish erreicht.");
        
        // Benachrichtige den LevelCompleteManager (sucht auch in deaktivierten GameObjects)
        LevelCompleteManager manager = FindObjectOfType<LevelCompleteManager>(true);
        if (manager != null)
        {
            manager.ShowLevelComplete();
        }
        else
        {
            Debug.LogWarning("Finish: Kein LevelCompleteManager in der Szene gefunden!");
        }
    }
}
