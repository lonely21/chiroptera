import Chiroptera.Base

def addtrigger(regex, action, triggername=None, triggergroup=None, userdata=None, priority=0, fallthrough=False, gag=False, ignorecase=False):
	"""Add a new trigger.
	
regex - Regular expression for matching the trigger.
        http://msdn2.microsoft.com/en-us/library/az24scfc.aspx
action - Function to be called when the trigger goes off OR code to be 
         run as string.
         actionfunc(msg, match, userdata)
             msg - Chiroptera.Base.ColorMessage
             match - System.Text.RegularExpressions.Match
                     http://msdn2.microsoft.com/en-us/library/system.text.regularexpressions.match.aspx
             userdata - object
triggername - Name of the trigger. Default None.
triggergroup - Name of the trigger group to which this trigger is added.
               Default None.
userdata - Data to be passed to the action function. Default None.
fallthrough - If True, continue searching matching triggers after this
              one. Default False.
"""
	t = Chiroptera.Base.Trigger(regex, action, userdata, triggername, triggergroup, priority, fallthrough, gag, ignorecase)
	Chiroptera.Base.PythonInterface.TriggerManager.AddTrigger(t)

def gettrigger(arg):
	"""Returns Trigger instance.
	
	arg - trigger id or trigger name.
	"""
	
	if isinstance(arg, int):
		return Chiroptera.Base.PythonInterface.TriggerManager.GetTrigger(arg)
	elif isinstance(arg, str):
		return Chiroptera.Base.PythonInterface.TriggerManager.GetTrigger(arg)
	else:
		return None

def removetrigger(arg):
	"""Remove trigger.
	
	arg - Trigger instance, trigger id or trigger name.
	"""
	
	t = None
	if isinstance(arg, int):
		t = Chiroptera.Base.PythonInterface.TriggerManager.GetTrigger(arg)
	elif isinstance(arg, str):
		t = Chiroptera.Base.PythonInterface.TriggerManager.GetTrigger(arg)
	elif isinstance(arg, Chiroptera.Base.Trigger):
		t = arg
	else:
		return False
	
	if t == None:
		return False
		
	return Chiroptera.Base.PythonInterface.TriggerManager.RemoveTrigger(t)	

def removetriggergroup(groupname):
	Chiroptera.Base.PythonInterface.TriggerManager.RemoveTriggerGroup(groupname)


