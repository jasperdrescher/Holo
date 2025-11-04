using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{
	[SerializeField] private HorizontalCardHolder playerCardHolder;
	[SerializeField] private HorizontalCardHolder npcCardHolder;
	[SerializeField] private TextMeshProUGUI manaText;
	[SerializeField] private TextMeshProUGUI roundText;

	public int round = 1;
	public int mana = 10;

	private void Start()
	{
		if (playerCardHolder == null || npcCardHolder == null)
			Debug.LogError("Failed to find HorizontalCardHolder");

		playerCardHolder.SelectedCardEvent.AddListener(OnSelected);
		npcCardHolder.SelectedCardEvent.AddListener(OnSelected);

		UpdateStats();
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
		if (playerCard.cost > mana)
		{
			return;
		}

		SFXManager.instance.PlayCardAttackSFX();

		jokerCard.hitpoints -= playerCard.strength;
		if (jokerCard.hitpoints <= 0)
		{
			npcCardHolder.OnCardDied(jokerCard);

			SFXManager.instance.PlayCardDiedSFX();
		}
		else
		{
			jokerCard.cardVisual.UpdateVisual();
		}

		playerCardHolder.DeselectAll();
		npcCardHolder.DeselectAll();

		mana -= playerCard.cost;
		round++;

		UpdateStats();
	}
	
	private void UpdateStats()
	{
		manaText.SetText("Mana " + mana.ToString());
		roundText.SetText("Round " + round.ToString());
	}
}
