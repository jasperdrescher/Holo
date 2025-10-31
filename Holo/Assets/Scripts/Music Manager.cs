using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
	[SerializeField] private AudioClip BackgroundMusic1;
	[SerializeField] private AudioClip BackgroundMusic2;

	private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

		if (audioSource == null)
		{
			Debug.LogError("AudioSource was not found.");
		}

		audioSource.PlayOneShot(BackgroundMusic1);
    }
}
