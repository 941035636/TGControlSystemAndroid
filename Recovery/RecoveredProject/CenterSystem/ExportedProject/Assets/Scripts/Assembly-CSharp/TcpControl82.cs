public class TcpControl82 : ReliableTcpControlBase
{
	private void OnEnable()
	{
		Initialise(Eventdefine.TcpConnect82, Eventdefine.TcpSend82, Eventdefine.TcpSendSingle82, "继电器82");
	}

	private void OnDisable()
	{
		Shutdown();
	}
}
