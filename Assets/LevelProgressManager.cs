using UnityEngine;

public static class LevelProgressManager
{
    private const string LEVEL_COMPLETED_PREFIX = "Level_Completed_";
    
    public static void SetLevelCompleted(string levelName)
    {
        string key = LEVEL_COMPLETED_PREFIX + levelName;
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
        
        Debug.Log($"Level '{levelName}' marked as completed!");
    }
    
    public static void SetLevelCompletedByIndex(int sceneBuildIndex)
    {
        string sceneName = GetSceneNameFromBuildIndex(sceneBuildIndex);
        if (!string.IsNullOrEmpty(sceneName))
        {
            SetLevelCompleted(sceneName);
        }
    }
    
    public static bool IsLevelCompleted(string levelName)
    {
        string key = LEVEL_COMPLETED_PREFIX + levelName;
        return PlayerPrefs.GetInt(key, 0) == 1;
    }
    
    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All level progress has been reset!");
    }
    
    private static string GetSceneNameFromBuildIndex(int buildIndex)
    {
        string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(buildIndex);
        if (string.IsNullOrEmpty(path))
            return null;
        
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
        return sceneName;
    }
    
    public static void LogCompletedLevels()
    {
        Debug.Log("=== Completed Levels ===");
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            string sceneName = GetSceneNameFromBuildIndex(i);
            if (!string.IsNullOrEmpty(sceneName) && sceneName.Contains("Level"))
            {
                bool completed = IsLevelCompleted(sceneName);
                Debug.Log($"{sceneName}: {(completed ? "✓ Completed" : "○ Not completed")}");
            }
        }
    }
}
