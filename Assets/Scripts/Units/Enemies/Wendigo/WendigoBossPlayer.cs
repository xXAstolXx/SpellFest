using UnityEngine;

public class WendigoBossPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    MusicPlayer musicPlayer;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayArenaSound(AudioClip clip)
    {
        DisableMusicPlayer();
        audioSource.clip = clip;
        audioSource.Play();

    }
    private void DisableMusicPlayer()
    {
        musicPlayer.gameObject.SetActive(false);
    }
}
