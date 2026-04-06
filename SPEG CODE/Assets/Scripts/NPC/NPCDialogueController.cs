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
        private Transform playerTransform;

        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            interactPromptUI.SetActive(false);
            dialogueUI.SetActive(false);

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
        }

        void Update()
        {
            if (playerTransform != null)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);
                bool nowInRange = distance <= interactionRange;

                if (nowInRange != playerInRange)
                {
                    playerInRange = nowInRange;

                    if (playerInRange)
                    {
                        if (!isTalking)
                            interactPromptUI.SetActive(true);
                    }
                    else
                    {
                        interactPromptUI.SetActive(false);
                        EndDialogue();
                    }
                }
            }

            if (playerInRange && Input.GetKeyDown(interactKey))
            {
                if (!isTalking)
                    StartDialogue();
                else
                    AdvanceDialogue();
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