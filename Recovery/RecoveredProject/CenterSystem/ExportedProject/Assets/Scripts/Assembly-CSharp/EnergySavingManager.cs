using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EnergySavingManager : MonoBehaviour
{
	public enum DetectMode
	{
		GYRO = 0,
		KeyBoard = 1,
		Touch = 2,
		ANY = 3
	}

	public static EnergySavingManager instance;

	[Tooltip("Status")]
	public bool isSleeping;

	private bool isSleeping_old;

	[Tooltip("Detection Method")]
	public DetectMode DMode;

	public GameObject SleepModePanel;

	[HideInInspector]
	public float sleepTimer;

	[Header("Enter Sleep Mode in sec")]
	public float sleepThreshold = 5f;

	private float awakeTimer;

	[Header("Delay WakeUp in sec")]
	[Range(0f, 1f)]
	public float awakeThreshold = 1f;

	private Vector3[] attitudeCollection;

	private Vector3 sumAttitude;

	private Vector3 tmp_averageAttitude;

	private Vector3 averageAttitude;

	private float magnitude;

	private float shakeThreshold = 0.01f;

	[Tooltip("(On Editor Mode Only)")]
	public bool ForceAwake = true;

	[Tooltip("Disable Objects when enter sleep mode")]
	public GameObject[] disableGrp;

	[Space(10f)]
	public UnityEvent OnSleepModeEnter;

	public UnityEvent OnSleepModeExit;

	[Tooltip("Debug Clock(x hr, y min, z sec)")]
	public Vector3 DebugClock;

	public bool AutoReloadScene;

	[Tooltip("reload scene at (x hour, y min)")]
	public Vector2[] Schedule_Reload;

	public bool AutoQuitApp;

	[Tooltip("quit at (x hour, y min)")]
	public Vector2[] Schedule_Quit;

	private bool needReloadScene;

	private bool needQuitApp;

	[Tooltip("Touch screen with 5 fingers \nDebug shortcut: Keyboard R + Mouse Left")]
	public bool ForceReloadAppGesture;

	[Tooltip("Threshold in sec")]
	[Range(0f, 180f)]
	public float ForceReloadAppThreshold = 30f;

	private float ForceReloadAppGestureTimer;

	[Tooltip("Touch screen with 5 fingers \nDebug shortcut: Keyboard R + Mouse Left")]
	public bool ForceQuitAppGesture;

	[Tooltip("Threshold in sec")]
	[Range(0f, 180f)]
	public float ForceQuitAppThreshold = 30f;

	private float ForceQuitAppGestureTimer;

	private bool getGyroChanged()
	{
		sumAttitude = attitudeCollection[0];
		for (int i = 0; i < attitudeCollection.Length - 1; i++)
		{
			attitudeCollection[i + 1] = attitudeCollection[i];
			sumAttitude += attitudeCollection[i + 1];
		}
		attitudeCollection[0] = Input.gyro.attitude.eulerAngles;
		tmp_averageAttitude = averageAttitude;
		averageAttitude = sumAttitude / attitudeCollection.Length;
		magnitude = (averageAttitude - tmp_averageAttitude).magnitude;
		if (!(magnitude < shakeThreshold))
		{
			return true;
		}
		return false;
	}

	private bool getAnyKeyChanged()
	{
		if (!Input.anyKey)
		{
			return false;
		}
		return true;
	}

	private bool getTouchChanged()
	{
		if (Input.touchCount <= 0 && !Input.GetMouseButton(0))
		{
			return false;
		}
		return true;
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		Screen.sleepTimeout = -1;
	}

	private void Start()
	{
		Input.gyro.enabled = true;
		attitudeCollection = new Vector3[30];
		for (int i = 0; i < attitudeCollection.Length; i++)
		{
			attitudeCollection[i] = Vector3.zero;
		}
		averageAttitude = sumAttitude / attitudeCollection.Length;
	}

	private void Update()
	{
		isSleeping_old = isSleeping;
		bool flag = false;
		if (DMode == DetectMode.GYRO)
		{
			flag = getGyroChanged();
		}
		else if (DMode == DetectMode.KeyBoard)
		{
			flag = getAnyKeyChanged();
		}
		else if (DMode == DetectMode.Touch)
		{
			flag = getTouchChanged();
		}
		else if (DMode == DetectMode.ANY)
		{
			flag = getAnyKeyChanged() || getTouchChanged();
			flag = getGyroChanged();
		}
		sleepTimer = Mathf.Clamp(sleepTimer + ((!flag) ? Time.deltaTime : (0f - Time.deltaTime)), 0f, sleepThreshold);
		if (sleepTimer >= sleepThreshold)
		{
			isSleeping = true;
		}
		else if (sleepTimer <= sleepThreshold - awakeThreshold)
		{
			isSleeping = false;
		}
		if (isSleeping)
		{
			awakeTimer = 0f;
			if (SleepModePanel != null)
			{
				SleepModePanel.SetActive(value: true);
			}
		}
		else
		{
			awakeTimer = Mathf.Clamp(awakeTimer + Time.deltaTime, 0f, awakeThreshold);
			if (awakeTimer >= awakeThreshold)
			{
				if (SleepModePanel != null)
				{
					SleepModePanel.SetActive(value: false);
				}
			}
			else if (SleepModePanel != null)
			{
				SleepModePanel.SetActive(value: true);
			}
		}
		if (isSleeping && !isSleeping_old)
		{
			OnSleepModeEnter.Invoke();
			GameObject[] array = disableGrp;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
		else if (!isSleeping && isSleeping_old)
		{
			OnSleepModeExit.Invoke();
			GameObject[] array = disableGrp;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
		}
		if (AutoReloadScene)
		{
			Vector2[] schedule_Reload = Schedule_Reload;
			for (int i = 0; i < schedule_Reload.Length; i++)
			{
				Vector2 vector = schedule_Reload[i];
				Action_ReloadScene(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
			}
		}
		if (AutoQuitApp)
		{
			Vector2[] schedule_Reload = Schedule_Quit;
			for (int i = 0; i < schedule_Reload.Length; i++)
			{
				Vector2 vector2 = schedule_Reload[i];
				Action_QuitApp(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));
			}
		}
		if (ForceReloadAppGesture)
		{
			if (Input.touchCount >= 5 || (Input.GetMouseButton(0) && Input.GetKey(KeyCode.R)))
			{
				ForceReloadAppGestureTimer += Time.deltaTime;
				if (ForceReloadAppGestureTimer > ForceReloadAppThreshold)
				{
					Debug.Log("reload scene now!");
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				}
			}
			else
			{
				ForceReloadAppGestureTimer = 0f;
			}
		}
		if (ForceQuitAppGesture)
		{
			if (Input.touchCount >= 5 || (Input.GetMouseButton(0) && Input.GetKey(KeyCode.R)))
			{
				ForceQuitAppGestureTimer += Time.deltaTime;
				if (ForceQuitAppGestureTimer > ForceQuitAppThreshold)
				{
					Debug.Log("quit app now!");
					Application.Quit();
				}
			}
			else
			{
				ForceQuitAppGestureTimer = 0f;
			}
		}
		DebugClock = new Vector3(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
	}

	public void Action_ReloadScene(int _hr, int _min)
	{
		if (DateTime.Now.Hour == _hr && DateTime.Now.Minute == _min && !needReloadScene)
		{
			Debug.Log("start count down for reload scene!");
			needReloadScene = true;
			StartCoroutine(DelayReloadScene(_hr, _min));
		}
	}

	private IEnumerator DelayReloadScene(int _hr, int _min)
	{
		while (needReloadScene)
		{
			if (DateTime.Now.Hour != _hr || DateTime.Now.Minute != _min)
			{
				needReloadScene = false;
			}
			yield return new WaitForEndOfFrame();
		}
		Debug.Log("reload scene now!");
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		yield return null;
	}

	public void Action_QuitApp(int _hr, int _min)
	{
		if (DateTime.Now.Hour == _hr && DateTime.Now.Minute == _min && !needQuitApp)
		{
			Debug.Log("start count down for quit app!");
			needQuitApp = true;
			StartCoroutine(DelayQuitApp(_hr, _min));
		}
	}

	private IEnumerator DelayQuitApp(int _hr, int _min)
	{
		while (needQuitApp)
		{
			if (DateTime.Now.Hour != _hr || DateTime.Now.Minute != _min)
			{
				needQuitApp = false;
			}
			yield return new WaitForEndOfFrame();
		}
		Debug.Log("quit app now!");
		Application.Quit();
		yield return null;
	}
}
