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

	private void OnSelected()
	{
		if (playerCardHolder.selectedCard && npcCardHolder.selectedCard)
		{
			Debug.Log("Fight!");
			Attack(playerCardHolder.selectedCard, npcCardHolder.selectedCard);
		}
	}
	
	private void Attack(Card playerCard, Card jokerCard)
	{
		jokerCard.hitpoints -= playerCard.strength;
		if (jokerCard.hitpoints <= 0)
		{
			npcCardHolder.OnCardDied(jokerCard);
		}
		else
		{
			jokerCard.cardVisual.UpdateVisual();
		}
	}
}
