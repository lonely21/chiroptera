using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class GNUReadLine
{
	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	extern static int rl_set_prompt(string str);

	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	extern static void stifle_history(int max);

	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void rl_redisplay();

	public delegate void LineHandlerDelegate(string line);
	
	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void rl_callback_handler_install(string prompt, LineHandlerDelegate handler);

	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void rl_callback_handler_remove();

	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void rl_callback_read_char();

	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void add_history(string str);

	public delegate int CommandFuncDelegate(int a, int b);

	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static CommandFuncDelegate rl_named_function(string name);
	
	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static int rl_bind_key(int key, CommandFuncDelegate fun);

	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static int rl_bind_keyseq(string keyseq, CommandFuncDelegate func);

	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static int rl_reset_line_state();

	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static int rl_set_signals();

	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void rl_resize_terminal();
	
	static GNUReadLine()
	{
		stifle_history(100);
		rl_set_signals();
	}
	
	static public void SetPrompt(string prompt)
	{
		rl_set_prompt(prompt);
		rl_redisplay();
	}
}

