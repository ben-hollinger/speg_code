using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DeathScreenUI : MonoBehaviour
{
    [FormerlySerializedAs("playerStats")]
    [SerializeField] private PlayerStats _playerStats;
    [FormerlySerializedAs("statsPanel")]
    [SerializeField] private GameObject _statsPanel;
    [FormerlySerializedAs("overlayGroup")]
    [SerializeField] private CanvasGroup _overlayGroup;
    [FormerlySerializedAs("_messageText")]
    [FormerlySerializedAs("messageText")]
    [SerializeField] private Graphic _messageGraphic;
    [FormerlySerializedAs("restartButton")]
    [SerializeField] private Button _restartButton;
    [FormerlySerializedAs("fadeDuration")]
    [SerializeField] private float _fadeDuration = 0.6f;
    [FormerlySerializedAs("typewriterDelay")]
    [SerializeField] private float _typewriterDelay = 0.03f;
    [FormerlySerializedAs("deathMessages")]
    [SerializeField] private string[] _deathMessages =
    {
        "ERROR: PROCESS TERMINATED",
        "ERROR: CONNECTION LOST",
        "ERROR: 404 HERO NOT FOUND"
    };

    private bool _shown;
    private PlayerStats _wiredPlayerStats;

    private void Awake()
    {
        if (_restartButton != null)
            _restartButton.onClick.AddListener(RestartScene);

        ResetDeathPresentation();
    }

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
        if (_playerStats != null)
            return;

        if (PlayerController.Instance != null)
            _playerStats = PlayerController.Instance.GetComponent<PlayerStats>();
    }

    private void WirePlayerStats()
    {
        UnwirePlayerStats();
        ResolvePlayerStats();
        if (_playerStats == null)
            return;

        _playerStats.PlayerDied += OnPlayerDied;
        _playerStats.PlayerRevived += OnPlayerRevived;
        _wiredPlayerStats = _playerStats;

        if (!_playerStats.IsDead)
            ResetDeathPresentation();
    }

    private void UnwirePlayerStats()
    {
        var playerStats = _wiredPlayerStats;
        _wiredPlayerStats = null;
        if (playerStats == null) return;

        playerStats.PlayerDied -= OnPlayerDied;
        playerStats.PlayerRevived -= OnPlayerRevived;
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

        SetGraphicText(_messageGraphic, string.Empty);
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
            if (_overlayGroup != null)
                _overlayGroup.alpha = Mathf.Clamp01(elapsed / _fadeDuration);
            yield return null;
        }

        if (_overlayGroup != null)
        {
            _overlayGroup.alpha = 1f;
            _overlayGroup.blocksRaycasts = true;
            _overlayGroup.interactable = true;
        }

        string message = _deathMessages != null && _deathMessages.Length > 0
            ? _deathMessages[Random.Range(0, _deathMessages.Length)]
            : "ERROR: PROCESS TERMINATED";
        SetGraphicText(_messageGraphic, string.Empty);
        for (int i = 0; i < message.Length; i++)
        {
            AppendGraphicText(_messageGraphic, message[i]);
            yield return new WaitForSeconds(_typewriterDelay);
        }

        if (_restartButton != null)
            _restartButton.gameObject.SetActive(true);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private static void SetGraphicText(Graphic textGraphic, string value)
    {
        switch (textGraphic)
        {
            case Text legacyText:
                legacyText.text = value;
                break;
            case TMP_Text tmpText:
                tmpText.text = value;
                break;
        }
    }

    private static void AppendGraphicText(Graphic textGraphic, char value)
    {
        switch (textGraphic)
        {
            case Text legacyText:
                legacyText.text += value;
                break;
            case TMP_Text tmpText:
                tmpText.text += value;
                break;
        }
    }
}
