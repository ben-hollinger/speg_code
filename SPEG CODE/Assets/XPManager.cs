using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton — tracks collected puzzle pieces and total XP.
/// Attach to a persistent GameObject (e.g. GameManager).
/// </summary>
public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }

    [Header("XP Settings")]
    [Tooltip("Total XP needed to fill the bar and unlock the chest.")]
    public int xpToFillBar = 80;   // 8 pieces × 10 XP each by default

    // ── State ─────────────────────────────────────────────────────────────────
    public int   CurrentXP        { get; private set; }
    public bool  BarFull          => CurrentXP >= xpToFillBar;
    public float XPFraction       => Mathf.Clamp01((float)CurrentXP / xpToFillBar);

    // Collected piece IDs so the same piece can't be collected twice
    private HashSet<int> _collectedPieceIDs = new HashSet<int>();
    public  IReadOnlyCollection<int> CollectedPieceIDs => _collectedPieceIDs;

    // ── Events ────────────────────────────────────────────────────────────────
    /// Fired whenever XP changes. Passes current XP and the 0-1 fraction.
    public static event Action<int, float> OnXPChanged;

    /// Fired once when the bar becomes full.
    public static event Action OnBarFull;

    // ── Unity lifecycle ───────────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Call this when the player collects a puzzle piece from an NPC.
    /// Returns false if the piece was already collected.
    /// </summary>
    public bool CollectPiece(PuzzlePieceData piece)
    {
        if (_collectedPieceIDs.Contains(piece.pieceID))
        {
            Debug.Log($"[XPManager] Piece '{piece.pieceName}' already collected.");
            return false;
        }

        _collectedPieceIDs.Add(piece.pieceID);
        AddXP(piece.xpReward);
        Debug.Log($"[XPManager] Collected '{piece.pieceName}'. XP: {CurrentXP}/{xpToFillBar}");
        return true;
    }

    public void AddXP(int amount)
    {
        bool wasFull = BarFull;
        CurrentXP = Mathf.Min(CurrentXP + amount, xpToFillBar);
        OnXPChanged?.Invoke(CurrentXP, XPFraction);
        if (!wasFull && BarFull)
            OnBarFull?.Invoke();
    }

    public bool HasPiece(int pieceID) => _collectedPieceIDs.Contains(pieceID);
}
