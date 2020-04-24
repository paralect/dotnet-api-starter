API_HOST=$1
API_PORT=$2

until nc -z $API_HOST $API_PORT
do
	echo "Waiting for api ($API_HOST:$API_PORT) to start..."
	sleep 0.5
done

echo "Api is up"

dotnet SignalR.dll