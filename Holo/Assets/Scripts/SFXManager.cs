using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
	public static SFXManager instance;

	[SerializeField] private AudioClip cardSpreadClip;
	[SerializeField] private AudioClip cardHoverClip;
	[SerializeField] private AudioClip cardBeginDragClip;
	[SerializeField] private AudioClip cardEndDragClip;
	[SerializeField] private AudioClip cardAttackClip;
	[SerializeField] private AudioClip cardDiedClip;
	[SerializeField] private AudioClip victoryClip;
	[SerializeField] private AudioClip defeatClip;

	private AudioSource audioSource;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();

		if (audioSource == null)
		{
			Debug.LogError("AudioSource was not found.");
		}
	}

	private void PlaySFX(AudioClip clip)
	{
		if (audioSource != null)
		{
			audioSource.PlayOneShot(clip);
		}
	}

	public void PlayCardSpreadSFX()
	{
		PlaySFX(cardSpreadClip);
	}

	public void PlayCardHoverSFX()
	{
		PlaySFX(cardHoverClip);
	}

	public void PlayCardBeginDragSFX()
	{
		PlaySFX(cardBeginDragClip);
	}

	public void PlayCardEndDragSFX()
	{
		PlaySFX(cardEndDragClip);
	}

	public void PlayCardAttackSFX()
	{
		PlaySFX(cardAttackClip);
	}

	public void PlayCardDiedSFX()
	{
		PlaySFX(cardDiedClip);
	}

	public void PlayVictorySFX()
	{
		PlaySFX(victoryClip);
	}

	public void PlayDefeatSFX()
	{
		PlaySFX(defeatClip);
	}
}
