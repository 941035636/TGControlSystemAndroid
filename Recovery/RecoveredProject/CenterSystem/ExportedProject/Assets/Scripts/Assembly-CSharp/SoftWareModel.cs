using System.Collections.Generic;

public class SoftWareModel
{
	private string Name;

	private string Note;

	private string Path;

	private string ip;

	private string Port;

	private string ParentName;

	private string TingName;

	private bool isVideo;

	private bool isSheBeiControl;

	private bool isPhotoControl;

	private bool isopen;

	private List<string> ShowContentList = new List<string>();

	public List<string> Get_ShowContenList
	{
		get
		{
			return ShowContentList;
		}
		set
		{
			ShowContentList = value;
		}
	}

	public string Get_ParentName
	{
		get
		{
			return ParentName;
		}
		set
		{
			ParentName = value;
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

	public string GetNote
	{
		get
		{
			return Note;
		}
		set
		{
			Note = value;
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

	public string GetIp
	{
		get
		{
			return ip;
		}
		set
		{
			ip = value;
		}
	}

	public string GetPort
	{
		get
		{
			return Port;
		}
		set
		{
			Port = value;
		}
	}

	public bool GetIsVideo
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

	public bool GetIsSheBei
	{
		get
		{
			return isSheBeiControl;
		}
		set
		{
			isSheBeiControl = value;
		}
	}

	public bool GetIsPhoto
	{
		get
		{
			return isPhotoControl;
		}
		set
		{
			isPhotoControl = value;
		}
	}

	public bool Get_IsOpen
	{
		get
		{
			return isopen;
		}
		set
		{
			isopen = value;
		}
	}

	public string Get_TingName
	{
		get
		{
			return TingName;
		}
		set
		{
			TingName = value;
		}
	}
}
