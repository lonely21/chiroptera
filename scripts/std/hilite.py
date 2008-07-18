import BatMud.BatClientBase
from batclient import *

def hiliteaction(msg, match, data):
	fg = data[0]
	fullline = data[1]
	if fullline:
		msg.SetText(colorize(msg.Text, fg))
	else:
		while match.Success:
			msg.Colorize(match.Index, match.Length, fg, BatMud.BatClientBase.Color.Empty)
			match = match.NextMatch()

def hilitecmd(input):
	def usage():
		print "usage: /hilite [-c <color>] [-b <color>] [-n name] [-i] [-f] <pattern>"
	
	try:
		args, opts = getopts(input, "n:c:b:fi")
	except Exception, err:
		print err
		usage()
		return -1
	
	if len(args) != 1:
		usage()
		return -1
	
	C = BatMud.BatClientBase.Color
	color = C.Empty
	bgcolor = C.Empty
	fullline = False
	ignorecase = True
	name = None
		
	for opt in opts:
		if opt.Key == "n":
			name = opt.Value
		if opt.Key == "c":
			try:
				color = C.FromHtml(opt.Value)
			except:
				print "Error parsing color", opt.Value
				return				
		if opt.Key == "b":
			try:
				bgcolor = C.FromHtml(opt.Value)
			except:
				print "Error parsing color", opt.Value
				return				
		if opt.Key == "f":
			fullline = True
		if opt.Key == "i":
			ignorecase = True

	if color.IsEmpty and bgcolor.IsEmpty:
		style = BatMud.BatClientBase.TextStyle(BatMud.BatClientBase.TextStyleFlags.HighIntensity)
		hilite = BatMud.BatClientBase.Hilite(args[0], ignorecase, style, fullline)
	else:
		hilite = BatMud.BatClientBase.Hilite(args[0], ignorecase, color, bgcolor, fullline)
	HiliteMgr.AddHilite(hilite)
	return 0
	
addcommand("hilite", hilitecmd, "hilite a pattern",
"""usage: /hilite [-c <color>] [-b <color>] [-n name] [-i] [-f] <pattern>

Hilites pattern with specified color. Options:
	-n <name>               Name
	-c <color>              Text color
	-b <color>              Background color
	-f                      Hilite the whole line insted of just the pattern
	-i                      Ignore case
""")

testmode = 1
if testmode and BatMud.BatClientBase.PythonInterface.IsDebug():
	from batclient import *

	#write("\x1b[38;5;41mkala\x1b[0mkissa")
	receive("jee")
	receive("\x1b[7mXX\x1b[1mXX\x1b[38;5;41mka\nXX\x1b[0mkissa")
	receive("joo")

	#receive(create256())
	
	hilitecmd("kiki")
	hilitecmd("-c green youro")
	hilitecmd("-c yellow Tomba")

	receive("You unzip your tomba kiki kuu and mumbku")
	receive("You unzip your zipper and mumb")
	receive("You unzip youro huku'")
	receive("You unzip tomba zipper tomba mumble 'kaakaa tomba kuu mopo huku'")

