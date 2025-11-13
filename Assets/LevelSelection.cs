using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    [System.Serializable]
    public class LevelButton
    {
        public string levelName;
        public Button button;
        public GameObject completedIcon;
    }
    
    [Header("Level Buttons")]
    [SerializeField] private LevelButton[] levelButtons;
    
    [Header("Debug")]
    [SerializeField] private bool resetProgressOnStart = false;
    
    private void Start()
    {
        if (resetProgressOnStart)
        {
            LevelProgressManager.ResetAllProgress();
            Debug.Log("Level progress has been reset!");
        }
        
        UpdateLevelButtonsUI();
        
        LevelProgressManager.LogCompletedLevels();
    }
    
    private void UpdateLevelButtonsUI()
    {
        if (levelButtons == null || levelButtons.Length == 0)
        {
            Debug.LogWarning("LevelSelection: No Level Buttons configured! Please fill the 'Level Buttons' array in the Inspector.");
            return;
        }
        
        foreach (var levelButton in levelButtons)
        {
            if (levelButton == null)
            {
                Debug.LogWarning("LevelSelection: A Level Button element is null!");
                continue;
            }
            
            if (levelButton.button == null)
            {
                Debug.LogWarning($"Button for '{levelButton.levelName}' is not assigned!");
                continue;
            }
            
            bool isCompleted = LevelProgressManager.IsLevelCompleted(levelButton.levelName);
            
            if (levelButton.completedIcon != null)
            {
                levelButton.completedIcon.SetActive(isCompleted);
                Debug.Log($"{levelButton.levelName}: {(isCompleted ? "✓ Completed (Icon activated)" : "○ Not completed (Icon deactivated)")}");
            }
            else
            {
                Debug.LogWarning($"Completed Icon for '{levelButton.levelName}' is not assigned!");
            }
        }
    }
    
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
