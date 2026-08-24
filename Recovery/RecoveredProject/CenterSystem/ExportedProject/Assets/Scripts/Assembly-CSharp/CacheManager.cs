public class CacheManager : Singleton<CacheManager>
{
	public AccControl Account;

	public AdminControl Admin;

	protected override void Awake()
	{
		base.Awake();
		Account = new AccControl();
		Admin = new AdminControl();
	}
}
