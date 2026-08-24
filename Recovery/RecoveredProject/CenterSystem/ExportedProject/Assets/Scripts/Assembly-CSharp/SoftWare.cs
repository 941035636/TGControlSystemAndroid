using System.Collections.Generic;

public class SoftWare
{
	private string Name;

	private string ModelName;

	private string Path;

	private string ip1;

	private int Port1;

	private string ip2;

	private int Port2;

	private string hwip;

	private bool isVideo;

	private List<string> VideoList;

	public List<string> GetVideoList
	{
		get
		{
			return VideoList;
		}
		set
		{
			VideoList = value;
		}
	}

	public string GetName
	{
		get
		{
			return Name;
		}
		set
		{
			Name = value;
		}
	}

	public string GetHWIP
	{
		get
		{
			return hwip;
		}
		set
		{
			hwip = value;
		}
	}

	public string GetModelName
	{
		get
		{
			return ModelName;
		}
		set
		{
			ModelName = value;
		}
	}

	public string GetPath
	{
		get
		{
			return Path;
		}
		set
		{
			Path = value;
		}
	}

	public string GetIP1
	{
		get
		{
			return ip1;
		}
		set
		{
			ip1 = value;
		}
	}

	public int GetPort1
	{
		get
		{
			return Port1;
		}
		set
		{
			Port1 = value;
		}
	}

	public string GetIP2
	{
		get
		{
			return ip2;
		}
		set
		{
			ip2 = value;
		}
	}

	public int GetPort2
	{
		get
		{
			return Port2;
		}
		set
		{
			Port2 = value;
		}
	}

	public bool ISVideo
	{
		get
		{
			return isVideo;
		}
		set
		{
			isVideo = value;
		}
	}
}
