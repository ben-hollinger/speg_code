using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StatsUI : MonoBehaviour
{
    private const char   BLOCK_FULL  = '\u2588';
    private const char   BLOCK_EMPTY = '\u2591';
    private const string HEART = "\u2665";

    [Header("UI Text References")]
    [Tooltip("The Text component showing the XP bar line.")]
    public TMP_Text xpLineText;

    [Tooltip("The Text component showing the hearts line.")]
    public TMP_Text threadLineText;

    [Header("Player Reference")]
    [Tooltip("Drag TechGuy here.")]
    public PlayerStats playerStats;

    [Header("XP Bar Appearance")]
    [Tooltip("Number of block characters in the bar.")]
    public int barSegmentCount = 20;
    public string xpFilledColor = "#4FA8FF";
    public string xpEmptyColor  = "#1A1F2E";

    [Header("Hearts Appearance")]
    public string heartColor = "#ff0000";

    private readonly StringBuilder _sb = new StringBuilder(256);

    // Dirty-check cache
    private int   _lastXP     = -1;
    private int   _lastMax    = -1;
    private int   _lastHealth = -1;

    void Awake()
    {
        // TMP supports rich text by default
        

        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();
    }

    void OnEnable()
    {
        if (playerStats != null)
            playerStats.HealthChanged += OnHealthChanged;

        XPManager.OnXPChanged += OnXPChanged;
    }

    void OnDisable()
    {
        if (playerStats != null)
            playerStats.HealthChanged -= OnHealthChanged;

        XPManager.OnXPChanged -= OnXPChanged;
    }

    void Start()
    {
        RefreshXP(force: true);
        RefreshHearts(force: true);
    }

    // Called by XPManager event when a puzzle piece is collected
    void OnXPChanged(int xp, float fraction) => RefreshXP(force: true);

    // Called by PlayerStats event when health changes
    void OnHealthChanged(int current, int max)
    {
        _lastHealth = -1;   // invalidate cache
        RefreshHearts(force: true);
    }

    // Poll every frame so the bar stays smooth if XPBar animates the slider
    void Update() => RefreshXP(force: false);


    void RefreshXP(bool force)
    {
        if (xpLineText == null || XPManager.Instance == null) return;

        int xp  = XPManager.Instance.CurrentXP;
        int max = XPManager.Instance.xpToFillBar;

        if (!force && xp == _lastXP && max == _lastMax) return;
        _lastXP  = xp;
        _lastMax = max;

        float fill     = max > 0 ? Mathf.Clamp01((float)xp / max) : 0f;
        int segments   = Mathf.Max(1, barSegmentCount);
        int filled     = Mathf.Clamp(Mathf.RoundToInt(fill * segments), 0, segments);

        _sb.Clear();
        _sb.Append("LVL 1 [");

        if (filled > 0)
            _sb.Append("<color=").Append(xpFilledColor).Append('>')
               .Append(BLOCK_FULL, filled)
               .Append("</color>");

        if (filled < segments)
            _sb.Append("<color=").Append(xpEmptyColor).Append('>')
               .Append(BLOCK_EMPTY, segments - filled)
               .Append("</color>");

        _sb.Append("] ").Append(xp).Append('/').Append(max).Append(" XP");
        xpLineText.text = _sb.ToString();
    }

   
    void RefreshHearts(bool force)
    {
    if (threadLineText == null || playerStats == null) return;

    int current = playerStats.CurrentHealth;
    if (!force && current == _lastHealth) return;
    _lastHealth = current;

    _sb.Clear();
    _sb.Append("INTEGRITY: ");
    for (int i = 0; i < current; i++)
        _sb.Append("<size=150%><color=").Append(heartColor).Append('>')
           .Append(HEART).Append("</color></size> ");

    threadLineText.text = _sb.ToString();
    }
}
