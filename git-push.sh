#!/bin/bash

function test()
{
	"$@"
	local s=$?
	[[ $s -ne 0 ]] && echo "Error on: $@" && exit $s
}
test git add .

test git commit

test git push git@github.com:gurudigitalsolutions/funguy.git master

