using System;
using System.Collections;
using UnityEngine;

public class LargeFileDecoder : MonoBehaviour
{
	public UnityEventByteArray OnReceivedByteArray;

	private bool ReadyToGetFrame = true;

	[Header("Pair Encoder & Decoder")]
	public int label = 8001;

	private int dataID;

	private int dataLength;

	private int receivedLength;

	private byte[] dataByte;

	private void Start()
	{
		Application.runInBackground = true;
	}

	public void Action_ProcessData(byte[] _byteData)
	{
		if (_byteData.Length > 8 && BitConverter.ToInt32(_byteData, 0) == label)
		{
			int num = BitConverter.ToInt32(_byteData, 4);
			if (num != dataID)
			{
				receivedLength = 0;
			}
			dataID = num;
			dataLength = BitConverter.ToInt32(_byteData, 8);
			int dstOffset = BitConverter.ToInt32(_byteData, 12);
			if (receivedLength == 0)
			{
				dataByte = new byte[dataLength];
			}
			receivedLength += _byteData.Length - 16;
			Buffer.BlockCopy(_byteData, 16, dataByte, dstOffset, _byteData.Length - 16);
			if (ReadyToGetFrame && receivedLength == dataLength)
			{
				StartCoroutine(ProcessData(dataByte));
			}
		}
	}

	private IEnumerator ProcessData(byte[] _byteData)
	{
		ReadyToGetFrame = false;
		OnReceivedByteArray.Invoke(_byteData);
		ReadyToGetFrame = true;
		yield return null;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}
}
