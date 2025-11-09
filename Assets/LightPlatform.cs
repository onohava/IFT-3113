using UnityEngine;

public class LightPlatform : MonoBehaviour
{
    // The color to change to when the player is touching
    public Color activatedColor = Color.green;
    
    // The light to turn on/off
    public GameObject lightToTurnOn;

    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get this object's SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Store the starting color so we can change it back
        originalColor = spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering is the "Player"
        if (other.CompareTag("Player"))
        {
            // 1. Change the platform's color
            spriteRenderer.color = activatedColor;

            // 2. Turn on the light
            if (lightToTurnOn != null)
            {
                print("Well this is weird");
                lightToTurnOn.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object leaving is the "Player"
        if (other.CompareTag("Player"))
        {
            // 1. Change the platform's color back to the original
            spriteRenderer.color = originalColor;

            // 2. Turn off the light
            if (lightToTurnOn != null)
            {
                lightToTurnOn.SetActive(false);
            }
        }
    }
}