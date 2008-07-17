
from batclient import *

def create256():
	s = ""

	# first the system ones:
	s += "System colors:\n"
	for color in range(8):
		s += "\x1b[48;5;%dm  " % color

	s += "\x1b[0m\n"
	
	for color in range(8, 16):
		s += "\x1b[48;5;%dm  " % color
	
	s += "\x1b[0m\n"

	# now the color cube
	s += "Color cube, 6x6x6:\n"
	for green in range(6):
		for red in range(6):
			for blue in range(6):
				color = 16 + (red * 36) + (green * 6) + blue
				s += "\x1b[48;5;%dm  " % color
			s += "\x1b[0m "
		s += "\n"

	# now the grayscale ramp
	s += "Grayscale ramp:\n"
	for color in range(232, 256):
		s += "\x1b[48;5;%dm  " % color
	
	s += "\x1b[0m\n"
	
	return s

print "running colortest.py"

receive("\x1b[1mkala\x1b[0mkissa")

receive("\x1b[32mkala\x1b[33mkissa\x1b[32;45mkoka")


write("joo" + colorize("KALA", "red", "blue") + "pajoo")

write("nomoi")
