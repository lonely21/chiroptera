# Generic python-like wrappers to BatClient

import BatMud.BatClientBase
import System.Drawing

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
	BatMud.BatClientBase.PythonInterface.Network.SendLine(str)
	
def write(str, *args):
	BatMud.BatClientBase.PythonInterface.Console.WriteLine(str, *args)
	
def receive(str):
	BatMud.BatClientBase.PythonInterface.Network.ReceiveLine(str)

def isdebug():
	BatMud.BatClientBase.PythonInterface.IsDebug()

def run(file):
	BatMud.BatClientBase.PythonInterface.RunScript(file)

def addcommand(cmd, action, help, longhelp):
	BatMud.BatClientBase.PythonInterface.CommandManager.AddCommand(cmd, action, help, longhelp)

def removecommand(cmd):
	BatMud.BatClientBase.PythonInterface.CommandManager.RemoveCommand(cmd)

def getopts(input, optstring):
	args, opts = BatMud.BatClientBase.PythonInterface.CommandManager.GetOpts(input, optstring)
	return (args, opts)
