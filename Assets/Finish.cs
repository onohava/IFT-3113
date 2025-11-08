using UnityEngine;
using System.Collections.Generic; // Added this to use HashSet

public class Finish : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool useTag = true;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private int requiredPlayerCount = 2; // <-- New variable!
    
    [Header("Visual Feedback")]
    [SerializeField] private bool changeColorOnActivation = true;
    [SerializeField] private Color completedColor = new Color(1f, 0.84f, 0f, 1f); // Gold
    
    private bool levelCompleted = false;
    private SpriteRenderer spriteRenderer;

    // This HashSet will store all the unique players currently in the trigger
    private HashSet<Collider2D> playersInZone = new HashSet<Collider2D>();
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // A helper function to check if an object is a player
    // This is just your logic from OnTriggerEnter, moved here
    private bool IsPlayer(Collider2D collision)
    {
        if (useTag)
        {
            return collision.CompareTag(playerTag);
        }
        else
        {
            // Note: This check will be slow. CompareTag is much better!
            return collision.GetComponent<PlayerMovementInputSystem>() != null;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Don't do anything if the level is already finished
        if (levelCompleted) return;
        
        // Check if the object that entered is a player
        if (IsPlayer(collision))
        {
            // Add the player to our list
            playersInZone.Add(collision);

            // Check if we now have enough players to finish
            CheckForCompletion();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Don't do anything if the level is already finished
        if (levelCompleted) return;

        // Check if the object that LEFT is a player
        if (IsPlayer(collision))
        {
            // Remove the player from our list if they are in it
            if (playersInZone.Contains(collision))
            {
                playersInZone.Remove(collision);
            }
        }
    }

    private void CheckForCompletion()
    {
        // Do we have the required number of players?
        if (playersInZone.Count == requiredPlayerCount)
        {
            CompleteLevel();
        }
    }
    
    private void CompleteLevel()
    {
        levelCompleted = true;
        
        // Visual Feedback
        if (changeColorOnActivation && spriteRenderer != null)
        {
            spriteRenderer.color = completedColor;
        }
        
        Debug.Log("Level Complete! Finish erreicht.");
        
        // Notify the LevelCompleteManager
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