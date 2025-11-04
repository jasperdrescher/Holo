using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
	[SerializeField] private AudioClip BackgroundMusic1;
	[SerializeField] private AudioClip BackgroundMusic2;

#if UNITY_EDITOR
	[SerializeField] private bool disableMusic = false;
#endif

	private AudioSource audioSource;

	void Start()
	{
		audioSource = GetComponent<AudioSource>();

		if (audioSource == null)
		{
			Debug.LogError("AudioSource was not found.");
		}

#if UNITY_EDITOR
		if (!disableMusic)
			audioSource.PlayOneShot(BackgroundMusic1);
#else
		audioSource.PlayOneShot(BackgroundMusic1);
#endif
	}
}
