using UnityEngine;

public class TutorialAreaTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _tutorialUIPanel;

    private void Start()
    {
        _tutorialUIPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _tutorialUIPanel.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            _tutorialUIPanel.SetActive(false);
    }
}