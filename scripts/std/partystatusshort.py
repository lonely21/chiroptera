# a test to catch party status output and reformat it

print "running partystatusshort.py"

from chiroptera import *
from triggers import *

handling_pss = False
members = {}

# Trigger handling functions

def handle_pss_start(msg, match, data):
	global handling_pss
	global members
	handling_pss = True
	members = {}
#	print "pss_start triggered"

def handle_pss_end(msg, match, data):
	global handling_pss
	handling_pss = False
#	print "pss_end triggered"
	print_members()

def handle_pss_line(msg, match, data):
	global handling_pss
	if handling_pss == False:
		return
#	print "pss_line triggered"
	handle_member(match.Groups["ppx"], match.Groups["ppy"], match.Groups["name"], 
		int(match.Groups["hp"].ToString()), int(match.Groups["sp"].ToString()))
	return True		# gag the line

def handle_member(ppx, ppy, name, hp, sp):
	global members
	members[str(ppx) + str(ppy)] = (name, hp, sp)

def print_members():
	global members
	for y in xrange(1,4):
		line = ""
		for x in xrange(1,4):
			k = str(x) + str(y)
			if members.has_key(k):
				hp = colorize("%4d" % members[k][1], "red")
				sp = colorize("%4d" % members[k][2], "blue")
				line += "| %-13s %4s %4s " % (members[k][0], hp, sp)
			else:
				line += "| %-13s %4s %4s " % ("", "", "")
		line += "|"
		write(line)
				

# Add triggers

removetriggergroup("pss")

addtrigger("^,-----------------------------------------------------------------------------.$",
	handle_pss_start, triggergroup="pss")

addtrigger("^`-----------------------------------------------------------------------------'$",
	handle_pss_end, triggergroup="pss")

addtrigger("""(?x)						# IgnoreWhiteSpace option
	^\\| \s+							# line start
	(?<ppy>\d)\.(?<ppx>\d) \s+			# party place
	(?<name>\w+) \s+					# name
	(?<status>\w+) \s+					# status
	(?<hp>\d+) \(\s* (?<maxhp>\d+) \) \s+	# hp/maxhp
	(?<sp>\d+) \(\s* (?<maxsp>\d+) \) \s+	# sp/maxsp
	(?<ep>\d+) \(\s* (?<maxep>\d+) \) \s+	# ep/maxep
	\\| \s+ (?<level>\d+) \s+ \\| \s+	# level
	(?<exp>\d+) \s+						# exp
	\\|									#line end
	""", handle_pss_line, triggergroup="pss")


# testmode

testmode = 1
if testmode != 0:
	receive(",-----------------------------------------------------------------------------.")
	receive("| 1.2   Tomba         ldr  948( 948)  100(  74) 170(328) | 600 |           0  |")
	receive("| 2.2   Tomba2        fol  948( 948)  100(  74) 170(328) | 600 |           0  |")
	receive("`-----------------------------------------------------------------------------'")


#,-----------------------------------------------------------------------------.
#| 1.2   Tomba         ldr  948( 948)  100(  74) 170(328) | 600 |           0  |
#`-----------------------------------------------------------------------------'

#,-----------------------------------------------------------------------------.
#|              H    S     | Dazzt        H 540S1225 |              H    S     |
#|              H    S     |              H    S     |              H    S     |
#|              H    S     |              H    S     |              H    S     |
#`---[ DIFF HP:      0 ]-------[ DIFF SP:      0 ]-------[ DIFF EP:      0 ]---'
