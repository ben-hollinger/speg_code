using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TreasureChest : MonoBehaviour
{
    [Header("Interaction")]
    [Tooltip("Key the player presses to open the chest.")]
    public Key openKey = Key.C;

    [Header("Prompt UI")]
    [Tooltip("World-space Canvas/panel that shows the prompt above the chest.")]
    public GameObject promptUI;

    [Tooltip("Text inside the prompt. Updated automatically based on XP state.")]
    public TextMeshProUGUI promptText;

    [Header("Puzzle UI")]
    [Tooltip("The 2D puzzle panel to open when the chest is unlocked.")]
    public GameObject puzzleUI;

    [Header("Messages")]
    [Tooltip("Shown when XP bar is not full yet.")]
    public string lockedMessage   = "Chest is sealed.\nCollect all puzzle pieces first.";

    [Tooltip("Shown when XP bar is full and chest can be opened.")]
    public string unlockedMessage = "Press C to open the chest!";

    private bool _playerInRange = false;
    private bool _opened        = false;

    void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
        if (puzzleUI != null) puzzleUI.SetActive(false);
    }

    void Update()
    {
        if (!_playerInRange || _opened) return;

        RefreshPrompt();

        if (Keyboard.current[openKey].wasPressedThisFrame
            && XPManager.Instance != null
            && XPManager.Instance.BarFull)
        {
            OpenChest();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInRange = true;
        if (promptUI != null) promptUI.SetActive(true);
        RefreshPrompt();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInRange = false;
        if (promptUI != null) promptUI.SetActive(false);
    }

    void RefreshPrompt()
    {
        if (promptText == null) return;
        bool full = XPManager.Instance != null && XPManager.Instance.BarFull;
        promptText.text = full ? unlockedMessage : lockedMessage;
    }

    void OpenChest()
    {
        _opened = true;
        if (promptUI != null) promptUI.SetActive(false);

        if (puzzleUI != null)
        {
            puzzleUI.SetActive(true);
            Time.timeScale = 0f;
        }

        Debug.Log("[TreasureChest] Opened — puzzle UI displayed.");
    }
}
