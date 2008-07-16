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
	if not isinstance(fg, System.Drawing.Color):
		if fg != None:
			fg = System.Drawing.ColorTranslator.FromHtml(fg)
		else:
			fg = System.Drawing.Color.Empty
		
	if not isinstance(bg, System.Drawing.Color):
		if bg != None:
			bg = System.Drawing.ColorTranslator.FromHtml(bg)
		else:
			bg = System.Drawing.Color.Empty
		
	str = BatMud.BatClientBase.ControlCodes.ColorizeString(str, fg, bg)
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
