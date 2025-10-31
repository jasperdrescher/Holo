using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
	[SerializeField] private GameObject dialoguePanel;

	[SerializeField] private float delayTime = 1.0f;

	private void Start()
	{
		if (dialoguePanel == null)
		{
			Debug.LogError("Failed to find dialogue panel");
		}

		dialoguePanel.SetActive(false);

		StartCoroutine(CallFunctionAfterDelay());
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
