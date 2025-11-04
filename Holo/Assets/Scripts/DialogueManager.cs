using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
	[SerializeField] private GameObject dialoguePanel;

	[SerializeField] private float delayTime = 1.0f;

#if UNITY_EDITOR
	[SerializeField] private bool disableDialogue = false;
#endif

	private void Start()
	{
		if (dialoguePanel == null)
		{
			Debug.LogError("Failed to find dialogue panel");
		}

		dialoguePanel.SetActive(false);

#if UNITY_EDITOR
		if (!disableDialogue)
			StartCoroutine(CallFunctionAfterDelay());
#else
		StartCoroutine(CallFunctionAfterDelay());
#endif
	}

	public void HideDialogue()
	{
		dialoguePanel.SetActive(false);
	}

	IEnumerator CallFunctionAfterDelay()
    {
		yield return new WaitForSeconds(delayTime);

		dialoguePanel.SetActive(true);
    }
}
