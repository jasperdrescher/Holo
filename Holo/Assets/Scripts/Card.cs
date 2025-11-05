
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
	private Canvas canvas;
	private Image imageComponent;
	[SerializeField] private bool instantiateVisual = true;
	private VisualCardsHandler visualHandler;
	private Vector3 offset;

	public int hitpoints = 10;
	public int strength = 2;
	public int cost = 1;

	public bool isPlayerCard = false;

	[Header("Movement")]
	[SerializeField] private float moveSpeedLimit = 50;

	[Header("Selection")]
	public bool isSelected = false;
	public float selectionOffset = 50;
	private float pointerDownTime;
	private float pointerUpTime;

	[Header("Visual")]
	[SerializeField] private GameObject cardVisualPrefab;
	[HideInInspector] public CardVisual cardVisual;

	[Header("States")]
	public bool isHovering = false;
	public bool isDragging = false;
	[HideInInspector] public bool wasDragged;

	[Header("Events")]
	[HideInInspector] public UnityEvent<Card> PointerEnterEvent;
	[HideInInspector] public UnityEvent<Card> PointerExitEvent;
	[HideInInspector] public UnityEvent<Card, bool> PointerUpEvent;
	[HideInInspector] public UnityEvent<Card> PointerDownEvent;
	[HideInInspector] public UnityEvent<Card> BeginDragEvent;
	[HideInInspector] public UnityEvent<Card> EndDragEvent;
	[HideInInspector] public UnityEvent<Card> SelectEvent;

	private BattleManager _battleManager;

	void Start()
	{
		_battleManager = FindFirstObjectByType<BattleManager>();
		if (!_battleManager)
			Debug.LogError("Failed to find Battle Manager");

		canvas = GetComponentInParent<Canvas>();
		imageComponent = GetComponent<Image>();

		if (!instantiateVisual)
			return;

		visualHandler = FindFirstObjectByType<VisualCardsHandler>();
		cardVisual = Instantiate(cardVisualPrefab, visualHandler ? visualHandler.transform : canvas.transform).GetComponent<CardVisual>();
		cardVisual.Initialize(this);
	}

	void Update()
	{
		ClampPosition();

		if (isDragging)
		{
			Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
			Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
			Vector2 velocity = direction * Mathf.Min(moveSpeedLimit, Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);
			transform.Translate(velocity * Time.deltaTime);
		}
	}

	void ClampPosition()
	{
		Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
		Vector3 clampedPosition = transform.position;
		clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
		clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
		transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (_battleManager.turnAuthor == TurnAuthor.AI)
			return;

		BeginDragEvent.Invoke(this);
		Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		offset = mousePosition - (Vector2)transform.position;
		isDragging = true;
		canvas.GetComponent<GraphicRaycaster>().enabled = false;
		imageComponent.raycastTarget = false;

		wasDragged = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		// Required for interface
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		EndDragEvent.Invoke(this);
		isDragging = false;
		canvas.GetComponent<GraphicRaycaster>().enabled = true;
		imageComponent.raycastTarget = true;

		StartCoroutine(FrameWait());

		IEnumerator FrameWait()
		{
			yield return new WaitForEndOfFrame();
			wasDragged = false;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (_battleManager.turnAuthor == TurnAuthor.AI)
			return;

		PointerEnterEvent.Invoke(this);
		isHovering = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		PointerExitEvent.Invoke(this);
		isHovering = false;
	}


	public void OnPointerDown(PointerEventData eventData)
	{
		if (_battleManager.turnAuthor == TurnAuthor.AI)
			return;

		if (eventData.button != PointerEventData.InputButton.Left)
			return;

		PointerDownEvent.Invoke(this);
		pointerDownTime = Time.time;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (_battleManager.turnAuthor == TurnAuthor.AI)
			return;

		if (eventData.button != PointerEventData.InputButton.Left)
			return;

		pointerUpTime = Time.time;

		PointerUpEvent.Invoke(this, pointerUpTime - pointerDownTime > .2f);

		if (pointerUpTime - pointerDownTime > .2f)
			return;

		if (wasDragged)
			return;

		isSelected = !isSelected;
		SelectEvent.Invoke(this);

		if (isSelected)
			transform.localPosition += cardVisual.transform.up * selectionOffset;
		else
			transform.localPosition = Vector3.zero;
	}

	public void AISelect()
	{
		isSelected = !isSelected;
		SelectEvent.Invoke(this);

		if (isSelected)
			transform.localPosition += cardVisual.transform.up * selectionOffset;
		else
			transform.localPosition = Vector3.zero;
	}

	public void Deselect()
	{
		if (isSelected)
		{
			isSelected = false;
			if (isSelected)
				transform.localPosition += cardVisual.transform.up * 50;
			else
				transform.localPosition = Vector3.zero;
		}

		SelectEvent.Invoke(this);
	}

	public int SiblingAmount()
	{
		return transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;
	}

	public int ParentIndex()
	{
		return transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
	}

	public float NormalizedPosition()
	{
		return transform.parent.CompareTag("Slot") ? ExtensionMethods.Remap(ParentIndex(), 0, transform.parent.parent.childCount - 1, 0, 1) : 0;
	}

	private void OnDestroy()
	{
		if (cardVisual != null)
			Destroy(cardVisual.gameObject);
	}
}
