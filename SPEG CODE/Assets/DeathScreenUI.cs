using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class DeathScreenUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag TechGuy here.")]
    public PlayerStats playerStats;

    [Tooltip("The stats panel to hide when player dies.")]
    public GameObject statsPanel;

    [Tooltip("CanvasGroup on the full-screen dark overlay panel.")]
    public CanvasGroup overlayGroup;

    [Tooltip("The large red error message Text component.")]
    public TMP_Text messageText;

    [Tooltip("The Reinitialize button.")]
    public Button restartButton;

    [Header("Timing")]
    public float fadeDuration    = 0.75f;
    public float typewriterDelay = 0.035f;

    [Header("Death Messages")]
    public string[] deathMessages =
    {
        "ERROR: PROCESS TERMINATED",
        "ERROR: CONNECTION LOST",
        "ERROR: 404 HERO NOT FOUND"
    };

    private bool _shown;

    void Awake()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        // Hide overlay completely at start
        if (overlayGroup != null)
        {
            overlayGroup.alpha          = 0f;
            overlayGroup.blocksRaycasts = false;
            overlayGroup.interactable   = false;
        }

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartScene);
        }
    }

    void OnEnable()
    {
        if (playerStats != null)
            playerStats.PlayerDied += OnPlayerDied;
    }

    void OnDisable()
    {
        if (playerStats != null)
            playerStats.PlayerDied -= OnPlayerDied;
    }

    void OnPlayerDied()
    {
        if (_shown) return;
        _shown = true;
        if (statsPanel != null) statsPanel.SetActive(false);
        StartCoroutine(ShowDeathSequence());
    }

    IEnumerator ShowDeathSequence()
    {
        // Fade in dark overlay
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (overlayGroup != null)
                overlayGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        if (overlayGroup != null)
        {
            overlayGroup.alpha          = 1f;
            overlayGroup.blocksRaycasts = true;
            overlayGroup.interactable   = true;
        }

        // Typewriter message
        if (messageText != null)
        {
            string msg = deathMessages[Random.Range(0, deathMessages.Length)];
            messageText.text = "";
            foreach (char c in msg)
            {
                messageText.text += c;
                yield return new WaitForSeconds(typewriterDelay);
            }
        }

        // Show button
        if (restartButton != null)
            restartButton.gameObject.SetActive(true);
    }

    void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
