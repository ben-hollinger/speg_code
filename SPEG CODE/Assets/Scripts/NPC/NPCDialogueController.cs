namespace NPC
{
    using UnityEngine;
    using TMPro;

    public class NPCDialogueController : MonoBehaviour
    {
        [Header("Interaction Settings")]
        public float interactionRange = 3f;
        public KeyCode interactKey = KeyCode.E;

        [Header("Dialogue")]
        public string[] dialogueLines;

        [Header("UI")]
        public GameObject interactPromptUI;   // "Press E to talk" world-space UI
        public GameObject dialogueUI;         // Your dialogue panel
        public TextMeshProUGUI dialogueText;  // Text inside dialogue panel

        private bool playerInRange = false;
        private bool isTalking = false;
        private int dialogueIndex = 0;

        private Animator animator;

        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            interactPromptUI.SetActive(false);
            dialogueUI.SetActive(false);
        }

        void Update()
        {
            if (playerInRange && Input.GetKeyDown(interactKey))
            {
                if (!isTalking)
                    StartDialogue();
                else
                    AdvanceDialogue();
            }
        }

        // --- Trigger Detection ---
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                interactPromptUI.SetActive(true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                interactPromptUI.SetActive(false);
                EndDialogue();
            }
        }

        // --- Dialogue Logic ---
        void StartDialogue()
        {
            isTalking = true;
            dialogueIndex = 0;
            dialogueUI.SetActive(true);
            interactPromptUI.SetActive(false);

            // Optional: trigger NPC talk animation
            animator?.SetBool("isTalking", true);

            dialogueText.text = dialogueLines[dialogueIndex];
        }

        void AdvanceDialogue()
        {
            dialogueIndex++;

            if (dialogueIndex < dialogueLines.Length)
            {
                dialogueText.text = dialogueLines[dialogueIndex];
            }
            else
            {
                EndDialogue();
            }
        }

//<<<<<<< Updated upstream
//=======
/*//<<<<<<< HEAD:SPEG CODE/Assets/Scripts/NPC/NPCController.cs
        void TryPlayVoiceLine(int index)
        {
            if (voiceLines == null || index < 0 || index >= voiceLines.Length) return;
            if (voiceLines[index] == null) return;
            AudioManager.Instance?.PlayDialogue(voiceLines[index], voiceLinesVolume);
        }*/

//=======
//>>>>>>> origin/BenHollinger2:SPEG CODE/Assets/Scripts/NPC/NPCDialogueController.cs
//>>>>>>> Stashed changes
        void EndDialogue()
        {
            isTalking = false;
            dialogueUI.SetActive(false);
//<<<<<<< Updated upstream
//=======
            AudioManager.Instance?.StopDialogue();
//>>>>>>> Stashed changes

            animator?.SetBool("isTalking", false);

            // Show prompt again if player is still nearby
            if (playerInRange)
                interactPromptUI.SetActive(true);
        }
    }

}