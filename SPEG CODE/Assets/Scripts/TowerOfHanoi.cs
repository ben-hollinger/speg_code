using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TowerOfHanoi : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float _interactRadius = 3f;
    [SerializeField] private GameObject _puzzleUI;
    [SerializeField] private GameObject _interactPrompt;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI _pegDisplay;
    [SerializeField] private TextMeshProUGUI _instructionText;
    [SerializeField] private TextMeshProUGUI _moveCountText;
    [SerializeField] private TextMeshProUGUI _statusText;

    private const int NUM_DISCS = 3;

    private System.Collections.Generic.Stack<int>[] _pegs;

    private int _selectedPeg = -1;
    private int _moveCount = 0;
    private bool _isOpen = false;
    private bool _solved = false;
    private Transform _player;

    private void Start()
    {
        _player = PlayerController.Instance.transform;

        if (_puzzleUI != null) _puzzleUI.SetActive(false);
        if (_interactPrompt != null) _interactPrompt.SetActive(false);

        InitializePuzzle();
    }

    private void InitializePuzzle()
    {
        _pegs = new System.Collections.Generic.Stack<int>[3];
        for (int i = 0; i < 3; i++)
            _pegs[i] = new System.Collections.Generic.Stack<int>();

        for (int i = NUM_DISCS; i >= 1; i--)
            _pegs[0].Push(i);

        _selectedPeg = -1;
        _moveCount = 0;
        _solved = false;
    }

    private void Update()
    {
        if (_player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        bool inRange = dist <= _interactRadius;

        if (_interactPrompt != null)
            _interactPrompt.SetActive(inRange && !_isOpen && !_solved);

        if (!_solved && inRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!_isOpen) OpenPuzzle();
        }

        if (_isOpen)
        {
            HandlePuzzleInput();

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                ClosePuzzle();
        }
    }

    private void OpenPuzzle()
    {
        _isOpen = true;
        if (_puzzleUI != null) _puzzleUI.SetActive(true);
        RefreshDisplay();
    }

    private void ClosePuzzle()
    {
        _isOpen = false;
        _selectedPeg = -1;
        if (_puzzleUI != null) _puzzleUI.SetActive(false);
    }

    private void HandlePuzzleInput()
    {
        int pressed = -1;
        if (Keyboard.current.digit1Key.wasPressedThisFrame) pressed = 0;
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) pressed = 1;
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) pressed = 2;

        if (pressed == -1) return;

        if (_selectedPeg == -1)
        {
            if (_pegs[pressed].Count == 0)
            {
                SetStatus("That peg is empty!");
                return;
            }
            _selectedPeg = pressed;
            SetStatus($"Peg {pressed + 1} selected. Now choose destination peg.");
        }
        else
        {
            if (pressed == _selectedPeg)
            {
                _selectedPeg = -1;
                SetStatus("Deselected. Choose a source peg.");
                RefreshDisplay();
                return;
            }

            TryMove(_selectedPeg, pressed);
            _selectedPeg = -1;
        }

        RefreshDisplay();
    }

    private void TryMove(int from, int to)
    {
        int disc = _pegs[from].Peek();

        if (_pegs[to].Count > 0 && _pegs[to].Peek() < disc)
        {
            SetStatus("Invalid move! Can't place a larger disc on a smaller one.");
            return;
        }

        _pegs[from].Pop();
        _pegs[to].Push(disc);
        _moveCount++;

        SetStatus($"Moved disc from Peg {from + 1} to Peg {to + 1}.");
        CheckWin();
    }

    private void CheckWin()
    {
        if (_pegs[2].Count == NUM_DISCS)
        {
            _solved = true;
            SetStatus($"Puzzle solved in {_moveCount} moves! The seal is broken!");

            if (_moveCountText != null)
                _moveCountText.text = $"Moves: {_moveCount}";

            Invoke(nameof(CompletePuzzle), 2f);
        }
    }

    private void CompletePuzzle()
    {
        ClosePuzzle();
        LevelManager.Instance.OnPuzzleComplete();
    }

    private void RefreshDisplay()
    {
        if (_pegDisplay != null)
            _pegDisplay.text = GetPegDisplayString();

        if (_moveCountText != null)
            _moveCountText.text = $"Moves: {_moveCount}";

        if (_instructionText != null)
            _instructionText.text = "WELCOME TO THE TOWER OF HANOI PUZZLE!\n1, 2, 3 to select pegs, move all the discs to Peg 3 to win.\nPress ESC to close.";
    }

    private string GetPegDisplayString()
    {
        string result = "";
        for (int i = 0; i < 3; i++)
        {
            string selected = (_selectedPeg == i) ? " <--" : "";
            int[] discs = _pegs[i].ToArray();
            string discStr = discs.Length > 0 ? string.Join(",", discs) : "empty";
            result += $"Peg {i + 1}: [{discStr}]{selected}\n";
        }
        return result;
    }

    private void SetStatus(string msg)
    {
        if (_statusText != null)
            _statusText.text = msg;
        Debug.Log($"[TowerOfHanoi] {msg}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }
}