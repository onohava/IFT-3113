using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Header("Settings")]
    public float timeLimit = 60f;
    public float shakeThreshold = 10f; 
    
    [Header("UI & Feedback")]
    public TextMeshProUGUI timerText;


    private float currentTime;
    private bool isGameOver = false;

    void Start()
    {
        currentTime = timeLimit;
    }

    void Update()
    {
        if (isGameOver) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateUI();

            if (currentTime <= shakeThreshold)
            {
                // Intensity increases as time nears zero
                float intensity = 1f - (currentTime / shakeThreshold);
                timerText.color = Color.red;
            }
        }
        else
        {
            RestartLevel();
        }
    }

    void UpdateUI()
    {
        // Mathf.Max(0, ...) ensures the timer never displays negative numbers
        int seconds = Mathf.CeilToInt(Mathf.Max(0, currentTime));

        // The "D3" or "000" format forces the string to always have 3 digits
        timerText.text = seconds.ToString("000");

        // B&W Glitch Effect when low on time
        if (currentTime <= shakeThreshold)
        {
            float glitch = 3f;
            timerText.rectTransform.anchoredPosition = new Vector2(
                Random.Range(-glitch, glitch), 
                Random.Range(-glitch, glitch)
            );
            timerText.color = (Random.value > 0.1f) ? Color.white : Color.black;
        }
        else
        {
            // Reset position and color if time is added back (e.g. from crystals later)
            timerText.rectTransform.anchoredPosition = Vector2.zero;
            timerText.color = Color.white;
        }
    }

    public void AddTime(float amount)
    {
        currentTime += amount;
        StartCoroutine(FlashColor(Color.green));
    }

    private System.Collections.IEnumerator FlashColor(Color flashColor)
    {
        timerText.color = flashColor;
        yield return new WaitForSeconds(0.5f);
        timerText.color = (currentTime <= shakeThreshold) ? Color.red : Color.white;
    }

    public void RestartLevel()
    {
        isGameOver = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}