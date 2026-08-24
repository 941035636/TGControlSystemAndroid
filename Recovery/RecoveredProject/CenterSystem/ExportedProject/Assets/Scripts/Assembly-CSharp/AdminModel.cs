public class AdminModel
{
	private int uid;

	private string lastUser;

	private string lastNetUser;

	private string lastNetUserPwd;

	private string adminName;

	private string adminPwd;

	private string registerdate;

	private int prescription;

	private int authority;

	private string deadlinedate;

	private int longtime;

	private string productNum;

	private string activeCode;

	private bool isRemPwd;

	private bool isActive;

	public int Uid
	{
		get
		{
			return uid;
		}
		set
		{
			uid = value;
		}
	}

	public string Registerdate
	{
		get
		{
			return registerdate;
		}
		set
		{
			registerdate = value;
		}
	}

	public int Prescription
	{
		get
		{
			return prescription;
		}
		set
		{
			prescription = value;
		}
	}

	public int Authority
	{
		get
		{
			return authority;
		}
		set
		{
			authority = value;
		}
	}

	public string Deadlinedate
	{
		get
		{
			return deadlinedate;
		}
		set
		{
			deadlinedate = value;
		}
	}

	public int Longtime
	{
		get
		{
			return longtime;
		}
		set
		{
			longtime = value;
		}
	}

	public string LastNetUserPwd
	{
		get
		{
			return lastNetUserPwd;
		}
		set
		{
			lastNetUserPwd = value;
		}
	}

	public string ActiveCode
	{
		get
		{
			return activeCode;
		}
		set
		{
			activeCode = value;
		}
	}

	public string AdminName
	{
		get
		{
			return adminName;
		}
		set
		{
			adminName = value;
		}
	}

	public string AdminPwd
	{
		get
		{
			return adminPwd;
		}
		set
		{
			adminPwd = value;
		}
	}

	public string ProductNum
	{
		get
		{
			return productNum;
		}
		set
		{
			productNum = value;
		}
	}

	public string LastUser
	{
		get
		{
			return lastUser;
		}
		set
		{
			lastUser = value;
		}
	}

	public string LastNetUser
	{
		get
		{
			return lastNetUser;
		}
		set
		{
			lastNetUser = value;
		}
	}

	public bool IsRemPwd
	{
		get
		{
			return isRemPwd;
		}
		set
		{
			isRemPwd = value;
		}
	}

	public bool IsActive
	{
		get
		{
			return isActive;
		}
		set
		{
			isActive = value;
		}
	}

	public AdminModel()
	{
		lastUser = null;
		adminName = null;
		AdminPwd = null;
		ActiveCode = null;
		ProductNum = null;
		IsRemPwd = true;
		IsActive = false;
		registerdate = null;
		prescription = 0;
		authority = 0;
		deadlinedate = null;
		longtime = 0;
	}
}
