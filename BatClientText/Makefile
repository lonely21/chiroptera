
SOURCES=\
	ClientCore.cs \
	GNUReadLine.cs \
	Program.cs \
	SynchronizedInvoke.cs \
	TermInfo.cs \
	TextConsole.cs \
	Properties/AssemblyInfo.cs

BatClientBase=../BatClientBase/bin/mono/BatClientBase.dll
IronPython=../IronPython/IronPython.dll


TARGET_DIR=bin/mono
TARGET=$(TARGET_DIR)/BatClientText.exe
TARGET_SO=$(TARGET_DIR)/libreadline-wrap.so

all: mkdirs $(TARGET_SO) $(TARGET)

mkdirs:
	mkdir -p $(TARGET_DIR)

$(TARGET_SO): readline-wrap.c
	gcc -g -fPIC -shared readline-wrap.c -o $(TARGET_SO) -l readline

$(TARGET): $(SOURCES)
	gmcs -debug -out:$(TARGET) $(SOURCES) -r:$(BatClientBase) -r:$(IronPython) -r:Mono.Posix

install: all
	mkdir -p ../bin
	cp -u $(TARGET_DIR)/* ../bin/
	cp -u ../IronPython/*.dll ../bin/
	cp -u bat ../bin/
	chmod a+x ../bin/bat

clean:
	rm -rf $(TARGET_DIR)
