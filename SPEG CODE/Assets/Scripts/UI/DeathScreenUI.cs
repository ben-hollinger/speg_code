using System.Collections;
<<<<<<< HEAD
using UnityEngine;
using UnityEngine.SceneManagement;
=======
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
>>>>>>> origin/main
using UnityEngine.UI;

public class DeathScreenUI : MonoBehaviour
{
<<<<<<< HEAD
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private GameObject _statsPanel;
    [SerializeField] private CanvasGroup _overlayGroup;
    [SerializeField] private Text _messageText;
    [SerializeField] private Button _restartButton;
    [SerializeField] private float _fadeDuration = 0.6f;
    [SerializeField] private float _typewriterDelay = 0.03f;
=======
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
>>>>>>> origin/main
    [SerializeField] private string[] _deathMessages =
    {
        "ERROR: PROCESS TERMINATED",
        "ERROR: CONNECTION LOST",
        "ERROR: 404 HERO NOT FOUND"
    };

    private bool _shown;
<<<<<<< HEAD
=======
    private PlayerStats _wiredPlayerStats;

    private void Awake()
    {
        if (_restartButton != null)
            _restartButton.onClick.AddListener(RestartScene);

        ResetDeathPresentation();
    }
>>>>>>> origin/main

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
<<<<<<< HEAD
=======
        if (_playerStats != null)
            return;

>>>>>>> origin/main
        if (PlayerController.Instance != null)
            _playerStats = PlayerController.Instance.GetComponent<PlayerStats>();
    }

    private void WirePlayerStats()
    {
        UnwirePlayerStats();
        ResolvePlayerStats();
<<<<<<< HEAD
        _playerStats = PlayerController.Instance.gameObject.GetComponent<PlayerStats>();

        _playerStats.PlayerDied += OnPlayerDied;
        _playerStats.PlayerRevived += OnPlayerRevived;
=======
        if (_playerStats == null)
            return;

        _playerStats.PlayerDied += OnPlayerDied;
        _playerStats.PlayerRevived += OnPlayerRevived;
        _wiredPlayerStats = _playerStats;
>>>>>>> origin/main

        if (!_playerStats.IsDead)
            ResetDeathPresentation();
    }

    private void UnwirePlayerStats()
    {
<<<<<<< HEAD
        if (_playerStats == null) return;
        _playerStats.PlayerDied -= OnPlayerDied;
        _playerStats.PlayerRevived -= OnPlayerRevived;
=======
        var playerStats = _wiredPlayerStats;
        _wiredPlayerStats = null;
        if (playerStats == null) return;

        playerStats.PlayerDied -= OnPlayerDied;
        playerStats.PlayerRevived -= OnPlayerRevived;
>>>>>>> origin/main
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

<<<<<<< HEAD
        if (_messageText != null)
            _messageText.text = string.Empty;
=======
        SetGraphicText(_messageGraphic, string.Empty);
>>>>>>> origin/main
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
<<<<<<< HEAD
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
=======
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
>>>>>>> origin/main
    }

    public void RestartScene()
    {
<<<<<<< HEAD
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
=======
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
>>>>>>> origin/main
}
