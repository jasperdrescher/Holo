using UnityEngine;

public class BattleManager : MonoBehaviour
{
	[SerializeField] private HorizontalCardHolder playerCardHolder;
	[SerializeField] private HorizontalCardHolder npcCardHolder;

	private void Start()
	{
		if (playerCardHolder == null || npcCardHolder == null)
			Debug.LogError("Failed to find HorizontalCardHolder");

		playerCardHolder.SelectedCardEvent.AddListener(OnSelected);
		npcCardHolder.SelectedCardEvent.AddListener(OnSelected);
	}

    void OnSelected()
	{
		if (playerCardHolder.selectedCard && npcCardHolder.selectedCard)
		{
			Debug.Log("Fight!");
		}
	}
}
