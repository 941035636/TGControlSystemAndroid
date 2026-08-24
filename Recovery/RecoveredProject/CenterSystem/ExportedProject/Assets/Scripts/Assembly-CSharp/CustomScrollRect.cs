using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomScrollRect : ScrollRect
{
	public enum Direction
	{
		Horizontal = 0,
		Vertical = 1
	}

	private CustomScrollRect m_Parent;

	private Direction m_Direction;

	private Direction m_BeginDragDirection;

	protected override void Awake()
	{
		base.Awake();
		Transform parent = base.transform.parent;
		if ((bool)parent)
		{
			m_Parent = parent.GetComponentInParent<CustomScrollRect>();
		}
		m_Direction = ((!base.horizontal) ? Direction.Vertical : Direction.Horizontal);
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		if ((bool)m_Parent)
		{
			m_BeginDragDirection = ((!(Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))) ? Direction.Vertical : Direction.Horizontal);
			if (m_BeginDragDirection != m_Direction)
			{
				ExecuteEvents.Execute(m_Parent.gameObject, eventData, ExecuteEvents.beginDragHandler);
				return;
			}
		}
		base.OnBeginDrag(eventData);
	}

	public override void OnDrag(PointerEventData eventData)
	{
		if ((bool)m_Parent && m_BeginDragDirection != m_Direction)
		{
			ExecuteEvents.Execute(m_Parent.gameObject, eventData, ExecuteEvents.dragHandler);
		}
		else
		{
			base.OnDrag(eventData);
		}
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		if ((bool)m_Parent && m_BeginDragDirection != m_Direction)
		{
			ExecuteEvents.Execute(m_Parent.gameObject, eventData, ExecuteEvents.endDragHandler);
		}
		else
		{
			base.OnEndDrag(eventData);
		}
	}

	public override void OnScroll(PointerEventData data)
	{
		if ((bool)m_Parent && m_BeginDragDirection != m_Direction)
		{
			ExecuteEvents.Execute(m_Parent.gameObject, data, ExecuteEvents.scrollHandler);
		}
		else
		{
			base.OnScroll(data);
		}
	}
}
