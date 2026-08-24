using System;
using System.Collections;
using UnityEngine;

public class LargeFileEncoder : MonoBehaviour
{
	public bool Sending;

	[Tooltip("smaller number: take longer time to send \nTry to reduce the number if the file cannot be sent successfully")]
	[Range(1f, 60f)]
	public int AddDelayEveryPacket = 4;

	public UnityEventByteArray OnDataByteReadyEvent;

	[Header("Pair Encoder & Decoder")]
	public int label = 8001;

	private int dataID;

	private int maxID = 1024;

	private int chunkSize = 8096;

	public int dataLength;

	private void Start()
	{
		Application.runInBackground = true;
	}

	public void Action_SendLargeByte(byte[] _data)
	{
		StartCoroutine(SenderCOR(_data));
	}

	private IEnumerator SenderCOR(byte[] dataByte)
	{
		yield return null;
		if (!Sending)
		{
			Sending = true;
			dataLength = dataByte.Length;
			int num = dataByte.Length;
			int num2 = 0;
			byte[] bytes = BitConverter.GetBytes(label);
			byte[] bytes2 = BitConverter.GetBytes(dataID);
			byte[] bytes3 = BitConverter.GetBytes(num);
			int num3 = Mathf.FloorToInt(dataByte.Length / chunkSize);
			for (int i = 0; i <= num3; i++)
			{
				int num4 = ((i == num3) ? (num % chunkSize + 16) : (chunkSize + 16));
				byte[] bytes4 = BitConverter.GetBytes(num2);
				byte[] array = new byte[num4];
				Buffer.BlockCopy(bytes, 0, array, 0, 4);
				Buffer.BlockCopy(bytes2, 0, array, 4, 4);
				Buffer.BlockCopy(bytes3, 0, array, 8, 4);
				Buffer.BlockCopy(bytes4, 0, array, 12, 4);
				Buffer.BlockCopy(dataByte, num2, array, 16, array.Length - 16);
				OnDataByteReadyEvent.Invoke(array);
				num2 += chunkSize;
			}
			dataID++;
			if (dataID > maxID)
			{
				dataID = 0;
			}
			Sending = false;
		}
	}

	private void OnDisable()
	{
		StopAll();
	}

	private void OnApplicationQuit()
	{
		StopAll();
	}

	private void OnDestroy()
	{
		StopAll();
	}

	private void StopAll()
	{
		StopAllCoroutines();
	}
}
