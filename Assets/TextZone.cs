using UnityEngine;
using TMPro; // Use 'using UnityEngine.UI;' if you are not using TextMeshPro

public class TextZone : MonoBehaviour
{
    [Header("What should this zone say?")]
    [TextArea] // Makes the box bigger in Inspector for easy typing
    public string message = "Write your text here...";

    [Header("Drag your ONE UI Text object here")]
    public TextMeshProUGUI sharedTextObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Change the text to match THIS zone's message
            sharedTextObject.text = message;

            // 2. Make sure it is visible
            sharedTextObject.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Hide the text when leaving
            sharedTextObject.gameObject.SetActive(false);
        }
    }
}