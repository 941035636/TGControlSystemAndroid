using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameViewDecoder : MonoBehaviour
{
	public bool FastMode;

	public Texture2D ReceivedTexture;

	public GameObject TestQuad;

	public RawImage TestImg;

	public UnityEventTexture2D OnReceivedTexture2D;

	private bool ReadyToGetFrame = true;

	public int label = 1001;

	private int dataID;

	private int dataLength;

	private int receivedLength;

	private byte[] dataByte;

	public bool GZipMode;

	private void Start()
	{
		Application.runInBackground = true;
	}

	public void Action_ProcessImageData(byte[] _byteData)
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
			if (receivedLength == 0)
			{
				dataByte = new byte[dataLength];
			}
			receivedLength += _byteData.Length - 17;
			Buffer.BlockCopy(_byteData, 17, dataByte, dstOffset, _byteData.Length - 17);
			if (ReadyToGetFrame && receivedLength == dataLength)
			{
				StartCoroutine(ProcessImageData(dataByte));
			}
		}
	}

	private IEnumerator ProcessImageData(byte[] _byteData)
	{
		ReadyToGetFrame = false;
		try
		{
			if (GZipMode)
			{
				_byteData = _byteData.FMUnzipBytes();
			}
			if (FastMode)
			{
				ReceivedTexture.FMLoadJPG(ref ReceivedTexture, _byteData);
			}
			else
			{
				if (ReceivedTexture == null)
				{
					ReceivedTexture = new Texture2D(0, 0);
				}
				ReceivedTexture.LoadImage(_byteData);
			}
			if (ReceivedTexture.width <= 8)
			{
				throw new Exception("texture is smaller than 8 x 8, wrong data");
			}
			if (TestQuad != null)
			{
				TestQuad.GetComponent<Renderer>().material.mainTexture = ReceivedTexture;
			}
			if (TestImg != null)
			{
				TestImg.texture = ReceivedTexture;
			}
			OnReceivedTexture2D.Invoke(ReceivedTexture);
		}
		catch (Exception)
		{
			UnityEngine.Object.Destroy(ReceivedTexture);
		}
		ReadyToGetFrame = true;
		yield return null;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}
}
