using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BatMud.BatClientBase;

namespace BatMud.BatClientWindows
{

	#region Enum HookType - Defines all supported Windows Hook types

	/// <summary>
	/// Enumeration containing all possible Windows Hook types
	/// </summary>
	internal enum HookType : int
	{
		JOURNALRECORD = 0,
		JOURNALPLAYBACK = 1,
		KEYBOARD = 2,
		GETMESSAGE = 3,
		CALLWNDPROC = 4,
		CBT = 5,
		SYSMSGFILTER = 6,
		MOUSE = 7,
		HARDWARE = 8,
		DEBUG = 9,
		SHELL = 10,
		FOREGROUNDIDLE = 11,
		CALLWNDPROCRET = 12,
		KEYBOARD_LL = 13,
		MOUSE_LL = 14
	}

	#endregion

	#region KeyState enum - Indicates whether a key is pressed or unpressed

	public enum KeyState : int
	{
		KeyDown = 1,
		KeyUp = 0
	}

	#endregion

	#region KeyDefinition class - Consolidates information about keyboard events

	[Serializable]
	public class KeyDefinition
	{

		public System.Windows.Forms.Keys KeyCode = Keys.None;
		public KeyState KeyState = KeyState.KeyDown;
		public bool IsAltDown = false;
		public bool IsCtrlDown = false;
		public bool IsShiftDown = false;

		public bool Equals(KeyDefinition e)
		{

			return
				KeyState == e.KeyState &&
				KeyCode == e.KeyCode &&
				IsAltDown == e.IsAltDown &&
				IsCtrlDown == e.IsCtrlDown &&
				IsShiftDown == e.IsShiftDown;

		}

	}

	#endregion

	#region LocalKeyboardHook class - Implements basic Windows Hook functionality

	public delegate void KeyboardEventHandler(KeyDefinition e);

	/// <summary>
	/// Core class for processing Windows Hook events.
	/// </summary>
	public class LocalKeyboardHook
	{

		#region Win32 API declarations, types, and definitions

		internal delegate int HookProc(int code, int wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		private static extern IntPtr SetWindowsHookEx(
			HookType code,
			HookProc func,
			IntPtr hInstance,
			int threadID);

		[DllImport("user32.dll")]
		private static extern int UnhookWindowsHookEx(IntPtr hhook);

		[DllImport("user32.dll")]
		private static extern int CallNextHookEx(
			IntPtr hhook,
			int code,
			int wParam,
			IntPtr lParam);

		[DllImport(@"user32.dll", CharSet = CharSet.Auto)]
		private static extern bool GetAsyncKeyState(Keys key);

		[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct KeyboardHookInfo
		{
			public int vkCode;
			public int scanCode;
			public int flags;
			public int time;
			public IntPtr dwExtraInfo;
		}

		private const uint KEYSTATE_DOWN = 0xffff8001;	// State of a key while pressed
		private const uint KEYSTATE_UP = 0x0;			// State of a key when not pressed

		#endregion

		#region Private data members

		private HookType hookType = HookType.KEYBOARD;
		private IntPtr hookHandle = IntPtr.Zero;
		private HookProc hookCallback = null;
		private int threadID = 0;
		private Keys lastKeyCode = 0;
		private bool lastKeyState = false;

		#endregion

		#region Constructor / Destructor

		public LocalKeyboardHook() { }

		~LocalKeyboardHook()
		{
			Detach();
		}

		#endregion

		#region Event handlers and declarations

		public event KeyboardEventHandler KeyboardEvent;
		private void OnKeyboardEvent(KeyDefinition args)
		{

			try
			{

				if (KeyboardEvent != null)
					KeyboardEvent(args);

			}
			catch { }

		}

		private int KeyboardHookHandler(int code, int wParam, IntPtr lParam)
		{

			try
			{

				if (code >= 0)
				{

					Keys key = (Keys)wParam;
					bool keyState = GetAsyncKeyState(key);

					if (key != lastKeyCode || keyState != lastKeyState)
					{

						lastKeyCode = key;
						lastKeyState = keyState;

						KeyDefinition args = new KeyDefinition();

						args.KeyCode = key;
						args.IsShiftDown = GetAsyncKeyState(Keys.ShiftKey);
						args.IsCtrlDown = GetAsyncKeyState(Keys.ControlKey);
						args.IsAltDown = GetAsyncKeyState(Keys.Menu);
						args.KeyState = (KeyState)(int)(keyState ? 1 : 0);

						OnKeyboardEvent(args);

					}

				}

			}
			catch (Exception err)
			{
				BatConsole.WriteError("Error in KeyboardHookHandler", err);
			}

			// Yield to the next hook in the chain
			return CallNextHookEx(hookHandle, code, wParam, lParam);

		}

		#endregion

		#region Attach() and Detach() methods - Set or release the Windows Hook

		public void Attach()
		{

			try
			{

				threadID = (int)System.Threading.Thread.CurrentThread.ManagedThreadId;
				hookCallback = new HookProc(this.KeyboardHookHandler);

				hookHandle = SetWindowsHookEx(
					hookType,
					hookCallback,
					IntPtr.Zero,
					threadID
					);

			}
			catch (Exception err)
			{
				System.Diagnostics.Debug.Assert(false, err.ToString());
			}

		}

		public void Detach()
		{
			if (IsAttached)
			{
				UnhookWindowsHookEx(hookHandle);
				hookHandle = IntPtr.Zero;
				hookCallback = null;
			}
		}

		public bool IsAttached
		{
			get
			{
				return hookCallback != null;
			}
		}

		#endregion

	}

	#endregion

}
