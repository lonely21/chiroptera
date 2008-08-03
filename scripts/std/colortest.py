
from batclient import *

print "running colortest.py"

receive("foo \x1b[1mbold\x1b[0m kissa \x1b[7mreverse\x1b[0m fuuba")

receive("\x1b[32mgreen\x1b[33myellow\x1b[32;45mgreenonpurple")

receive("pla\x1b[0m")

write("joo" + colorize("blueonyellow", "blue", "yellow") + "pajoo")

write("nomoi")

receive("")

write("foo " + colorize("orange", "orange") + " pajoo " + colorize("red", "red") + " plim")

write("jep")

receive("\x1b[34;1m`---------\x1b[0m\x1b[37;1mv\x1b[0m\x1b[34;1m---------\x1b[0m\x1b[37;1m>\x1b[0m\x1b[34;1m\x1b[0m")

receive("\x1b[34;1m`---------\x1b[0m\x1b[37;1mv\x1b[0m")

receive("none\x1b[32mgreen\x1b[1mgreen_bold\x1b[45mgreen_bold_on_purple\x1b[33myellow_bold_on_purple\x1b[0mnone")
