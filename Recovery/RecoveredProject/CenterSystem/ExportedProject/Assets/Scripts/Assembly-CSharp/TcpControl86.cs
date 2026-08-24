public class TcpControl86 : ReliableTcpControlBase
{
	private void OnEnable()
	{
		Initialise(Eventdefine.TcpConnect86, Eventdefine.TcpSend86, Eventdefine.TcpSendSingle86, "继电器86");
	}

	private void OnDisable()
	{
		Shutdown();
	}
}
