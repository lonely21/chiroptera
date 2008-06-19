
all:
	make -C BatClientBase
	make -C BatClientText

install:
	make -C BatClientBase install
	make -C BatClientText install
	cp -ru scripts/* bin/
	echo "all:" > bin/Makefile
	echo "	make -C .. install" >> bin/Makefile
	echo "run: all" >> bin/Makefile
	echo "	./bat" >> bin/Makefile

clean:
	make -C BatClientBase clean
	make -C BatClientText clean

distclean: clean
	rm -rf bin
