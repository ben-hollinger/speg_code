namespace NPC

{

    using UnityEngine;
    using UnityEngine.InputSystem;
    using TMPro;



    public class NPCController : MonoBehaviour

    {

        [Header("Interaction Settings")]
        public float interactionRange = 3f;
        // Interact key is fixed to E (new Input System)

        [Header("Dialogue")]
        public string[] dialogueLines;

        [Header("UI")]
        public GameObject interactPromptUI;
        public GameObject dialogueUI;
        public TextMeshProUGUI dialogueText;

        [Header("Prompt Position")]
        [Tooltip("Assign a Transform to pin the prompt above (e.g. an empty child " +
                 "called 'PromptAnchor' placed at head height). This must be a direct " +
                 "child of the ROOT NPC object — NOT parented to any mixamorig bone.")]

        public Transform promptAnchor;



        private bool _playerInRange = false;
        private bool _isTalking     = false;
        private int  _dialogueIndex = 0;



        private Animator       _animator;
        private PuzzlePieceNPC _puzzleNPC;
        private Transform      _promptTransform;



        void Start()
        {
            _animator  = GetComponentInChildren<Animator>();
            _puzzleNPC = GetComponent<PuzzlePieceNPC>();


            // Cache the prompt UI transform so we can reposition it each frame
            if (interactPromptUI != null)
            {
                _promptTransform = interactPromptUI.transform;
                interactPromptUI.SetActive(false);
            }


            if (dialogueUI != null)
                dialogueUI.SetActive(false);
        }



        void Update()
        {

            // Keep prompt anchored above NPC even if it lives outside bone hierarchy
            if (_playerInRange && promptAnchor != null && _promptTransform != null)
                _promptTransform.position = promptAnchor.position;

            if (_playerInRange && Keyboard.current[Key.E].wasPressedThisFrame)
            {
                if (!_isTalking)
                    StartDialogue();
                else
                    AdvanceDialogue();
            }
        }



       

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
                interactPromptUI.SetActive(true);
            }
        }



        void OnTriggerExit(Collider other)

        {

            if (!other.CompareTag("Player")) return;

            _playerInRange = false;

            ShowPrompt(false);

            EndDialogue();

        }



        void StartDialogue()

        {

            _isTalking     = true;

            _dialogueIndex = 0;

            ShowPrompt(false);



            if (dialogueUI != null) dialogueUI.SetActive(true);

            _animator?.SetBool("isTalking", true);

            ShowLine(_dialogueIndex);

        }



        void AdvanceDialogue()

        {

            _dialogueIndex++;

            if (_dialogueIndex < dialogueLines.Length)

            {

                ShowLine(_dialogueIndex);

            }

            else

            {

                _puzzleNPC?.OnDialogueEnded();

                EndDialogue();

            }

        }



        void ShowLine(int index)

        {

            if (dialogueText == null) return;



            if (dialogueLines == null || dialogueLines.Length == 0)

            {

                dialogueText.text = "...";

                return;

            }

            dialogueText.text = dialogueLines[Mathf.Clamp(index, 0, dialogueLines.Length - 1)];

        }



        void EndDialogue()

        {

            _isTalking = false;

            if (dialogueUI != null) dialogueUI.SetActive(false);

            _animator?.SetBool("isTalking", false);

            if (_playerInRange) ShowPrompt(true);

        }



        void ShowPrompt(bool show)

        {

            if (interactPromptUI == null) return;



            // If a world-space canvas, force it to face the camera

            if (show && Camera.main != null)

                interactPromptUI.transform.LookAt(

                    interactPromptUI.transform.position + Camera.main.transform.forward);



            interactPromptUI.SetActive(show);

        }

    }

}