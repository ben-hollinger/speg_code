using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public void PlaySfx(AudioClip audioClip)
    {
        AudioManager.Instance.PlaySfx(audioClip);
    }

    public void PlaySfxLoud(AudioClip audioClip) {
        AudioManager.Instance.PlaySfx(audioClip, 2.0f);
    }
}
