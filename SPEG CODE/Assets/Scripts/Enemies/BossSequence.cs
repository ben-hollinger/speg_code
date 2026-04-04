using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;    

public class BossSequence : MonoBehaviour
{
    public static AudioClip PlayerDeathSfxWhileBossActive;

    [SerializeField] private Animator _animator;
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _dialogueText;

    [SerializeField] private float _dialogueStartRange = 5f;
    [SerializeField] private Key _advanceKey = Key.E;
    [SerializeField] private string[] _dialogueLines;

    [SerializeField] private GameObject _sword;
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private AudioClip _fightMusic;
    [SerializeField] private AudioClip _musicOnBossDefeat;
    [SerializeField] private float _stopMusicFadeDuration = 1f;
    [SerializeField] private AudioClip[] _lineSfx = new AudioClip[3];
    [SerializeField] private float _lineSfxVolume = 1f;
    [SerializeField] private AudioClip _playerDeathSfxAfterBossStarted;

    [SerializeField] private float _forwardMoveInitialDelay = 2f;
    [SerializeField] private float _forwardMoveDistance = 2f;
    [SerializeField] private float _forwardMoveDuration = 0.75f;

    private bool _started;
    private bool _talking;
    private bool _done;
    private int _line;
    private bool _playerFrozenForDialogue;

    private void Awake()
    {
        SetUi(false);
    }

    private void Update()
    {
        if (!_started && PlayerController.Instance != null &&
            Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <= _dialogueStartRange)
        {
            _started = true;
            OpenDialogue();
        }

        if (_talking && Keyboard.current != null && Keyboard.current[_advanceKey].wasPressedThisFrame)
            NextLine();
    }

    private void OpenDialogue()
    {
        PlayerController.Instance.SetMovementFrozen(true);
        _playerFrozenForDialogue = true;

        AudioManager.Instance.FadeOutMusic(_stopMusicFadeDuration);
        _talking = true;
        _done = false;
        _line = 0;
        SetUi(true);
        ShowLine(_line);
    }

    private void NextLine()
    {
        _line++;
        if (_dialogueLines == null || _line >= _dialogueLines.Length)
        {
            _talking = false;
            _done = true;
            SetUi(false);
            EnableCombat();
            return;
        }

        ShowLine(_line);
    }

    private void ShowLine(int i)
    {
        _dialogueText.text = _dialogueLines[Mathf.Clamp(i, 0, _dialogueLines.Length - 1)];
        AudioManager.Instance?.PlaySfx(_lineSfx[i], _lineSfxVolume);

        switch (i)
        {
            case 1:
                _animator?.SetTrigger("Stand");
                if (_forwardMoveDistance > 0f)
                {
                    if (_forwardMoveDuration > 0f) StartCoroutine(LerpForward());
                    else transform.position += transform.forward * _forwardMoveDistance;
                }
                break;
            case 2:
                _animator?.SetTrigger("StartFight");
                break;
        }
    }

    private IEnumerator LerpForward()
    {
        if (_forwardMoveInitialDelay > 0f)
            yield return new WaitForSeconds(_forwardMoveInitialDelay);

        Vector3 a = transform.position;
        Vector3 b = a + transform.forward * _forwardMoveDistance;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / _forwardMoveDuration;
            transform.position = Vector3.Lerp(a, b, Mathf.Clamp01(t));
            yield return null;
        }

        transform.position = b;
    }

    private void EnableCombat()
    {
        if (_playerFrozenForDialogue)
        {
            PlayerController.Instance?.SetMovementFrozen(false);
            _playerFrozenForDialogue = false;
        }

        _enemyController.enabled = true;
        PlayerDeathSfxWhileBossActive = _playerDeathSfxAfterBossStarted;
        AudioManager.Instance?.PlayMusic(_fightMusic);
        _healthBar.SetActive(true);
    }

    public void OnBossDefeated()
    {
        PlayerDeathSfxWhileBossActive = null;
        if (_musicOnBossDefeat != null)
            AudioManager.Instance?.PlayMusic(_musicOnBossDefeat);
    }

    public void StartFight()
    {
        if (_done) EnableCombat();
    }

    public void ShowSword()
    {
        _sword.SetActive(true);
    }

    private void SetUi(bool on)
    {
        _dialoguePanel.SetActive(on);
    }

    private void OnDisable()
    {
        PlayerDeathSfxWhileBossActive = null;
        if (!_playerFrozenForDialogue) return;
        PlayerController.Instance?.SetMovementFrozen(false);
        _playerFrozenForDialogue = false;
    }
}
