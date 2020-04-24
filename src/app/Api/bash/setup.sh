#!/bin/bash

DIR="${BASH_SOURCE%/*}"
. "$DIR/mongo-tools.sh" $1 $2 $3 $4 $5

waitForMongo
status=$(getStatus)

if [ $status -eq 0 ]
then
	echo "Setting up mongo replication"
	
	initiate
	waitForOkStatus
	
	echo "Replication done"
else
	echo "Mongo is already replicated"
fi

dotnet Api.dll