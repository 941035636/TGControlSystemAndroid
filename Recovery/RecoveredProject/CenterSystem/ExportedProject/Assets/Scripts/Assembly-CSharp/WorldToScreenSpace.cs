using UnityEngine;
using UnityEngine.Events;

public class WorldToScreenSpace : MonoBehaviour
{
	public Transform reference;

	public RectTransform[] targetRect;

	[Header("When target on screen")]
	public UnityEvent OnScreenEvent;

	[Header("When target off screen")]
	public UnityEvent OffScreenEvent;

	private void Start()
	{
		OffScreenEvent.Invoke();
	}

	private void Update()
	{
		Vector3 position = Camera.main.WorldToScreenPoint(reference.position);
		RectTransform[] array = targetRect;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].position = position;
		}
		if (position.z < 0f || position.x < (0f - (float)Screen.width) / 2f || position.x > (float)Screen.width * 3f / 2f || position.y < (0f - (float)Screen.height) / 2f || position.y > (float)Screen.height * 3f / 2f)
		{
			OffScreenEvent.Invoke();
		}
		else
		{
			OnScreenEvent.Invoke();
		}
	}

	public void InvokeOnScreen()
	{
		OnScreenEvent.Invoke();
	}

	public void InvokeOffScreen()
	{
		OffScreenEvent.Invoke();
	}

	private void OnDisable()
	{
		OffScreenEvent.Invoke();
	}

	private void OnEnable()
	{
	}

	public void DebugTest(string _text)
	{
		MonoBehaviour.print("Trigger Debug: " + _text);
	}
}
