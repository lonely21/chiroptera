#!/bin/bash

cd BatClientText/bin/Debug

exec -a "BatClient" /usr/bin/mono --debug ./BatClientText.exe "$@"

