
from batclient import *

print "running colortest.py"

receive("\x1b[1mkala\x1b[0mkissa")

receive("\x1b[32mkala\x1b[33mkissa\x1b[32;45mkoka")

receive("pla\x1b[0m")

write("joo" + colorize("KALA", "orange", "green") + "pajoo")

write("nomoi")

