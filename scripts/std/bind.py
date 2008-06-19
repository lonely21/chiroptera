import BatMud.BatClientBase
from batclient import *

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

	km = BatMud.BatClientBase.PythonInterface.KeyManager

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
		type = BatMud.BatClientBase.KeyBindingType.Send
	elif mode == "script":
		type = BatMud.BatClientBase.KeyBindingType.Script
	else:
		print "Unknown mode " + mode
		return -1
		
	km.RemoveBinding(key)
	binding = BatMud.BatClientBase.ScriptedKeyBinding(key, type, action, False)
	km.AddBinding(binding)

	return 0
	

removecommand("bind")
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

	km = BatMud.BatClientBase.PythonInterface.KeyManager

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

	km.UnbindKey(key, mods)

	return 0
	

removecommand("unbind")
addcommand("unbind", unbindcmd, "unbind a key",
"""usage: /unbind [options] <key>

Unbind a key that was bound earlier. Options:
	-s				Shift modifier
	-c				Control modifier
	-a				Alt modifier
""")

