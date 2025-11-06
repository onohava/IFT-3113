using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    [System.Serializable]
    public class LevelButton
    {
        public string levelName;          // z.B. "Level1"
        public Button button;              // Der Button selbst
        public GameObject completedIcon;   // Das Icon das angezeigt wird wenn abgeschlossen
    }
    
    [Header("Level Buttons")]
    [SerializeField] private LevelButton[] levelButtons;
    
    [Header("Debug")]
    [SerializeField] private bool resetProgressOnStart = false;
    
    private void Start()
    {
        // Debug: Fortschritt zurücksetzen (nur für Testing)
        if (resetProgressOnStart)
        {
            LevelProgressManager.ResetAllProgress();
            Debug.Log("Level-Fortschritt wurde zurückgesetzt!");
        }
        
        // Aktualisiere die UI für alle Level-Buttons
        UpdateLevelButtonsUI();
        
        // Debug: Zeige alle abgeschlossenen Levels
        LevelProgressManager.LogCompletedLevels();
    }
    
    private void UpdateLevelButtonsUI()
    {
        // Prüfe ob das Array überhaupt existiert
        if (levelButtons == null || levelButtons.Length == 0)
        {
            Debug.LogWarning("LevelSelection: Keine Level Buttons konfiguriert! Bitte im Inspector das 'Level Buttons' Array ausfüllen.");
            return;
        }
        
        foreach (var levelButton in levelButtons)
        {
            // Prüfe ob das Element null ist
            if (levelButton == null)
            {
                Debug.LogWarning("LevelSelection: Ein Level Button Element ist null!");
                continue;
            }
            
            if (levelButton.button == null)
            {
                Debug.LogWarning($"Button für '{levelButton.levelName}' ist nicht zugewiesen!");
                continue;
            }
            
            // Prüfe ob das Level abgeschlossen wurde
            bool isCompleted = LevelProgressManager.IsLevelCompleted(levelButton.levelName);
            
            // Aktiviere/Deaktiviere das Completed-Icon
            if (levelButton.completedIcon != null)
            {
                levelButton.completedIcon.SetActive(isCompleted);
                Debug.Log($"{levelButton.levelName}: {(isCompleted ? "✓ Abgeschlossen (Icon aktiviert)" : "○ Nicht abgeschlossen (Icon deaktiviert)")}");
            }
            else
            {
                Debug.LogWarning($"Completed Icon für '{levelButton.levelName}' ist nicht zugewiesen!");
            }
        }
    }
    
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
