
from batclient import *

print "running colortest.py"

receive("foo \x1b[1mbold\x1b[0m kissa \x1b[7mreverse\x1b[0m fuuba")

receive("\x1b[32mgreen\x1b[33myellow\x1b[32;45mgreenonpurple")

receive("pla\x1b[0m")

write("joo" + colorize("blueonyellow", "blue", "yellow") + "pajoo")

write("nomoi")

write("foo " + colorize("orange", "orange") + " pajoo " + colorize("red", "red") + " plim")

write("jep")
