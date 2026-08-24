public class TcpControl85 : ReliableTcpControlBase
{
	private void OnEnable()
	{
		Initialise(Eventdefine.TcpConnect85, Eventdefine.TcpSend85, Eventdefine.TcpSendSingle85, "继电器85");
	}

	private void OnDisable()
	{
		Shutdown();
	}
}
