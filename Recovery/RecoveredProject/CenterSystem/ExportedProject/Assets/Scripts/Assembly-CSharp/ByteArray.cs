using System;

public class ByteArray
{
	private const int DEFAULT_SIZE = 1024;

	private int initSize;

	public byte[] bytes;

	public int readIdx;

	public int writeIdx;

	private int Capacity;

	public int remain => Capacity - writeIdx;

	public int length => writeIdx - readIdx;

	public ByteArray(int size = 1024)
	{
		bytes = new byte[size];
		Capacity = size;
		initSize = size;
		readIdx = 0;
		writeIdx = 0;
	}

	public ByteArray(byte[] defaultBytes)
	{
		bytes = defaultBytes;
		Capacity = defaultBytes.Length;
		initSize = defaultBytes.Length;
		readIdx = 0;
		writeIdx = defaultBytes.Length;
	}

	public void ReSize(int size)
	{
		if (size >= length && size >= initSize)
		{
			int num;
			for (num = 1; num < size; num *= 2)
			{
			}
			Capacity = num;
			byte[] destinationArray = new byte[Capacity];
			Array.Copy(bytes, readIdx, destinationArray, 0, writeIdx - readIdx);
			bytes = destinationArray;
			writeIdx = length;
			readIdx = 0;
		}
	}

	public void MoveBytes()
	{
		Array.Copy(bytes, readIdx, bytes, 0, length);
		writeIdx = length;
		readIdx = 0;
	}

	public void CheckAndMoveBytes()
	{
		if (length < 8)
		{
			MoveBytes();
		}
	}
}
