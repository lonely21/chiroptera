
SOURCES=\
	Ansi.cs \
	BaseServicesDispatcher.cs \
	BatConsole.cs \
	BatPythonStream.cs \
	ClientSocket.cs \
	ColorMessage.cs \
	CommandManager.cs \
	ControlCodes.cs \
	Collections.cs \
	HiliteManager.cs \
	INetwork.cs \
	KeyManager.cs \
	PythonInterface.cs \
	Telnet.cs \
	TriggerManager.cs \
	Properties/AssemblyInfo.cs

IronPython=../IronPython/IronPython.dll

TARGET_DIR=bin/mono
TARGET=$(TARGET_DIR)/BatClientBase.dll

all: mkdirs $(TARGET)

mkdirs:
	mkdir -p $(TARGET_DIR)

$(TARGET): $(SOURCES)
	gmcs -debug -target:library -out:$(TARGET) $(SOURCES) -r:System.Drawing -r:$(IronPython) -r:System.Windows.Forms

install: all
	mkdir -p ../bin
	cp -u $(TARGET_DIR)/* ../bin/

clean:
	rm -rf $(TARGET_DIR)
