namespace NPC
{
    using UnityEngine;
    using TMPro;

    public class NPCControllerVillage : MonoBehaviour
    {
        [Header("Interaction Settings")]
        public float interactionRange = 3f;
        public KeyCode interactKey = KeyCode.E;

        [Header("Dialogue")]
        public string[] dialogueLines;
        public AudioClip[] voiceLines;
        [Range(0f, 1f)]
        public float voiceLinesVolume = 1f;

        [Header("UI")]
        public GameObject interactPromptUI;
        public GameObject dialogueUI;
        public TextMeshProUGUI dialogueText;

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

        void StartDialogue()
        {
            isTalking = true;
            dialogueIndex = 0;
            dialogueUI.SetActive(true);
            interactPromptUI.SetActive(false);

            animator?.SetBool("isTalking", true);

            dialogueText.text = dialogueLines[dialogueIndex];
            TryPlayVoiceLine(dialogueIndex);
        }

        void AdvanceDialogue()
        {
            dialogueIndex++;

            if (dialogueIndex < dialogueLines.Length)
            {
                dialogueText.text = dialogueLines[dialogueIndex];
                TryPlayVoiceLine(dialogueIndex);
            }
            else
            {
                EndDialogue();
            }
        }

        void TryPlayVoiceLine(int index)
        {
            if (voiceLines == null || index < 0 || index >= voiceLines.Length) return;
            if (voiceLines[index] == null) return;
            AudioManager.Instance?.PlayDialogue(voiceLines[index], voiceLinesVolume);
        }

        void EndDialogue()
        {
            isTalking = false;
            dialogueUI.SetActive(false);
            AudioManager.Instance?.StopDialogue();

            animator?.SetBool("isTalking", false);

            if (playerInRange)
                interactPromptUI.SetActive(true);
        }
    }
}
