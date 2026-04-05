using UnityEngine;

/// <summary>
/// ScriptableObject — create one asset per puzzle piece.
/// Right-click in Project → Create → Puzzle → Puzzle Piece Data
/// </summary>
[CreateAssetMenu(fileName = "PuzzlePiece", menuName = "Puzzle/Puzzle Piece Data")]
public class PuzzlePieceData : ScriptableObject
{
    [Header("Piece Identity")]
    [Tooltip("Unique ID, e.g. 0-8 for a 3x3 grid.")]
    public int pieceID;

    [Tooltip("Human-readable name shown in inventory/puzzle UI.")]
    public string pieceName;

    [Tooltip("The sprite shown in the puzzle grid slot.")]
    public Sprite pieceSprite;

    [Header("Clue")]
    [Tooltip("What the NPC says as a hint about where this goes in the puzzle.\n" +
             "Examples:\n" +
             "  'This tile belongs in the top-left corner.'\n" +
             "  'I believe this is the rightmost piece of the middle row.'\n" +
             "  'This one sits dead centre in the grid.'")]
    [TextArea(2, 4)]
    public string clueText;

    [Header("XP Reward")]
    [Tooltip("XP awarded to the player when they collect this piece.")]
    public int xpReward = 10;
}
