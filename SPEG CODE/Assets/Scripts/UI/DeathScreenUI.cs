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

    private void Awake()
    {
        if (_playerStats == null && PlayerController.Instance != null)
        {
            _playerStats = PlayerController.Instance.GetComponent<PlayerStats>();
        }
    }

    private void OnEnable()
    {
        if (_playerStats != null)
        {
            _playerStats.PlayerDied += OnPlayerDied;
        }
    }

    private void OnDisable()
    {
        if (_playerStats != null)
        {
            _playerStats.PlayerDied -= OnPlayerDied;
        }
    }

    private void OnPlayerDied()
    {
        if (_shown) return;
        _shown = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeOutMusic(_fadeDuration);

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

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
