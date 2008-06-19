#print "running triggers script"

import BatMud.BatClientBase
import triggers
from batclient import *

def listcmd(input):
	def usage():
		print "usage: /list"
		
	try:
		args, opts = getopts(input, "")
	except Exception, err:
		print err
		usage()
		return -1
	
	if len(args) != 0:
		usage()
		return -1

	trigs = BatMud.BatClientBase.PythonInterface.TriggerManager.Triggers
	write("Defined triggers:")
	for t in trigs:
		flags = ""
		
		if t.IsFallThrough: 
			flags += "F"
		else:
			flags += "-"
			
		if t.IsGag: 
			flags += "G"
		else:
			flags += "-"
		
		target = "<unknown-target>"

		write("%d: %d %s '%s', '%s' -> %s" % (t.TriggerID, t.Priority, flags, t.TriggerName, t.Regex, target))

	return 0

removecommand("list")
addcommand("list", listcmd, "list triggers", "usage: /list\n\nLists all defined triggers. The listed fields are in order: " 
"trigger ID, trigger priority, trigger flags (F - fallthrough, G - gag), trigger name, trigger pattern, trigger target action")


def deletecmd(input):
	def usage():
		print "usage: /delete [-n] <trigger>"
	
	try:
		args, opts = getopts(input, "n")
	except Exception, err:
		print err
		usage()
		return -1

	if len(args) != 1:
		usage()
		return -1
	
	name = False
	for opt in opts:
		if opt.Key == "n":
			name = True

	if name == False:
		try:
			trigger = int(args[0])
		except:
			print "Trigger id must be a number"
			return -1
	else:
		trigger = args[0]
	
	if triggers.removetrigger(trigger) == False:
		print "Trigger " + args[0] + " not found."
	else:
		print "Trigger " + args[0] + " deleted."

	return 0

removecommand("delete")
addcommand("delete", deletecmd, "delete a trigger", "usage: /delete [-n] <trigger>\n\nDeletes a trigger with ID <trigger>, or if -n flag is given a trigger named <trigger>.")

def triggercmd(input):
	def usage():
		print "usage: /trigger [options] <pattern> -> <action>"
	
	try:
		input, action = input.split(" -> ", 1)
	except:
		usage()
		return -1
		
	if len(action) == 0:
		usage()
		return -1
	
	try:
		args, opts = getopts(input, "p:n:k:gfim:")
	except Exception, err:
		print err
		usage()
		return -1
	
	if len(args) != 1:
		usage()
		return -1

	pattern = args[0]
	mode = "send"
	name = None
	group = None
	priority = 0
	fallthrough = False
	gag = False
	ignorecase = False
	
	for opt in opts:
		if opt.Key == "n":
			name = opt.Value
		if opt.Key == "k":
			group = opt.Value
		if opt.Key == "g":
			gag = True
		if opt.Key == "f":
			fallthrough = True
		if opt.Key == "m":
			mode = opt.Value
		if opt.Key == "p":
			priority = int(opt.Value)
		if opt.Key == "i":
			ignorecase = True

	tm = BatMud.BatClientBase.PythonInterface.TriggerManager

	if mode == "send":
		type = BatMud.BatClientBase.TriggerType.Send
	elif mode == "replace":
		type = BatMud.BatClientBase.TriggerType.Replace
	elif mode == "script":
		type = BatMud.BatClientBase.TriggerType.Script
	else:
		print "Unknown mode " + mode
		return -1
		
	t = BatMud.BatClientBase.ScriptedTrigger(pattern, ignorecase, type, action)
	
	tm.AddTrigger(t)
	
	return 0
	

removecommand("trigger")
addcommand("trigger", triggercmd, "define a new trigger", "usage: /trigger [options] <pattern> -> <action>\n\n"
"""Defines a new trigger. Options are:
	-n <name>		Trigger name
	-k <group>		Trigger group
	-g				Gag
	-f				Fallthrough
	-m <mode>		Mode
	-p <priority>	Priority
	-i				Ignore case

Modes
-----
send - When the trigger fires, the string in <action> is evaluated and send to MUD.
replace - when the trigger fires, <action> is evaluated and the matched pattern is replaced with the <action>.
script - when the trigger fires, <action> is executed as python script.

Evaluation
----------
When a trigger fires its action is evaluated and the following replacements are made:
%<n> - n'th matched regexp group, or if n is zero, the whole match. n can be from 0 to 9.
%L or %R - The rest of the line left of the match, or right of the match.
%{<code>} - code is evaluated as python script and its return value is used as the replacement.

Examples
--------
/trigger -p 0 -f -n scripttest -m script "(\w*) arrives from (\w*)" -> print "player: '%s' arrives from direction: '%s'" % (%1.upper(), %2)
/trigger -f -n replacetest -m replace "^(\w*) attacks you!$" -> *** %{batclient.colorize(%0.upper(), "red")} ***
/trigger -f -n sendtest -m send "(\w*) is dead, RIP" -> dig grave
""")
