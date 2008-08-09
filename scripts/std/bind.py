import Chiroptera.Base
from chiroptera import *

def bindcmd(input):
	def usage():
		print "usage: /bind [options] <key> -> action"
	
	try:
		input, action = input.split(" -> ", 1)
	except:
		usage()
		return -1
		
	if len(action) == 0:
		usage()
		return -1
	
	try:
		args, opts = getopts(input, "m:sca")
	except Exception, err:
		print err
		usage()
		return -1
	
	if len(args) != 1:
		usage()
		return -1
	
	mode = "send"
	shift = False
	ctrl = False
	alt = False
	
	for opt in opts:
		if opt.Key == "m":
			mode = opt.Value
		if opt.Key == "s":
			shift = True
		if opt.Key == "c":
			ctrl = True
		if opt.Key == "a":
			alt = True

	try:
		key = System.Enum.Parse(System.Windows.Forms.Keys, args[0])
	except Exception:
		print "Key " + args[0] + " was not found."
		return -1

	if shift:
		key |= System.Windows.Forms.Keys.Shift
	if ctrl:
		key |= System.Windows.Forms.Keys.Control
	if alt:
		key |= System.Windows.Forms.Keys.Alt
		
	if mode == "send":
		type = Chiroptera.Base.KeyBindingType.Send
	elif mode == "script":
		type = Chiroptera.Base.KeyBindingType.Script
	else:
		print "Unknown mode " + mode
		return -1
		
	KeyMgr.RemoveBinding(key)
	binding = Chiroptera.Base.ScriptedKeyBinding(key, type, action, False)
	KeyMgr.AddBinding(binding)

	return 0
	

addcommand("bind", bindcmd, "bind a key",
"""usage: /bind [options] <key> -> <action>

Bind a key to an action. Options:
	-m <mode>		Mode. send or script
	-s				Shift modifier
	-c				Control modifier
	-a				Alt modifier
""")


def unbindcmd(input):
	def usage():
		print "usage: /unbind [options] <key>"
	
	try:
		args, opts = getopts(input, "sca")
	except Exception, err:
		print err
		usage()
		return -1
	
	if len(args) != 1:
		usage()
		return -1
	
	shift = False
	ctrl = False
	alt = False
	
	for opt in opts:
		if opt.Key == "s":
			shift = True
		if opt.Key == "c":
			ctrl = True
		if opt.Key == "a":
			alt = True

	try:
		key = System.Enum.Parse(System.ConsoleKey, args[0])
	except Exception:
		print "Key " + args[0] + " was not found."
		return -1
		
	mods = System.ConsoleModifiers()
	if shift:
		mods |= System.ConsoleModifiers.Shift
	if ctrl:
		mods |= System.ConsoleModifiers.Control
	if alt:
		mods |= System.ConsoleModifiers.Alt

	KeyMgr.UnbindKey(key, mods)

	return 0
	

addcommand("unbind", unbindcmd, "unbind a key",
"""usage: /unbind [options] <key>

Unbind a key that was bound earlier. Options:
	-s				Shift modifier
	-c				Control modifier
	-a				Alt modifier
""")

