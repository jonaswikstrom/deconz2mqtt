# deconz2mqtt
This is a bridge between deCONZ and a MQTT broker. deCONZ is a software that communicates with ConBee/RaspBee Zigbee gateways and exposes Zigbee devices that are connected to the gateway. 
- [Conbee](https://phoscon.de/en/conbee2)
- [deCONZ websocket API](https://dresden-elektronik.github.io/deconz-rest-doc/websocket/).

## Docker image
The application is based upon Microsoft .NET Core 3.1, fully able to install and run on Windows/Mac/Linux but the main focus should be to run it as a Docker Container. Two images exists on Docker Hub:
- deconz2mqtt_x86 (Tested and run on Intel NUC running Ubuntu 18.04.4 LTS (Bionic Beaver))
- deconz2mqtt_arm32 (Tested and run on Rpi Model 3b+ running Raspian Buster)

## Docker compose
The following docker compose can be used. Pay attention to the volume mount for the appsettings file

```
deconz2mqtt:
    image: jonwik/deconz2mqtt_arm32:latest
    container_name: deconz2mqtt
    restart: unless-stopped
    volumes:
      - /home/pi/docker/deconz2mqtt/appsettings.json:/app/appsettings.json
```

## Acquire API key
Open the phoscon web application and navigate to: Gateway / Extended

Click on: Connect App

Execute the following command within the next 60 secounds: ```curl -H "Content-Type: application/json" -X POST -d '{"devicetype": "deconz2mqtt"}' http://<host>/api```

the given username can be used as API key.

## Get the websocket port
By default the port number is *4433* but can be found defined in *websocketport* using the command:

``` curl http://<host>/api/<apikey>/config ```
