using UnityEngine;
using TMPro;

public class RockTutorialTracker : MonoBehaviour
{
    [Header("Tutorial Rocks (assign exactly the 3 tutorial rocks)")]
    [SerializeField] private DestructibleRock[] _rocks;

    [Header("UI")]
    [SerializeField] private GameObject _tutorialUIPanel;
    [SerializeField] private TextMeshProUGUI _progressText;

    private int _destroyedCount;
    private int _totalCount;

    private void Start()
    {
        _totalCount = _rocks.Length;
        _destroyedCount = 0;

        // Register this tracker with each rock so they can report back.
        foreach (var rock in _rocks)
        {
            if (rock != null)
                rock.TutorialTracker = this;
        }

        UpdateUI();
    }

    // Called by DestructibleRock when it is destroyed.
    public void OnRockDestroyed()
    {
        _destroyedCount++;
        _destroyedCount = Mathf.Min(_destroyedCount, _totalCount);

        UpdateUI();

        if (_destroyedCount >= _totalCount)
            CompleteTutorial();
    }

    private void UpdateUI()
    {
        if (_progressText != null)
            _progressText.text = "Rocks Destroyed: " + _destroyedCount + "/" + _totalCount;
    }

    private void CompleteTutorial()
    {
        Debug.Log("[RockTutorial] All rocks destroyed. Tutorial complete!");

        // Hide the tutorial UI after a short delay so the player sees 3/3.
        Invoke(nameof(HideTutorialUI), 1.5f);
    }

    private void HideTutorialUI()
    {
        if (_tutorialUIPanel != null)
            _tutorialUIPanel.SetActive(false);
    }
}