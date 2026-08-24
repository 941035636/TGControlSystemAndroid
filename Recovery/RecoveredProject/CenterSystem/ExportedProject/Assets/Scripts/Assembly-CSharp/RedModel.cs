using UnityEngine;

[SerializeField]
public class RedModel
{
	private string redkinds;

	private int channelNum;

	private string redip;

	private int redPort;

	private int AllChannelNum;

	private string redname;

	private RedOrder order;

	public int GetAllChannelNum
	{
		get
		{
			return AllChannelNum;
		}
		set
		{
			AllChannelNum = value;
		}
	}

	public string Getredkinds
	{
		get
		{
			return redkinds;
		}
		set
		{
			redkinds = value;
		}
	}

	public int GetChannelNum
	{
		get
		{
			return channelNum;
		}
		set
		{
			channelNum = value;
		}
	}

	public string GetredIp
	{
		get
		{
			return redip;
		}
		set
		{
			redip = value;
		}
	}

	public int GetRedPort
	{
		get
		{
			return redPort;
		}
		set
		{
			redPort = value;
		}
	}

	public string GetredName
	{
		get
		{
			return redname;
		}
		set
		{
			redname = value;
		}
	}

	public RedOrder GetOrder
	{
		get
		{
			return order;
		}
		set
		{
			order = value;
		}
	}
}
