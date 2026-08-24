using System.Runtime.InteropServices;

public class WindowDll
{
	[DllImport("Comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true)]
	public static extern bool GetOpenFileName([In][Out] OpenFileName ofn);

	public static bool GetOpenFileName1([In][Out] OpenFileName ofn)
	{
		return GetOpenFileName(ofn);
	}
}
