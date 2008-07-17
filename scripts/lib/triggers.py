import BatMud.BatClientBase

def addtrigger(regex, action, triggername=None, triggergroup=None, userdata=None, priority=0, fallthrough=False, gag=False, ignorecase=False):
	"""Add a new trigger.
	
regex - Regular expression for matching the trigger.
        http://msdn2.microsoft.com/en-us/library/az24scfc.aspx
action - Function to be called when the trigger goes off OR code to be 
         run as string.
         actionfunc(msg, match, userdata)
             msg - BatMud.BatClientBase.ColorMessage
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
	t = BatMud.BatClientBase.Trigger(regex, action, userdata, triggername, triggergroup, priority, fallthrough, gag, ignorecase)
	BatMud.BatClientBase.PythonInterface.TriggerManager.AddTrigger(t)

def gettrigger(arg):
	"""Returns Trigger instance.
	
	arg - trigger id or trigger name.
	"""
	
	if isinstance(arg, int):
		return BatMud.BatClientBase.PythonInterface.TriggerManager.GetTrigger(arg)
	elif isinstance(arg, str):
		return BatMud.BatClientBase.PythonInterface.TriggerManager.GetTrigger(arg)
	else:
		return None

def removetrigger(arg):
	"""Remove trigger.
	
	arg - Trigger instance, trigger id or trigger name.
	"""
	
	t = None
	if isinstance(arg, int):
		t = BatMud.BatClientBase.PythonInterface.TriggerManager.GetTrigger(arg)
	elif isinstance(arg, str):
		t = BatMud.BatClientBase.PythonInterface.TriggerManager.GetTrigger(arg)
	elif isinstance(arg, BatMud.BatClientBase.Trigger):
		t = arg
	else:
		return False
	
	if t == None:
		return False
		
	return BatMud.BatClientBase.PythonInterface.TriggerManager.RemoveTrigger(t)	

def removetriggergroup(groupname):
	BatMud.BatClientBase.PythonInterface.TriggerManager.RemoveTriggerGroup(groupname)


