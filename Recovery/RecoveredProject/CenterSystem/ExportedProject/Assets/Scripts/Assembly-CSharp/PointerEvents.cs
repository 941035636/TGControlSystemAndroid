using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class PointerEvents : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Image mask;

	public bool isPressed;

	public Color ColorOnNormal = new Color(1f, 1f, 1f, 1f);

	public Color ColorOnPress = new Color(1f, 1f, 1f, 1f);

	[Header("On Press Event")]
	public UnityEvent OnPressEvent;

	[Header("On Press Enter Event")]
	public UnityEvent OnPressEnterEvent;

	[Header("On Press Exit Event")]
	public UnityEvent OnPressExitEvent;

	private void Start()
	{
		ColorOnNormal = mask.color;
	}

	private void Update()
	{
		if (isPressed)
		{
			OnPressEvent.Invoke();
			mask.color = ColorOnPress;
		}
		else
		{
			mask.color = ColorOnNormal;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		isPressed = true;
		OnPressEnterEvent.Invoke();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isPressed = false;
		OnPressExitEvent.Invoke();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
	}

	public void OnPointerExit(PointerEventData eventData)
	{
	}
}
