using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public static class GNUReadLine
{
	public delegate void LineHandlerDelegate(IntPtr line);
	
	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static int rl_set_prompt(string str);

	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void stifle_history(int max);

	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static int read_history(string filename);

	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static int write_history(string filename);
	
	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void rl_redisplay();
	
	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void rl_forced_update_display();

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
	
	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void rl_clear_signals();
	
	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void rl_reset_after_signal();
	
	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void rl_cleanup_after_signal();
	
	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void rl_clear_message();
	
	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void rl_on_new_line();
	
	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void rl_initialize();

	[DllImport("libreadline", CallingConvention = CallingConvention.Cdecl)]
	public extern static void rl_prep_terminal(int meta);
	
	[DllImport("libbatclient", CallingConvention = CallingConvention.Cdecl)]
	public extern static void mono_rl_set_catch_signals(bool enable);
	
	[DllImport("libbatclient", CallingConvention = CallingConvention.Cdecl)]
	public extern static bool mono_rl_get_window_size(out int width, out int height);

	[DllImport("libbatclient", CallingConvention = CallingConvention.Cdecl)]
	public extern static void mono_rl_save_and_clear();

	[DllImport("libbatclient", CallingConvention = CallingConvention.Cdecl)]
	public extern static void mono_rl_restore();

	[DllImport("libbatclient", CallingConvention = CallingConvention.Cdecl)]
	public extern static IntPtr mono_rl_get_line();

	[DllImport("libbatclient", CallingConvention = CallingConvention.Cdecl)]
	public extern static void mono_rl_set_line(string str);
}

