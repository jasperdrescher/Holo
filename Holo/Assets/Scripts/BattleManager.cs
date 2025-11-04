using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
	[SerializeField] private HorizontalCardHolder playerCardHolder;
	[SerializeField] private HorizontalCardHolder npcCardHolder;
	[SerializeField] private TextMeshProUGUI manaText;
	[SerializeField] private TextMeshProUGUI roundText;
	[SerializeField] private Button attackButton;

	public int round = 1;
	public int mana = 10;

	private void Start()
	{
		if (playerCardHolder == null || npcCardHolder == null)
			Debug.LogError("Failed to find HorizontalCardHolder");

		playerCardHolder.SelectedCardEvent.AddListener(OnSelected);
		npcCardHolder.SelectedCardEvent.AddListener(OnSelected);

		UpdateStats();

		DisableAttackButton();
	}

	public void AttackEvent()
	{
		Attack(playerCardHolder.selectedCard, npcCardHolder.selectedCard);
	}

	private void OnSelected()
	{
		if (playerCardHolder.selectedCard && npcCardHolder.selectedCard)
		{
			if (playerCardHolder.selectedCard.cost <= mana)
			{
				EnableAttackButton();
			}
			else
			{
				DisableAttackButton();
			}
		}
		else
		{
			DisableAttackButton();
		}
	}

	private void Attack(Card playerCard, Card jokerCard)
	{
		DisableAttackButton();

		SFXManager.instance.PlayCardAttackSFX();

		mana -= playerCard.cost;

		jokerCard.hitpoints -= playerCard.strength;
		if (jokerCard.hitpoints <= 0)
		{
			npcCardHolder.OnCardDied(jokerCard);

			mana += 2;

			SFXManager.instance.PlayCardDiedSFX();
		}
		else
		{
			jokerCard.cardVisual.UpdateVisual();
		}

		playerCardHolder.DeselectAll();
		npcCardHolder.DeselectAll();

		round++;

		UpdateStats();
	}

	private void UpdateStats()
	{
		manaText.SetText("Mana " + mana.ToString());
		roundText.SetText("Round " + round.ToString());
	}

	private void EnableAttackButton()
	{
		attackButton.interactable = true;
	}

	private void DisableAttackButton()
	{
		attackButton.interactable = false;
	}
}
