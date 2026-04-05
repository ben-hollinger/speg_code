using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    [Header("Key Settings")]
    [Tooltip("How many keys are needed before the player can move to the next level.")]
    [SerializeField] private int _keyThreshold = 1;

    [Header("Next Scene")]
    [Tooltip("Exact name of the scene to load when the threshold is met and K is pressed.")]
    [SerializeField] private string _nextSceneName;

    [Header("UI")]
    [Tooltip("The panel to show when the player has enough keys.")]
    [SerializeField] private GameObject _keyUI;

    [Tooltip("Text inside the panel. Updated automatically.")]
    [SerializeField] private TextMeshProUGUI _keyText;

    [Header("Messages")]
    [SerializeField] private string _successMessage = "Well done! You have your key.\nPress K to enter the next level.";

    private int _keyCount;
    private bool _thresholdReached;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (_keyUI != null) _keyUI.SetActive(false);
    }

    private void Update()
    {
        if (!_thresholdReached) return;

        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
            LoadNextScene();
    }

    public void AddKey()
    {
        _keyCount++;
        Debug.Log("[KeyManager] Key added. Total: " + _keyCount + "/" + _keyThreshold);

        if (_keyCount >= _keyThreshold)
            OnThresholdReached();
    }

    private void OnThresholdReached()
    {
        _thresholdReached = true;
        Debug.Log("[KeyManager] Key threshold reached. Player can advance.");

        if (_keyUI != null)
        {
            _keyUI.SetActive(true);
            if (_keyText != null)
                _keyText.text = _successMessage;
        }
    }

    private void LoadNextScene()
    {
        if (string.IsNullOrEmpty(_nextSceneName))
        {
            Debug.LogWarning("[KeyManager] No next scene name set.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(_nextSceneName);
    }
}