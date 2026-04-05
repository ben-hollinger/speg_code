namespace NPC
{
    using UnityEngine;
    using TMPro;

    /// <summary>
    /// Add this alongside NPCController on each villager NPC.
    /// Assign a PuzzlePieceData asset to give that NPC their unique piece.
    ///
    /// DIALOGUE FLOW PER NPC
    /// ─────────────────────────────────────────────────────────────────────────
    /// 1. Player walks up → "Press E to talk" prompt appears.
    /// 2. Player presses E → NPC speaks their dialogue lines (set in NPCController).
    ///    The last line should be the clue from the PuzzlePieceData.
    /// 3. At the end of dialogue → piece is collected, XP awarded, confirmation shown.
    /// 4. On subsequent visits → NPC says their "already gave" lines only.
    /// </summary>
    public class PuzzlePieceNPC : MonoBehaviour
    {
        [Header("Puzzle Piece")]
        [Tooltip("Drag the PuzzlePieceData ScriptableObject asset for this NPC here.")]
        public PuzzlePieceData puzzlePiece;

        [Header("Post-Collection Dialogue")]
        [Tooltip("What this NPC says if the player talks to them again after collecting the piece.")]
        [TextArea(2, 3)]
        public string alreadyCollectedLine = "You already have what I gave you. Good luck with the puzzle!";

        [Header("Collection Feedback UI")]
        [Tooltip("Optional: a world-space popup (e.g. '+10 XP') that appears on collection.")]
        public GameObject collectionFeedbackUI;

        [Tooltip("Text inside the feedback popup.")]
        public TextMeshProUGUI feedbackText;

        [Tooltip("How long (seconds) the feedback popup is visible.")]
        public float feedbackDuration = 2f;

        // ── Internal refs ─────────────────────────────────────────────────────
        private NPCController _npc;

        void Awake()
        {
            _npc = GetComponent<NPCController>();
            if (_npc == null)
                Debug.LogError("[PuzzlePieceNPC] NPCController not found on this GameObject!");

            if (collectionFeedbackUI != null)
                collectionFeedbackUI.SetActive(false);
        }

        void Start()
        {
            if (puzzlePiece == null)
            {
                Debug.LogWarning($"[PuzzlePieceNPC] {gameObject.name} has no PuzzlePieceData assigned!");
                return;
            }

            // Inject the clue as the final dialogue line so designers only need to
            // write the NPC's intro lines in NPCController.dialogueLines.
            InjectClueIntoDialogue();
        }

        /// <summary>
        /// Called by NPCController when dialogue ends.
        /// Hooks into the NPC controller via Unity's event system or direct call.
        /// </summary>
        public void OnDialogueEnded()
        {
            if (puzzlePiece == null) return;

            // Already collected — swap to the "already gave" dialogue
            if (XPManager.Instance != null && XPManager.Instance.HasPiece(puzzlePiece.pieceID))
            {
                SetAlreadyCollectedDialogue();
                return;
            }

            // Collect the piece
            bool collected = XPManager.Instance != null && XPManager.Instance.CollectPiece(puzzlePiece);
            if (collected)
            {
                ShowFeedback($"+ {puzzlePiece.xpReward} XP\nReceived: {puzzlePiece.pieceName}");
                SetAlreadyCollectedDialogue();
            }
        }

        // ── Private helpers ───────────────────────────────────────────────────

        void InjectClueIntoDialogue()
        {
            if (_npc == null || puzzlePiece == null) return;

            // Build new dialogue array: existing lines + clue line at the end
            string clue = $"\"{puzzlePiece.clueText}\"\n[Received: {puzzlePiece.pieceName}]";
            string[] existing = _npc.dialogueLines ?? new string[0];
            string[] combined = new string[existing.Length + 1];
            existing.CopyTo(combined, 0);
            combined[combined.Length - 1] = clue;
            _npc.dialogueLines = combined;
        }

        void SetAlreadyCollectedDialogue()
        {
            if (_npc == null) return;
            _npc.dialogueLines = new string[] { alreadyCollectedLine };
        }

        void ShowFeedback(string message)
        {
            if (collectionFeedbackUI == null) return;
            if (feedbackText != null) feedbackText.text = message;
            collectionFeedbackUI.SetActive(true);
            Invoke(nameof(HideFeedback), feedbackDuration);
        }

        void HideFeedback()
        {
            if (collectionFeedbackUI != null)
                collectionFeedbackUI.SetActive(false);
        }
    }
}
