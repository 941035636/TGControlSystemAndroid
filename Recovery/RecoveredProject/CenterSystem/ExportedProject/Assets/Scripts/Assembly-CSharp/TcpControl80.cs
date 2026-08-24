public class TcpControl80 : ReliableTcpControlBase
{
	private void OnEnable()
	{
		Initialise(Eventdefine.TcpConnect80, Eventdefine.TcpSend80, Eventdefine.TcpSendSingle80, "继电器80");
	}

	private void OnDisable()
	{
		Shutdown();
	}
}
