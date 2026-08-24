public class TcpControl81 : ReliableTcpControlBase
{
	private void OnEnable()
	{
		Initialise(Eventdefine.TcpConnect81, Eventdefine.TcpSend81, Eventdefine.TcpSendSingle81, "继电器81");
	}

	private void OnDisable()
	{
		Shutdown();
	}
}
