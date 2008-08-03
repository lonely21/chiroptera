from batcore import *
from batclient import *

def echocmd(input):
	write(input)
	return 0

addcommand("echo", echocmd, "Write text to console", "")

def sendcmd(input):
	if isconnected():
		send(input)
	else:
		write("/send failed: not connected")
	return 0

addcommand("send", sendcmd, "Send text to mud", "")

def receivecmd(input):
	receive(input)
	return 0

addcommand("receive", receivecmd, "Receive text, as it would have came from the mud", "")

def connectcmd(input):
	if Net.IsConnected:
		Net.Disconnect()

	Net.Connect("bat.org", 23)

	return 0

addcommand("connect", connectcmd, "Connect to MUD", "usage: /connect [host [port]]\n\nConnected to a MUD. Defaults to batmud.bat.org.")

def promptcmd(input):
	Console.Prompt = input
	return 0

addcommand("prompt", promptcmd, "Connect to MUD", "usage: /connect [host [port]]\n\nConnected to a MUD. Defaults to batmud.bat.org.")


