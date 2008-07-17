# a test that converts spellnames to readable versions

print "running spellnames.py"

from batclient import *
from triggers import *

spells = (
# name                  transl       type       damtype
( "huku mopo huku",		"rain",		"utility",	None ),
( "ful",				"light",	"utility",	None ),
( "kissa",				"kala",		"utility",	None ),
)

removetriggergroup("spellnames")

def handlespell(msg, match, spelldata):
	name = "'" + spelldata[1] + "'"
	msg.SetText(match.Result("$`" + colorize(name, "red")))

for spelldata in spells:
	regexp = "'" + spelldata[0] + "'"
	addtrigger(regexp, handlespell, userdata=spelldata, triggergroup="spellnames")

testmode = 1
if testmode:
	receive("You unzip your zipper and mumbku 'ful'")
	receive("You unzip your zipper and mumble 'huku mopo huku'")
	receive("You unzip youro huku'")
	receive("You unzip your zipper and mumble 'huku mopo huku'")

