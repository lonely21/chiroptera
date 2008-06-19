
import BatMud.BatClientBase
from batclient import *

def echocmd(input):
	write(input)
	return 0

removecommand("echo")
addcommand("echo", echocmd, "Write text to console", "")

def sendcmd(input):
	send(input)
	return 0

removecommand("send")
addcommand("send", sendcmd, "Send text to mud", "")

def receivecmd(input):
	receive(input)
	return 0

removecommand("receive")
addcommand("receive", receivecmd, "Receive text, as it would have came from the mud", "")
