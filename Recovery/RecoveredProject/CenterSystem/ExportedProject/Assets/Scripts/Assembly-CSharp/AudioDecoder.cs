using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioDecoder : MonoBehaviour
{
	private bool ReadyToGetFrame = true;

	public int label = 2001;

	private int dataID;

	private int dataLength;

	private int receivedLength;

	private byte[] dataByte;

	public bool GZipMode;

	public int SourceChannels = 1;

	public double SourceSampleRate = 48000.0;

	public double DeviceSampleRate = 48000.0;

	private Queue<float> ABufferQueue = new Queue<float>();

	private object _asyncLock = new object();

	private int position;

	private int samplerate = 44100;

	private int channel = 2;

	private AudioClip myClip;

	private AudioSource Audio;

	private void Start()
	{
		Application.runInBackground = true;
		DeviceSampleRate = AudioSettings.GetConfiguration().sampleRate;
	}

	public void Action_ProcessData(byte[] _byteData)
	{
		if (base.enabled && _byteData.Length > 8 && BitConverter.ToInt32(_byteData, 0) == label)
		{
			int num = BitConverter.ToInt32(_byteData, 4);
			if (num != dataID)
			{
				receivedLength = 0;
			}
			dataID = num;
			dataLength = BitConverter.ToInt32(_byteData, 8);
			int dstOffset = BitConverter.ToInt32(_byteData, 12);
			GZipMode = _byteData[16] == 1;
			Debug.Log("GZipMode: " + GZipMode);
			if (receivedLength == 0)
			{
				dataByte = new byte[dataLength];
			}
			receivedLength += _byteData.Length - 17;
			Buffer.BlockCopy(_byteData, 17, dataByte, dstOffset, _byteData.Length - 17);
			if (ReadyToGetFrame && receivedLength == dataLength)
			{
				StartCoroutine(ProcessAudioData(dataByte));
			}
		}
	}

	private IEnumerator ProcessAudioData(byte[] receivedAudioBytes)
	{
		if (Application.platform != RuntimePlatform.WebGLPlayer)
		{
			ReadyToGetFrame = false;
			if (GZipMode)
			{
				receivedAudioBytes = receivedAudioBytes.FMUnzipBytes();
			}
			if (receivedAudioBytes.Length >= 1032)
			{
				byte[] array = new byte[4];
				byte[] array2 = new byte[4];
				byte[] array3 = new byte[1];
				lock (_asyncLock)
				{
					array3 = new byte[receivedAudioBytes.Length - 8];
					Buffer.BlockCopy(receivedAudioBytes, 0, array, 0, array.Length);
					Buffer.BlockCopy(receivedAudioBytes, 4, array2, 0, array2.Length);
					Buffer.BlockCopy(receivedAudioBytes, 8, array3, 0, array3.Length);
				}
				SourceSampleRate = BitConverter.ToInt32(array, 0);
				SourceChannels = BitConverter.ToInt32(array2, 0);
				float[] array4 = ToFloatArray(array3);
				for (int i = 0; i < array4.Length; i++)
				{
					ABufferQueue.Enqueue(array4[i]);
				}
				CreateClip();
			}
			ReadyToGetFrame = true;
		}
		yield return null;
	}

	private void CreateClip()
	{
		if (samplerate != (int)SourceSampleRate || channel != SourceChannels)
		{
			samplerate = (int)SourceSampleRate;
			channel = SourceChannels;
			if (Audio != null)
			{
				Audio.Stop();
			}
			if (myClip != null)
			{
				UnityEngine.Object.DestroyImmediate(myClip);
			}
			myClip = AudioClip.Create("StreamingAudio", samplerate * SourceChannels, SourceChannels, samplerate, stream: true, OnAudioRead, OnAudioSetPosition);
			Audio = GetComponent<AudioSource>();
			Audio.clip = myClip;
			Audio.loop = true;
			Audio.Play();
		}
	}

	private void OnAudioRead(float[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			if (ABufferQueue.Count > 0)
			{
				lock (_asyncLock)
				{
					data[i] = ABufferQueue.Dequeue();
				}
			}
			else
			{
				data[i] = 0f;
			}
			position++;
		}
	}

	private void OnAudioSetPosition(int newPosition)
	{
		position = newPosition;
	}

	public float[] ToFloatArray(byte[] byteArray)
	{
		float[] array = new float[byteArray.Length / 4];
		for (int i = 0; i < byteArray.Length; i += 4)
		{
			array[i / 4] = BitConverter.ToSingle(byteArray, i);
		}
		return array;
	}
}
