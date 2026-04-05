using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public AudioClip musicClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlayMusic(musicClip);
        }
    }
}
