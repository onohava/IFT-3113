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
        }
        
        UpdateLevelButtonsUI();
        LevelProgressManager.LogCompletedLevels();
    }
    
    private void UpdateLevelButtonsUI()
    {
        if (levelButtons == null || levelButtons.Length == 0)
        {
            Debug.LogWarning("LevelSelection: No level buttons configured!");
            return;
        }
        
        foreach (var levelButton in levelButtons)
        {
            if (levelButton == null)
            {
                Debug.LogWarning("LevelSelection: A level button element is null!");
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
            }
            else
            {
                Debug.LogWarning($"Completed icon for '{levelButton.levelName}' is not assigned!");
            }
        }
    }
    
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
