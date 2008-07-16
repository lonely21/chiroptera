import BatMud.BatClientBase
import System.Drawing
from batclient import *

def hiliteaction(msg, match, data):
	fg = data[0]
	fullline = data[1]
	if fullline:
		msg.SetText(colorize(msg.Text, fg))
	else:
		while match.Success:
			msg.Colorize(match.Index, match.Length, fg, System.Drawing.Color.Empty)
			match = match.NextMatch()

def hilitecmd(input):
	def usage():
		print "usage: /hilite [-c <color>] [-n name] [-i] [-f] <pattern>"
	
	try:
		args, opts = getopts(input, "n:c:fi")
	except Exception, err:
		print err
		usage()
		return -1
	
	if len(args) != 1:
		usage()
		return -1
	
	color = System.Drawing.Color.White
	fullline = False
	ignorecase = True
	name = None
		
	for opt in opts:
		if opt.Key == "n":
			name = opt.Value
		if opt.Key == "c":
			try:
				color = System.Drawing.ColorTranslator.FromHtml(opt.Value)
			except:
				print "Error parsing color", opt.Value
				return				
		if opt.Key == "f":
			fullline = True
		if opt.Key == "i":
			ignorecase = True

	hilite = BatMud.BatClientBase.Hilite(args[0], ignorecase, color, System.Drawing.Color.Empty, fullline)
	HiliteMgr.AddHilite(hilite)
	return 0
	
addcommand("hilite", hilitecmd, "hilite a pattern",
"""usage: /hilite [-c <color>] [-n name] [-i] [-f] <pattern>

Hilites pattern with specified color. Options:
	-n <name>		Name
	-c <color>		Hilite color
	-f				Hilite the whole line insted of just the pattern
	-i				Ignore case
""")

testmode = 1
if testmode and BatMud.BatClientBase.PythonInterface.IsDebug():
	from batclient import *
	
	hilitecmd("kiki")
	hilitecmd("-c yellow Tomba")

	receive("You unzip your tomba kiki kuu and mumbku")
	receive("You unzip your zipper and mumb")
	receive("You unzip youro huku'")
	receive("You unzip tomba zipper tomba mumble 'kaakaa tomba kuu mopo huku'")
