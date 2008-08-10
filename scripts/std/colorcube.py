
from chiroptera import *

def create256():
	s = ""

	# first the conventional system ones:
	s += "Conventional system colors:\n"
	for color in range(8):
		s += "\x1b[%dm%d " % (color + 30, color)

	s += "\x1b[0m\n"
	
	for color in range(8):
		s += "\x1b[1;%dm%d " % (color + 30, color)
	
	s += "\x1b[0m\n"

	# first the system ones:
	s += "System colors:\n"
	for color in range(8):
		s += "\x1b[48;5;%dm%2d " % (color, color)

	s += "\x1b[0m\n"
	
	for color in range(8, 16):
		s += "\x1b[48;5;%dm%2d " % (color, color)
	
	s += "\x1b[0m\n"

	# now the color cube
	s += "Color cube, 6x6x6:\n"
	for green in range(6):
		for red in range(6):
			for blue in range(6):
				color = 16 + (red * 36) + (green * 6) + blue
				s += "\x1b[48;5;%dm%2d" % (color, (green + 1) * (blue + 1))
			s += "\x1b[0m "
		s += "\n"

	# now the grayscale ramp
	s += "Grayscale ramp:\n"
	i = 0
	for color in range(232, 256):
		s += "\x1b[48;5;%dm%2d " % (color, i)
		i = i + 1
	
	s += "\x1b[0m\n"
	
	return s

receive(create256())

