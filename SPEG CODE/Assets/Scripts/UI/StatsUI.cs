using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    private const char BlockFull = '\u2588';
    private const char BlockEmpty = '\u2591';
    private const string HeartGlyph = "\u2764";

    [Header("References")]
    [SerializeField] private Text _xpLineText;
    [SerializeField] private Text _threadLineText;
    [SerializeField] private XPBar _xpBar;
    [SerializeField] private PlayerStats _playerStats;

    [Header("XP bar")]
    [SerializeField] private int _barSegmentCount = 40;
    [SerializeField] private string _xpFilledColor = "#4FA8FF";
    [SerializeField] private string _xpEmptyColor = "#1A1F2E";

    [Header("Life (hearts)")]
    [SerializeField] private int _healthPerHeart = 1;
    [SerializeField] private string _fullHeartColor = "#e858d8";

    private readonly StringBuilder _sb = new StringBuilder(256);
    private int _lastLevel = int.MinValue;
    private float _lastSlider;
    private int _lastXpFloor = int.MinValue;
    private int _lastXpToNextFloor = int.MinValue;
    private PlayerStats _wiredHealthSource;

    private void Awake()
    {
        if (_xpBar == null)
            _xpBar = XPBar.Instance;

        if (_xpLineText != null)
            _xpLineText.supportRichText = true;
        if (_threadLineText != null)
            _threadLineText.supportRichText = true;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        WirePlayerStatsSubscription();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnwirePlayerStatsSubscription();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        WirePlayerStatsSubscription();
    }

    private void ResolvePlayerStats()
    {
        if (PlayerController.Instance != null)
            _playerStats = PlayerController.Instance.GetComponent<PlayerStats>();
    }

    private void WirePlayerStatsSubscription()
    {
        UnwirePlayerStatsSubscription();
        ResolvePlayerStats();
        if (_playerStats == null) return;

        _playerStats.HealthChanged += OnHealthChanged;
        _wiredHealthSource = _playerStats;
        RefreshThreadLine();
        RefreshXpLine(force: true);
    }

    private void UnwirePlayerStatsSubscription()
    {
        var s = _wiredHealthSource;
        _wiredHealthSource = null;
        if (s != null)
            s.HealthChanged -= OnHealthChanged;
    }

    private void Update()
    {
        RefreshXpLine(force: false);
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        RefreshThreadLine();
    }

    private void RefreshXpLine(bool force)
    {
        if (_xpLineText == null || _xpBar == null)
            return;

        float fill;
        if (_xpBar.xpSlider != null)
            fill = Mathf.Clamp01(_xpBar.xpSlider.value);
        else if (_xpBar.xpToNextLevel > 0.01f)
            fill = Mathf.Clamp01(_xpBar.currentXP / _xpBar.xpToNextLevel);
        else
            fill = 0f;

        int xpFloor = Mathf.FloorToInt(_xpBar.currentXP);
        int toNextFloor = Mathf.FloorToInt(_xpBar.xpToNextLevel);

        if (!force &&
            _xpBar.currentLevel == _lastLevel &&
            Mathf.Approximately(fill, _lastSlider) &&
            xpFloor == _lastXpFloor &&
            toNextFloor == _lastXpToNextFloor)
        {
            return;
        }

        _lastLevel = _xpBar.currentLevel;
        _lastSlider = fill;
        _lastXpFloor = xpFloor;
        _lastXpToNextFloor = toNextFloor;

        int segments = _barSegmentCount > 0 ? _barSegmentCount : 40;
        int filled = Mathf.Clamp(Mathf.RoundToInt(fill * segments), 0, segments);

        _sb.Clear();
        _sb.Append("LVL ");
        _sb.Append(_xpBar.currentLevel);
        _sb.Append(" ");
        _sb.Append('[');
        if (filled > 0)
        {
            _sb.Append("<color=");
            _sb.Append(_xpFilledColor);
            _sb.Append('>');
            for (int i = 0; i < filled; i++)
                _sb.Append(BlockFull);
            _sb.Append("</color>");
        }

        if (filled < segments)
        {
            _sb.Append("<color=");
            _sb.Append(_xpEmptyColor);
            _sb.Append('>');
            for (int i = filled; i < segments; i++)
                _sb.Append(BlockEmpty);
            _sb.Append("</color>");
        }

        _sb.Append("] ");
        _sb.Append(xpFloor);
        _sb.Append('/');
        _sb.Append(toNextFloor);
        _sb.Append(" XP");
        _xpLineText.text = _sb.ToString();
    }

    private void RefreshThreadLine()
    {
        if (_threadLineText == null || _playerStats == null)
            return;

        int hph = _healthPerHeart > 0 ? _healthPerHeart : 1;
        int currentHealth = Mathf.Clamp(_playerStats.CurrentHealth, 0, _playerStats.MaxHealth);
        int heartCount = (currentHealth + hph - 1) / hph;

        _sb.Clear();
        _sb.Append("INTEGRITY: ");
        for (int i = 0; i < heartCount; i++)
        {
            _sb.Append("<color=");
            _sb.Append(_fullHeartColor);
            _sb.Append('>');
            _sb.Append(HeartGlyph);
            _sb.Append("</color> ");
        }

        _threadLineText.text = _sb.ToString();
    }
}
