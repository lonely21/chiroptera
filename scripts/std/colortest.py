
from batclient import *

print "running colortest.py"

receive("\x1b[1mkala\x1b[0mkissa")

receive("\x1b[32mkala\x1b[33mkissa\x1b[32;45mkoka")


write("joo" + colorize("KALA", "red", "blue") + "pajoo")

write("nomoi")
