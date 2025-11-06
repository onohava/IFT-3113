using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject levelCompleteCanvas;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button nextLevelButton;
    
    [Header("Settings")]
    [SerializeField] private bool pauseGameOnComplete = true;
    [SerializeField] private float delayBeforeShow = 0.5f;
    
    private bool isLevelComplete = false;
    
    private void Start()
    {
        // Falls kein Canvas zugewiesen wurde, nehme das GameObject auf dem dieses Skript ist
        if (levelCompleteCanvas == null)
        {
            levelCompleteCanvas = gameObject;
            Debug.Log("LevelCompleteManager: Canvas wurde automatisch auf dieses GameObject gesetzt.");
        }
        
        // Stelle sicher, dass das Canvas initial versteckt ist
        if (levelCompleteCanvas != null)
        {
            levelCompleteCanvas.SetActive(false);
            Debug.Log($"LevelCompleteManager: Canvas '{levelCompleteCanvas.name}' initial deaktiviert.");
        }
        
        // Verbinde Button-Events
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RetryLevel);
            Debug.Log("LevelCompleteManager: Retry Button verbunden.");
        }
        else
        {
            Debug.LogWarning("LevelCompleteManager: Retry Button fehlt!");
        }
        
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(LoadNextLevel);
            Debug.Log("LevelCompleteManager: Next Level Button verbunden.");
        }
        else
        {
            Debug.LogWarning("LevelCompleteManager: Next Level Button fehlt!");
        }
        
        // Prüfe ob es ein nächstes Level gibt
        CheckIfLastLevel();
    }
    
    public void ShowLevelComplete()
    {
        Debug.Log("ShowLevelComplete() aufgerufen!");
        
        if (isLevelComplete)
        {
            Debug.LogWarning("Level ist bereits komplett!");
            return;
        }
        
        isLevelComplete = true;
        
        // Speichere dass dieses Level abgeschlossen wurde
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string currentSceneName = SceneManager.GetActiveScene().name;
        LevelProgressManager.SetLevelCompleted(currentSceneName);
        Debug.Log($"Level '{currentSceneName}' (Index: {currentSceneIndex}) wurde als abgeschlossen gespeichert!");
        
        Debug.Log($"Zeige UI in {delayBeforeShow} Sekunden...");
        
        // Zeige UI mit kleiner Verzögerung
        Invoke(nameof(DisplayUI), delayBeforeShow);
    }
    
    private void DisplayUI()
    {
        Debug.Log("DisplayUI() aufgerufen!");
        
        if (levelCompleteCanvas != null)
        {
            Debug.Log($"Aktiviere Canvas: {levelCompleteCanvas.name}");
            levelCompleteCanvas.SetActive(true);
            Debug.Log($"Canvas ist jetzt aktiv: {levelCompleteCanvas.activeSelf}");
        }
        else
        {
            Debug.LogError("LevelCompleteManager: levelCompleteCanvas ist NULL!");
        }
        
        // Pausiere das Spiel (optional)
        if (pauseGameOnComplete)
        {
            Time.timeScale = 0f;
            Debug.Log("Spiel pausiert (Time.timeScale = 0)");
        }
        
        Debug.Log("Level Complete UI angezeigt");
    }
    
    public void RetryLevel()
    {
        Debug.Log("Retry Level...");
        
        // Setze Time Scale zurück
        Time.timeScale = 1f;
        
        // Lade die aktuelle Szene neu
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void LoadNextLevel()
    {
        Debug.Log("Lade nächstes Level...");
        
        // Setze Time Scale zurück
        Time.timeScale = 1f;
        
        // Hole den Index der nächsten Szene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        
        // Prüfe ob es noch eine weitere Szene gibt
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Kein weiteres Level vorhanden! Dies war das letzte Level.");
            // Optional: Zurück zum Hauptmenü oder Level Selection
            // SceneManager.LoadScene("MainMenu");
        }
    }
    
    private void CheckIfLastLevel()
    {
        // Prüfe ob dies das letzte Level ist
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            // Dies ist das letzte Level - deaktiviere Next Level Button
            if (nextLevelButton != null)
            {
                nextLevelButton.interactable = false;
                // Optional: Ändere den Text zu "Letztes Level!"
                var buttonText = nextLevelButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = "Letztes Level!";
                }
            }
        }
    }
    
    // Cleanup wenn das Skript zerstört wird
    private void OnDestroy()
    {
        // Stelle sicher, dass Time Scale zurückgesetzt wird
        Time.timeScale = 1f;
        
        // Entferne Button-Listener
        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(RetryLevel);
        }
        
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveListener(LoadNextLevel);
        }
    }
}

