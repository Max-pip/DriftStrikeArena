using UnityEngine;

public class PlayerAudioSource : MonoBehaviour
{
    [SerializeField] private AudioSource _playerAudioSource;
    private const string AccelerationString = "acceleration";
    private const string BreakingString = "breaking";

    private void OnEnable()
    {
        CarController.onTouchedAccelerationPlatform += NeesPlayerClip;
    }

    private void OnDisable()
    {
        CarController.onTouchedAccelerationPlatform -= NeesPlayerClip;
    }

    public void NeesPlayerClip(string nameClip)
    {
        if (nameClip == AccelerationString)
        {
            if (AudioManager.Instance != null)
            {
                _playerAudioSource.PlayOneShot(AudioManager.Instance.accelerationClip);
            }
        }

        if (nameClip == BreakingString)
        {
            if (AudioManager.Instance != null)
            {
                _playerAudioSource.PlayOneShot(AudioManager.Instance.breakingClip);
            }
        }
    }
}
