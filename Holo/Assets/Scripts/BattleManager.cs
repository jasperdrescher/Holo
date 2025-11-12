using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
	[SerializeField] private TextMeshProUGUI anyoneWonText;
	[SerializeField] private GameObject anyoneWonPanel;
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

		anyoneWonPanel.SetActive(false);

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

	private void Attack(Card playerCard, Card npcCard)
	{
		DisableAttackButton();

		SFXManager.instance.PlayCardAttackSFX();

		bool hasAnyoneWon = false;
		if (turnAuthor == TurnAuthor.Player)
		{
			mana -= playerCard.cost;

			npcCard.hitpoints -= playerCard.strength;
			if (npcCard.hitpoints <= 0)
			{
				npcCardHolder.OnCardDied(npcCard);

				switch (npcCard.cardVisual.GetEdition())
				{
					case Edition.Regular:
						mana += 2;
						break;
					case Edition.Polychrome:
						mana += 4;
						break;
					case Edition.Foil:
						mana += 3;
						break;
					case Edition.Negative:
						mana += 5;
						break;
				}

				SFXManager.instance.PlayCardDiedSFX();

				if (npcCardHolder.cards.Count == 0)
				{
					SFXManager.instance.PlayVictorySFX();
					MusicManager.instance.StopMusic();
					anyoneWonText.text = "You won!";
					anyoneWonPanel.SetActive(true);
					hasAnyoneWon = true;
				}
			}
			else
			{
				npcCard.cardVisual.UpdateVisual();
			}
		}
		else
		{
			playerCard.hitpoints -= npcCard.strength;
			if (playerCard.hitpoints <= 0)
			{
				playerCardHolder.OnCardDied(playerCard);

				SFXManager.instance.PlayCardDiedSFX();

				if (playerCardHolder.cards.Count == 0)
				{
					SFXManager.instance.PlayDefeatSFX();
					MusicManager.instance.StopMusic();
					anyoneWonText.text = "You lost!";
					anyoneWonPanel.SetActive(true);
					hasAnyoneWon = true;
				}
			}
			else
			{
				playerCard.cardVisual.UpdateVisual();
			}
		}

		playerCardHolder.DeselectAll();
		npcCardHolder.DeselectAll();

		if (!hasAnyoneWon)
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

	public void QuitEvent()
	{
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
	}

	private IEnumerator AITurn()
	{
		yield return new WaitForSeconds(1f);

		int prevHighestStrength = 0;
		int bestCardIndex = 0;
		for (int i = 0; i < npcCardHolder.cards.Count; i++)
		{
			if (npcCardHolder.cards[i].strength > prevHighestStrength)
			{
				prevHighestStrength = npcCardHolder.cards[i].strength;
				bestCardIndex = i;
			}
		}

		Card selectedNpcCard = npcCardHolder.cards[bestCardIndex];
		selectedNpcCard.AISelect();
        SFXManager.instance.PlayCardHoverSFX();

		yield return new WaitForSeconds(1f);

		prevHighestStrength = 0;
		bestCardIndex = 0;
		for (int i = 0; i < playerCardHolder.cards.Count; i++)
		{
			if (playerCardHolder.cards[i].strength > prevHighestStrength)
			{
				prevHighestStrength = playerCardHolder.cards[i].strength;
				bestCardIndex = i;
			}
		}

		Card selectedPlayerCard = playerCardHolder.cards[bestCardIndex];
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
