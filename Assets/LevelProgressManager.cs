using UnityEngine;

/// <summary>
/// Verwaltet den Fortschritt aller Levels (welche abgeschlossen wurden)
/// Verwendet PlayerPrefs für persistente Speicherung
/// </summary>
public static class LevelProgressManager
{
    private const string LEVEL_COMPLETED_PREFIX = "Level_Completed_";
    
    /// <summary>
    /// Markiert ein Level als abgeschlossen
    /// </summary>
    /// <param name="levelName">Name der Szene (z.B. "Level1")</param>
    public static void SetLevelCompleted(string levelName)
    {
        string key = LEVEL_COMPLETED_PREFIX + levelName;
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
        
        Debug.Log($"Level '{levelName}' als abgeschlossen markiert!");
    }
    
    /// <summary>
    /// Markiert ein Level anhand des Scene Build Index als abgeschlossen
    /// </summary>
    /// <param name="sceneBuildIndex">Build Index der Szene</param>
    public static void SetLevelCompletedByIndex(int sceneBuildIndex)
    {
        string sceneName = GetSceneNameFromBuildIndex(sceneBuildIndex);
        if (!string.IsNullOrEmpty(sceneName))
        {
            SetLevelCompleted(sceneName);
        }
    }
    
    /// <summary>
    /// Prüft ob ein Level abgeschlossen wurde
    /// </summary>
    /// <param name="levelName">Name der Szene (z.B. "Level1")</param>
    /// <returns>True wenn abgeschlossen, sonst False</returns>
    public static bool IsLevelCompleted(string levelName)
    {
        string key = LEVEL_COMPLETED_PREFIX + levelName;
        return PlayerPrefs.GetInt(key, 0) == 1;
    }
    
    /// <summary>
    /// Setzt den Fortschritt aller Levels zurück (für Debugging)
    /// </summary>
    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Aller Level-Fortschritt wurde zurückgesetzt!");
    }
    
    /// <summary>
    /// Gibt den Szenen-Namen für einen Build Index zurück
    /// </summary>
    private static string GetSceneNameFromBuildIndex(int buildIndex)
    {
        string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(buildIndex);
        if (string.IsNullOrEmpty(path))
            return null;
        
        // Extrahiere den Szenen-Namen aus dem Pfad
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
        return sceneName;
    }
    
    /// <summary>
    /// Debug-Funktion: Zeigt alle abgeschlossenen Levels
    /// </summary>
    public static void LogCompletedLevels()
    {
        Debug.Log("=== Abgeschlossene Levels ===");
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            string sceneName = GetSceneNameFromBuildIndex(i);
            if (!string.IsNullOrEmpty(sceneName) && sceneName.Contains("Level"))
            {
                bool completed = IsLevelCompleted(sceneName);
                Debug.Log($"{sceneName}: {(completed ? "✓ Abgeschlossen" : "○ Nicht abgeschlossen")}");
            }
        }
    }
}

