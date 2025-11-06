using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    public void PlayGame() {
        SceneManager.LoadScene("Level1");
    }

    public void LoadLevelSelection() {
        SceneManager.LoadScene("LevelSelection");
    }
    
    public void QuitGame() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    void Update() {
        // Space gedrückt in diesem Frame?
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) {
            PlayGame();
        }
    }
}
