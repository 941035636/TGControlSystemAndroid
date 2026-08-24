public class TcpControl90 : ReliableTcpControlBase
{
	private void OnEnable()
	{
		Initialise(Eventdefine.TcpConnect90, Eventdefine.TcpSend90, Eventdefine.TcpSendSingle90, "红外模块90");
	}

	private void OnDisable()
	{
		Shutdown();
	}
}
