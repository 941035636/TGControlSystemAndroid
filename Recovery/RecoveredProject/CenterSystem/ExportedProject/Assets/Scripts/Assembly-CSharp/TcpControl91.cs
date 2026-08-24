public class TcpControl91 : ReliableTcpControlBase
{
	private void OnEnable()
	{
		Initialise(Eventdefine.TcpConnect91, Eventdefine.TcpSend91, Eventdefine.TcpSendSingle91, "红外模块91");
	}

	private void OnDisable()
	{
		Shutdown();
	}
}
