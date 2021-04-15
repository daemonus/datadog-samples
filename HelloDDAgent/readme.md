#Datatroll APM & Tracing

This is a sample project attempting to get APM and tracing working inside containers in a .net core app. 
This should somewhat simulate what's happening on Fargate (kind of.. not really XD)

1. Add DD_API_KEY
1. Open `HelloDDAgent.sln`
1. Build the solution
1. Run `docker-build.bat`
1. Run `docker-compose up -d` in the root directory
1. Pull all your hair out 


## Agent commands 

### Check if the agent is up
docker exec -it dd-agent s6-svstat /var/run/s6/services/agent/

### Full status report
docker exec -it dd-agent agent status