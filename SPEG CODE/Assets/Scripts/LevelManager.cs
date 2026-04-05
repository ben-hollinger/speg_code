using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Complete UI")]
    [SerializeField] private GameObject _levelCompleteUI;

    [Header("Ability Unlock UI")]
    [SerializeField] private GameObject _abilityUnlockPanel;
    [SerializeField] private float _abilityPanelDuration = 3f;

    [Header("Hat")]
    [SerializeField] private GameObject _pjHat;
    [SerializeField] private GameObject _sombrero;
    [SerializeField] private GameObject _vikingHat;

    [Header("Enemies (in spawn order)")]
    [SerializeField] private GameObject[] _enemies;

    private int _currentEnemyIndex = 0;
    private bool _puzzleComplete;
    private bool _levelComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy != null)
                enemy.SetActive(false);
        }

        _currentEnemyIndex = 0;
        _puzzleComplete = false;
        _levelComplete = false;

        if (_levelCompleteUI != null)
            _levelCompleteUI.SetActive(false);

        if (_abilityUnlockPanel != null)
            _abilityUnlockPanel.SetActive(false);

        if (_pjHat != null)
            _pjHat.SetActive(false);

        Debug.Log($"[LevelManager] Level started. Total enemies: {_enemies.Length}");
    }

    public void OnPuzzleComplete()
    {
        if (_puzzleComplete) return;
        _puzzleComplete = true;
        
        Debug.Log($"[LevelManager] Hat swap - sombrero:{_sombrero != null}, viking:{_vikingHat != null}, pj:{_pjHat != null}");

        Debug.Log("[LevelManager] Puzzle complete - unlocking double jump and spawning first enemy.");

        // Swap hats
        if (_sombrero != null) _sombrero.SetActive(false);
        if (_vikingHat != null) _vikingHat.SetActive(false);
        if (_pjHat != null) _pjHat.SetActive(true);

        // Unlock double jump
        var doubleJump = PlayerController.Instance.GetComponent<DoubleJump>();
        if (doubleJump != null)
            doubleJump.UnlockDoubleJump();

        // Show ability unlock panel
        if (_abilityUnlockPanel != null)
            StartCoroutine(ShowAbilityUnlockPanel());

        // Spawn first enemy
        SpawnNextEnemy();
    }

    private IEnumerator ShowAbilityUnlockPanel()
    {
        _abilityUnlockPanel.SetActive(true);
        yield return new WaitForSeconds(_abilityPanelDuration);
        _abilityUnlockPanel.SetActive(false);
    }

    public void OnEnemyDefeated()
    {
        _currentEnemyIndex++;
        Debug.Log($"[LevelManager] Enemy defeated. Next index: {_currentEnemyIndex}/{_enemies.Length}");

        if (_currentEnemyIndex >= _enemies.Length)
            TriggerLevelComplete();
        else
            SpawnNextEnemy();
    }

    private void SpawnNextEnemy()
    {
        if (_currentEnemyIndex >= _enemies.Length) return;

        GameObject next = _enemies[_currentEnemyIndex];
        if (next != null)
        {
            next.SetActive(true);
            Debug.Log($"[LevelManager] Spawned enemy {_currentEnemyIndex + 1}/{_enemies.Length}: {next.name}");
        }
    }

    private void TriggerLevelComplete()
    {
        if (_levelComplete) return;
        _levelComplete = true;

        Debug.Log("[LevelManager] Level Complete!");

        if (_levelCompleteUI != null)
            _levelCompleteUI.SetActive(true);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}