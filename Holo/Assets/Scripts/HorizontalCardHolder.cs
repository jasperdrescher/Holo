using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class HorizontalCardHolder : MonoBehaviour
{
    [SerializeField] private Card draggedCard;
    [SerializeReference] private Card hoveredCard;
    public Card selectedCard;

    [SerializeField] private GameObject slotPrefab;
    private RectTransform rect;

	[Header("Spawn Settings")]
	[SerializeField] private bool isPlayerDeck = false;
    [SerializeField] private int cardsToSpawn = 7;
	public List<Card> cards;
	
	[Header("Events")]
	[HideInInspector] public UnityEvent SelectedCardEvent;

    bool isCrossing = false;
    [SerializeField] private bool tweenCardReturn = true;

    void Start()
    {
        for (int i = 0; i < cardsToSpawn; i++)
        {
            Instantiate(slotPrefab, transform);
        }

        rect = GetComponent<RectTransform>();
        cards = GetComponentsInChildren<Card>().ToList();

        int cardCount = 0;

        foreach (Card card in cards)
        {
            card.PointerEnterEvent.AddListener(CardPointerEnter);
            card.PointerExitEvent.AddListener(CardPointerExit);
            card.BeginDragEvent.AddListener(BeginDrag);
            card.EndDragEvent.AddListener(EndDrag);
            card.SelectEvent.AddListener(Selected);
			card.name = cardCount.ToString();
			card.isPlayerCard = isPlayerDeck;
            cardCount++;
        }

        StartCoroutine(Frame());

        IEnumerator Frame()
		{
			yield return new WaitForSecondsRealtime(.1f);
			for (int i = 0; i < cards.Count; i++)
			{
				if (cards[i].cardVisual != null)
				{
					cards[i].cardVisual.UpdateIndex(transform.childCount);
				}
			}
		}

		SFXManager.instance.PlayCardSpreadSFX();
    }

    private void BeginDrag(Card card)
    {
        draggedCard = card;
        SFXManager.instance.PlayCardBeginDragSFX();
    }

	void EndDrag(Card card)
	{
		if (draggedCard == null)
			return;

		draggedCard.transform.DOLocalMove(draggedCard.isSelected ? new Vector3(0, draggedCard.selectionOffset, 0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);

		rect.sizeDelta += Vector2.right;
		rect.sizeDelta -= Vector2.right;

		draggedCard = null;

		SFXManager.instance.PlayCardEndDragSFX();
	}

	void Selected(Card card)
	{
		if (card.isSelected)
		{
			selectedCard = card;
		}
		else
		{
			selectedCard = null;
		}

		SelectedCardEvent.Invoke();
	}

    void CardPointerEnter(Card card)
    {
        hoveredCard = card;
        SFXManager.instance.PlayCardHoverSFX();
    }

    void CardPointerExit(Card card)
    {
        hoveredCard = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (hoveredCard != null)
            {
                Destroy(hoveredCard.transform.parent.gameObject);
                cards.Remove(hoveredCard);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach (Card card in cards)
            {
                card.Deselect();
            }
        }

        if (draggedCard == null)
            return;

        if (isCrossing)
            return;

        for (int i = 0; i < cards.Count; i++)
        {
            if (draggedCard.transform.position.x > cards[i].transform.position.x)
            {
                if (draggedCard.ParentIndex() < cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if (draggedCard.transform.position.x < cards[i].transform.position.x)
            {
                if (draggedCard.ParentIndex() > cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }
        }
    }

	void Swap(int index)
	{
		isCrossing = true;

		Transform focusedParent = draggedCard.transform.parent;
		Transform crossedParent = cards[index].transform.parent;

		cards[index].transform.SetParent(focusedParent);
		cards[index].transform.localPosition = cards[index].isSelected ? new Vector3(0, cards[index].selectionOffset, 0) : Vector3.zero;
		draggedCard.transform.SetParent(crossedParent);

		isCrossing = false;

		if (cards[index].cardVisual == null)
			return;

		bool swapIsRight = cards[index].ParentIndex() > draggedCard.ParentIndex();
		cards[index].cardVisual.Swap(swapIsRight ? -1 : 1);

		//Updated Visual Indexes
		foreach (Card card in cards)
		{
			card.cardVisual.UpdateIndex(transform.childCount);
		}
	}

	public void OnCardDied(Card deadCard)
	{
		Destroy(deadCard.transform.parent.gameObject);
		cards.Remove(deadCard);
	}

	public void DeselectAll()
	{
		foreach (Card card in cards)
		{
			card.Deselect();
		}
	}
}
