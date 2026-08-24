using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicEncoder : MonoBehaviour
{
	private AudioSource AudioMic;

	private Queue<byte> AudioBytes = new Queue<byte>();

	public bool StreamGameSound = true;

	public int OutputSampleRate = 11025;

	public int OutputChannels = 1;

	private object _asyncLockAudio = new object();

	private int CurrentAudioTimeSample;

	private int LastAudioTimeSample;

	[Range(1f, 60f)]
	public float StreamFPS = 20f;

	private float interval = 0.05f;

	public bool GZipMode;

	public UnityEventByteArray OnDataByteReadyEvent;

	public int label = 2001;

	private int dataID;

	private int maxID = 1024;

	private int chunkSize = 8096;

	private float next;

	private bool stop;

	public int dataLength;

	private void Start()
	{
		StartCoroutine(CaptureMic());
		StartCoroutine(SenderCOR());
	}

	private IEnumerator CaptureMic()
	{
		if (AudioMic == null)
		{
			AudioMic = GetComponent<AudioSource>();
		}
		if (AudioMic == null)
		{
			AudioMic = base.gameObject.AddComponent<AudioSource>();
		}
		AudioMic.clip = Microphone.Start(null, loop: true, 1, OutputSampleRate);
		AudioMic.loop = true;
		while (Microphone.GetPosition(null) <= 0)
		{
		}
		Debug.Log("Start Mic(pos): " + Microphone.GetPosition(null));
		AudioMic.Play();
		AudioMic.volume = 0f;
		OutputChannels = AudioMic.clip.channels;
		while (!stop)
		{
			AddMicData();
			yield return null;
		}
		yield return null;
	}

	private void AddMicData()
	{
		LastAudioTimeSample = CurrentAudioTimeSample;
		CurrentAudioTimeSample = Microphone.GetPosition(null);
		if (CurrentAudioTimeSample == LastAudioTimeSample)
		{
			return;
		}
		float[] array = new float[AudioMic.clip.samples];
		AudioMic.clip.GetData(array, 0);
		if (CurrentAudioTimeSample > LastAudioTimeSample)
		{
			lock (_asyncLockAudio)
			{
				for (int i = LastAudioTimeSample; i < CurrentAudioTimeSample; i++)
				{
					byte[] bytes = BitConverter.GetBytes(array[i]);
					foreach (byte item in bytes)
					{
						AudioBytes.Enqueue(item);
					}
				}
				return;
			}
		}
		if (CurrentAudioTimeSample >= LastAudioTimeSample)
		{
			return;
		}
		lock (_asyncLockAudio)
		{
			for (int k = LastAudioTimeSample; k < array.Length; k++)
			{
				byte[] bytes = BitConverter.GetBytes(array[k]);
				foreach (byte item2 in bytes)
				{
					AudioBytes.Enqueue(item2);
				}
			}
			for (int l = 0; l < CurrentAudioTimeSample; l++)
			{
				byte[] bytes = BitConverter.GetBytes(array[l]);
				foreach (byte item3 in bytes)
				{
					AudioBytes.Enqueue(item3);
				}
			}
		}
	}

	private IEnumerator SenderCOR()
	{
		while (!stop)
		{
			if (Time.realtimeSinceStartup > next)
			{
				interval = 1f / StreamFPS;
				next = Time.realtimeSinceStartup + interval;
				EncodeBytes();
			}
			yield return null;
		}
	}

	private void EncodeBytes()
	{
		byte[] bytes = BitConverter.GetBytes(OutputSampleRate);
		byte[] bytes2 = BitConverter.GetBytes(OutputChannels);
		byte[] array;
		lock (_asyncLockAudio)
		{
			array = new byte[AudioBytes.Count + bytes.Length + bytes2.Length];
			Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
			Buffer.BlockCopy(bytes2, 0, array, 4, bytes2.Length);
			Buffer.BlockCopy(AudioBytes.ToArray(), 0, array, 8, AudioBytes.Count);
			AudioBytes.Clear();
		}
		if (GZipMode)
		{
			array = array.FMZipBytes();
		}
		dataLength = array.Length;
		int num = array.Length;
		int num2 = 0;
		byte[] bytes3 = BitConverter.GetBytes(label);
		byte[] bytes4 = BitConverter.GetBytes(dataID);
		byte[] bytes5 = BitConverter.GetBytes(num);
		int num3 = Mathf.FloorToInt(array.Length / chunkSize);
		for (int i = 0; i <= num3; i++)
		{
			byte[] bytes6 = BitConverter.GetBytes(num2);
			byte[] array2 = new byte[(i == num3) ? (num % chunkSize + 17) : (chunkSize + 17)];
			Buffer.BlockCopy(bytes3, 0, array2, 0, 4);
			Buffer.BlockCopy(bytes4, 0, array2, 4, 4);
			Buffer.BlockCopy(bytes5, 0, array2, 8, 4);
			Buffer.BlockCopy(bytes6, 0, array2, 12, 4);
			array2[16] = (byte)(GZipMode ? 1u : 0u);
			Buffer.BlockCopy(array, num2, array2, 17, array2.Length - 17);
			OnDataByteReadyEvent.Invoke(array2);
			num2 += chunkSize;
		}
		dataID++;
		if (dataID > maxID)
		{
			dataID = 0;
		}
	}

	private void OnEnable()
	{
		if (!(Time.realtimeSinceStartup <= 3f))
		{
			StartAll();
		}
	}

	private void OnDisable()
	{
		StopAll();
	}

	private void StartAll()
	{
		if (stop)
		{
			stop = false;
			StartCoroutine(SenderCOR());
			StartCoroutine(CaptureMic());
		}
	}

	private void StopAll()
	{
		stop = true;
		StopCoroutine(SenderCOR());
		StopCoroutine(CaptureMic());
		AudioMic.Stop();
		Microphone.End(null);
	}
}
