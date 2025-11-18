using UnityEngine;
using System.Collections.Generic;

public class Finish : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool useTag = true;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private int requiredPlayerCount = 2;
    
    [Header("Visual Feedback")]
    [SerializeField] private bool changeColorOnActivation = true;
    [SerializeField] private Color completedColor = new Color(1f, 0.84f, 0f, 1f);
    
    private bool levelCompleted = false;
    private SpriteRenderer spriteRenderer;

    private HashSet<Collider2D> playersInZone = new HashSet<Collider2D>();
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private bool IsPlayer(Collider2D collision)
    {
        if (useTag)
        {
            return collision.CompareTag(playerTag);
        }
        else
        {
            return collision.GetComponent<PlayerMovementInputSystem>() != null;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (levelCompleted) return;
        
        if (IsPlayer(collision))
        {
            playersInZone.Add(collision);

            CheckForCompletion();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (levelCompleted) return;

        if (IsPlayer(collision))
        {
            if (playersInZone.Contains(collision))
            {
                playersInZone.Remove(collision);
            }
        }
    }

    private void CheckForCompletion()
    {
        if (playersInZone.Count == requiredPlayerCount)
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
                
        LevelCompleteManager manager = FindObjectOfType<LevelCompleteManager>(true);
        if (manager != null)
        {
            manager.ShowLevelComplete();
        }
        else
        {
            Debug.LogWarning("Finish: No LevelCompleteManager found!");
        }
    }
}