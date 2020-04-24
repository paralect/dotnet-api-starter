#!/bin/bash

if [ "$#" -lt 5 ]; then
    echo "You must pass host, port, database name, login and password"
	exit 2
fi

MONGO_HOST=$1
MONGO_PORT=$2
MONGO_DATABASE=$3
MONGO_LOGIN=$4
MONGO_PASSWORD=$5
MONGO_INITIATE_SETTINGS="{ _id: 'ship-rs', members: [ { _id: 0, host: 'mongo:27017' } ] }"

function getStatus {
	local response=$(mongo $MONGO_HOST:$MONGO_PORT --authenticationDatabase "$MONGO_DATABASE" -u "$MONGO_LOGIN" -p "$MONGO_PASSWORD" --eval "rs.status();")
	
	echo "$response" | grep -Po '"ok".*\K[0-1]'
}

function initiate {
	mongo $MONGO_HOST:$MONGO_PORT --authenticationDatabase "$MONGO_DATABASE" -u "$MONGO_LOGIN" -p "$MONGO_PASSWORD" --eval "rs.initiate($MONGO_INITIATE_SETTINGS);"
}

function waitForMongo {
	until nc -z $MONGO_HOST $MONGO_PORT
	do
		echo "Waiting for Mongo ($MONGO_HOST:$MONGO_PORT) to start..."
		sleep 0.5
	done
	
	echo "Mongo is up"
}

function waitForOkStatus {
	local status=$(getStatus)
	
	until [ $status -eq 1 ]
	do
		echo "Waiting for OK status"
		sleep 0.5
		status=$(getStatus)
	done
	
	echo "Mongo status is OK"
}