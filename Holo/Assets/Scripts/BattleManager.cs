using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public enum TurnAuthor
{
	AI,
	Player
}

public class BattleManager : MonoBehaviour
{
	[SerializeField] private HorizontalCardHolder playerCardHolder;
	[SerializeField] private HorizontalCardHolder npcCardHolder;
	[SerializeField] private TextMeshProUGUI manaText;
	[SerializeField] private TextMeshProUGUI turnText;
	[SerializeField] private Button attackButton;

	public int turn = 1;
	public int mana = 10;

	public TurnAuthor turnAuthor = TurnAuthor.Player;

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

		StartNextTurn();

		UpdateStats();
	}

	private void UpdateStats()
	{
		manaText.SetText("Mana " + mana.ToString());
		turnText.SetText("Turn " + turn.ToString());
	}

	private void StartNextTurn()
	{
		turn++;

		if (turnAuthor == TurnAuthor.Player)
		{
			turnAuthor = TurnAuthor.AI;

			StartCoroutine(AITurn());
		}
		else
		{
			turnAuthor = TurnAuthor.Player;
		}
	}

	private IEnumerator AITurn()
	{
		yield return new WaitForSeconds(1f);
		Card selectedNpcCard = npcCardHolder.cards[Random.Range(0, npcCardHolder.cards.Count)];
		selectedNpcCard.AISelect();
        SFXManager.instance.PlayCardHoverSFX();

		yield return new WaitForSeconds(1f);
		Card selectedPlayerCard = playerCardHolder.cards[Random.Range(0, playerCardHolder.cards.Count)];
		selectedPlayerCard.AISelect();
        SFXManager.instance.PlayCardHoverSFX();

		yield return new WaitForSeconds(1f);
		Attack(selectedPlayerCard, selectedNpcCard);
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
