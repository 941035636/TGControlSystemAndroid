using System;
using System.Runtime.InteropServices;

public class TopMostWindow
{
	public const int HWND_TOP = 0;

	public const int HWND_BOTTOM = 1;

	public const int HWND_TOPMOST = -1;

	public const int HWND_NOTOPMOST = -2;

	[DllImport("user32.dll")]
	public static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint wFlags);

	[DllImport("user32.dll")]
	public static extern bool GetWindowRect(IntPtr hWnd, out WindowRect lpRect);

	public static void SetTopomost(IntPtr hWnd)
	{
		WindowRect lpRect = default(WindowRect);
		GetWindowRect(hWnd, out lpRect);
		SetWindowPos(hWnd, (IntPtr)(-1), lpRect.Left, lpRect.Top, lpRect.Right - lpRect.Left, lpRect.Bottom - lpRect.Top, 0u);
	}
}
