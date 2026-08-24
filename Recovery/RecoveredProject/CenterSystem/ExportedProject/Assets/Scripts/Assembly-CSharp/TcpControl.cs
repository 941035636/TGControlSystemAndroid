public class TcpControl : ReliableTcpControlBase
{
	private void OnEnable()
	{
		Initialise(Eventdefine.TcpConnect, Eventdefine.TcpSend, Eventdefine.TcpSendSingle, "TCP控制器");
	}

	private void OnDisable()
	{
		Shutdown();
	}
}
