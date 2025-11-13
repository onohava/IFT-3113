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
        if (levelCompleteCanvas == null)
        {
            levelCompleteCanvas = gameObject;
            Debug.Log("LevelCompleteManager: Canvas automatically set to this GameObject.");
        }
        
        if (levelCompleteCanvas != null)
        {
            levelCompleteCanvas.SetActive(false);
            Debug.Log($"LevelCompleteManager: Canvas '{levelCompleteCanvas.name}' initially deactivated.");
        }
        
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RetryLevel);
            Debug.Log("LevelCompleteManager: Retry Button connected.");
        }
        else
        {
            Debug.LogWarning("LevelCompleteManager: Retry Button missing!");
        }
        
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(LoadNextLevel);
            Debug.Log("LevelCompleteManager: Next Level Button connected.");
        }
        else
        {
            Debug.LogWarning("LevelCompleteManager: Next Level Button missing!");
        }
        
        CheckIfLastLevel();
    }
    
    public void ShowLevelComplete()
    {
        Debug.Log("ShowLevelComplete() called!");
        
        if (isLevelComplete)
        {
            Debug.LogWarning("Level is already complete!");
            return;
        }
        
        isLevelComplete = true;
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string currentSceneName = SceneManager.GetActiveScene().name;
        LevelProgressManager.SetLevelCompleted(currentSceneName);
        Debug.Log($"Level '{currentSceneName}' (Index: {currentSceneIndex}) has been saved as completed!");
        
        Debug.Log($"Showing UI in {delayBeforeShow} seconds...");
        
        Invoke(nameof(DisplayUI), delayBeforeShow);
    }
    
    private void DisplayUI()
    {
        Debug.Log("DisplayUI() called!");
        
        if (levelCompleteCanvas != null)
        {
            Debug.Log($"Activating Canvas: {levelCompleteCanvas.name}");
            levelCompleteCanvas.SetActive(true);
            Debug.Log($"Canvas is now active: {levelCompleteCanvas.activeSelf}");
        }
        else
        {
            Debug.LogError("LevelCompleteManager: levelCompleteCanvas is NULL!");
        }
        
        if (pauseGameOnComplete)
        {
            Time.timeScale = 0f;
            Debug.Log("Game paused (Time.timeScale = 0)");
        }
        
        Debug.Log("Level Complete UI displayed");
    }
    
    public void RetryLevel()
    {
        Debug.Log("Retry Level...");
        
        Time.timeScale = 1f;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void LoadNextLevel()
    {
        Debug.Log("Loading next level...");
        
        Time.timeScale = 1f;
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No further level available! This was the last level.");
        }
    }
    
    private void CheckIfLastLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            if (nextLevelButton != null)
            {
                nextLevelButton.interactable = false;
                var buttonText = nextLevelButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = "Last Level!";
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        Time.timeScale = 1f;
        
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
