using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreenUI : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private GameObject _statsPanel;
    [SerializeField] private CanvasGroup _overlayGroup;
    [SerializeField] private Text _messageText;
    [SerializeField] private Button _restartButton;
    [SerializeField] private float _fadeDuration = 0.6f;
    [SerializeField] private float _typewriterDelay = 0.03f;
    [SerializeField] private string[] _deathMessages =
    {
        "ERROR: PROCESS TERMINATED",
        "ERROR: CONNECTION LOST",
        "ERROR: 404 HERO NOT FOUND"
    };

    private bool _shown;

    private void Start()
    {
        WirePlayerStats();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        WirePlayerStats();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnwirePlayerStats();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        WirePlayerStats();
    }

    private void ResolvePlayerStats()
    {
        if (PlayerController.Instance != null)
            _playerStats = PlayerController.Instance.GetComponent<PlayerStats>();
    }

    private void WirePlayerStats()
    {
        UnwirePlayerStats();
        ResolvePlayerStats();
        if (_playerStats == null) return;

        _playerStats.PlayerDied += OnPlayerDied;
        _playerStats.PlayerRevived += OnPlayerRevived;

        if (!_playerStats.IsDead)
            ResetDeathPresentation();
    }

    private void UnwirePlayerStats()
    {
        if (_playerStats == null) return;
        _playerStats.PlayerDied -= OnPlayerDied;
        _playerStats.PlayerRevived -= OnPlayerRevived;
    }

    private void OnPlayerRevived()
    {
        ResetDeathPresentation();
    }

    private void ResetDeathPresentation()
    {
        StopAllCoroutines();
        _shown = false;

        if (_statsPanel != null)
            _statsPanel.SetActive(true);

        if (_overlayGroup != null)
        {
            _overlayGroup.alpha = 0f;
            _overlayGroup.blocksRaycasts = false;
            _overlayGroup.interactable = false;
        }

        if (_restartButton != null)
            _restartButton.gameObject.SetActive(false);

        if (_messageText != null)
            _messageText.text = string.Empty;
    }

    private void OnPlayerDied()
    {
        if (_shown) return;
        _shown = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeOutMusic(_fadeDuration);

        if (_statsPanel != null)
            _statsPanel.SetActive(false);

        StartCoroutine(ShowDeathSequence());
    }

    private IEnumerator ShowDeathSequence()
    {
        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            _overlayGroup.alpha = Mathf.Clamp01(elapsed / _fadeDuration);
            yield return null;
        }

        _overlayGroup.alpha = 1f;
        _overlayGroup.blocksRaycasts = true;
        _overlayGroup.interactable = true;

        string message = _deathMessages[Random.Range(0, _deathMessages.Length)];
        _messageText.text = string.Empty;
        for (int i = 0; i < message.Length; i++)
        {
            _messageText.text += message[i];
            yield return new WaitForSeconds(_typewriterDelay);
        }

        _restartButton.gameObject.SetActive(true);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
