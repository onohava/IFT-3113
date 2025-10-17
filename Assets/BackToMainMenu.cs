using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour
{
    public void Go() {
        SceneManager.LoadScene("MainMenu");
    }
}
