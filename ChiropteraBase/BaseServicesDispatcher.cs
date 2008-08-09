#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace Chiroptera.Base
{
	public delegate void ConnectEventDelegate(Exception exception);
	public delegate void DisconnectEventDelegate();
	public delegate ColorMessage ReceiveEventDelegate(ColorMessage colorMessage);
	public delegate	ColorMessage PromptEventDelegate(ColorMessage prompt);
	public delegate	bool TelnetEventDelegate(Telnet.TelnetCodes code, Telnet.TelnetOpts opt);
	public delegate	string InputEventDelegate(string input);
	public delegate ColorMessage OutputEventDelegate(ColorMessage colorMessage);
	public delegate bool KeyDownEventDelegate(System.Windows.Forms.Keys ckey);

	public class BaseServicesDispatcher
	{
		event ConnectEventDelegate ConnectEvent;
		event DisconnectEventDelegate DisconnectEvent;
		event ReceiveEventDelegate ReceiveEvent;
		event PromptEventDelegate PromptEvent;
		event TelnetEventDelegate TelnetEvent;
		event InputEventDelegate InputEvent;
		event OutputEventDelegate OutputEvent;
		event KeyDownEventDelegate KeyDownEvent;

		public BaseServicesDispatcher()
		{
		}

		public void RegisterConnectHandler(ConnectEventDelegate handler)
		{
			ConnectEvent += handler;
		}

		public void RegisterDisconnectHandler(DisconnectEventDelegate handler)
		{
			DisconnectEvent += handler;
		}

		public void RegisterMessageHandler(ReceiveEventDelegate handler)
		{
			ReceiveEvent += handler;
		}

		public void RegisterPromptHandler(PromptEventDelegate handler)
		{
			PromptEvent += handler;
		}

		public void RegisterTelnetHandler(TelnetEventDelegate handler)
		{
			TelnetEvent += handler;
		}

		public void RegisterInputHandler(InputEventDelegate handler)
		{
			InputEvent += handler;
		}

		public void RegisterOutputHandler(OutputEventDelegate handler)
		{
			OutputEvent += handler;
		}

		public void RegisterKeyDownHandler(KeyDownEventDelegate handler)
		{
			KeyDownEvent += handler;
		}


		public void DispatchConnectEvent(Exception exception)
		{
			if (ConnectEvent == null)
				return;

			try
			{
				ConnectEvent(exception);
			}
			catch (Exception e)
			{
				ChiConsole.WriteError("Error calling connect handler", e);
			}
		}

		public void DispatchDisconnectEvent()
		{
			if (DisconnectEvent == null)
				return;

			try
			{
				DisconnectEvent();
			}
			catch (Exception e)
			{
				ChiConsole.WriteError("Error calling disconnect handler", e);
			}
		}

		public ColorMessage DispatchReceiveColorMessage(ColorMessage colorMessage)
		{
			if (ReceiveEvent == null)
				return colorMessage;

			foreach (ReceiveEventDelegate del in ReceiveEvent.GetInvocationList())
			{
				try
				{
					colorMessage = del(colorMessage);
				}
				catch (Exception e)
				{
					ChiConsole.WriteError("Error calling colormessage handler", e);
				}

				if (colorMessage == null)
					break;
			}

			return colorMessage;
		}

		public ColorMessage DispatchPromptEvent(ColorMessage colorPrompt)
		{
			if (PromptEvent == null)
				return colorPrompt;

			foreach (PromptEventDelegate del in PromptEvent.GetInvocationList())
			{
				try
				{
					colorPrompt = del(colorPrompt);
				}
				catch (Exception e)
				{
					ChiConsole.WriteError("Error calling prompt handler", e);
				}

				if (colorPrompt == null)
					break;
			}

			return colorPrompt;
		}

		public void DispatchTelnetEvent(Telnet.TelnetCodes code, Telnet.TelnetOpts opt)
		{
			if (TelnetEvent == null)
				return;

			try
			{
				TelnetEvent(code, opt);
			}
			catch (Exception e)
			{
				ChiConsole.WriteError("Error calling telnet handler", e);
			}
		}

		public string DispatchInputEvent(string input)
		{
			if (InputEvent == null)
				return input;

			foreach (InputEventDelegate del in InputEvent.GetInvocationList())
			{
				try
				{
					input = del(input);
				}
				catch (Exception e)
				{
					ChiConsole.WriteError("Error calling input handler", e);
				}

				if (input == null)
					break;
			}

			return input;
		}

		public ColorMessage DispatchOutputMessage(ColorMessage colorMessage)
		{
			if (OutputEvent == null)
				return colorMessage;

			foreach (OutputEventDelegate del in OutputEvent.GetInvocationList())
			{
				try
				{
					colorMessage = del(colorMessage);
				}
				catch (Exception e)
				{
					ChiConsole.WriteError("Error calling output handler", e);
				}

				if (colorMessage == null)
					break;
			}

			return colorMessage;
		}

		public bool DispatchKeyDownEvent(System.Windows.Forms.Keys ckey)
		{
			if (KeyDownEvent == null)
				return false;

			bool ret = false;

			foreach (KeyDownEventDelegate del in KeyDownEvent.GetInvocationList())
			{
				try
				{
					ret = del(ckey);
				}
				catch (Exception e)
				{
					ChiConsole.WriteError("Error calling console key handler", e);
				}

				if (ret == true)
					break;
			}

			return ret;
		}

	}
}
