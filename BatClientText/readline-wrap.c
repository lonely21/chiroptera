
#include <stdio.h>
#include <stdlib.h>

#include <readline/readline.h>
#include <readline/history.h>

static char mono_line_buffer[1024];
static int saved_point, saved_end;

const char* mono_rl_readline(char* prompt)
{
	char* str = readline(prompt);

	if(str == 0)
	{
		mono_line_buffer[0] = 0;
		return mono_line_buffer;
	}

	if(strlen(str) > 0)
		add_history(str);

	strcpy(mono_line_buffer, str);
	free(str);

	return mono_line_buffer;
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
