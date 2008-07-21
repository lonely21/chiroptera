# Generic python-like wrappers to BatClient

import BatMud.BatClientBase
import System.Drawing

# Global helper variables

Bat = BatMud.BatClientBase.PythonInterface
Net = Bat.Network
Console = Bat.Console
CmdMgr = Bat.CommandManager
KeyMgr = Bat.KeyManager
HiliteMgr = Bat.HiliteManager
TriggerMgr = Bat.TriggerManager

def colorize(str, fg, bg=None):
	C = BatMud.BatClientBase.Color
	if not isinstance(fg, C):
		if fg != None:
			fg = C.FromHtml(fg)
		else:
			fg = C.Empty
		
	if not isinstance(bg, C):
		if bg != None:
			bg = C.FromHtml(bg)
		else:
			bg = C.Empty
		
	str = BatMud.BatClientBase.Ansi.ColorizeString(str, fg, bg)
	return str

def send(str):
	Net.SendLine(str)
	
def write(str, *args):
	Console.WriteLine(str, *args)

def isconnected():
	return Net.IsConnected
	
def receive(str):
	Net.ReceiveLine(str)

def isdebug():
	Bat.IsDebug()

def run(file):
	Bat.RunScript(file)

def addcommand(cmd, action, help, longhelp):
	if CmdMgr.HasCommand(cmd):
		print "warning: overriding command '" + cmd + "'"
		removecommand(cmd)
	CmdMgr.AddCommand(cmd, action, help, longhelp)

def removecommand(cmd):
	CmdMgr.RemoveCommand(cmd)

def getopts(input, optstring):
	args, opts = CmdMgr.GetOpts(input, optstring)
	return (args, opts)
