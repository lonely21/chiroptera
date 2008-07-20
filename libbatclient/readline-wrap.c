
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/ioctl.h>

#include <readline/readline.h>
#include <readline/history.h>

static int saved_point, saved_end;

void mono_rl_set_catch_signals(int enable)
{
	rl_catch_signals = enable;
	rl_catch_sigwinch = enable;
}

int mono_rl_get_window_size(int *w, int *h)
{
	struct winsize size;

	if(ioctl(STDIN_FILENO, TIOCGWINSZ, &size) == 0)
	{
		*w = size.ws_col;
		*h = size.ws_row;
		return 1;
	}
	else
	{
		*w = 0;
		*h = 0;
		return 0;
	}
}

const char *mono_rl_get_line()
{
	return rl_line_buffer;
}

void mono_rl_set_line(const char *str)
{
	rl_replace_line(str, 0);
}

void mono_rl_set_event_hook(int (*fn)())
{
	rl_event_hook = fn;
}

void mono_rl_save_and_clear()
{
	rl_save_prompt();
	rl_clear_message();

	saved_point = rl_point;
	saved_end = rl_end;
	rl_point = 0;
	rl_end = 0;

	rl_redisplay();
}

void mono_rl_restore()
{
	rl_restore_prompt();
	rl_point = saved_point;
	rl_end = saved_end;
	rl_redisplay();
}

int mono_history_get_length()
{
	return history_length;
}

const char *mono_history_get(int offset)
{
	HIST_ENTRY* entry = history_get(offset);
	
	return entry->line;
}

