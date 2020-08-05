# deconz2mqtt
This is a bridge between deCONZ and a MQTT broker. deCONZ is a software that communicates with ConBee/RaspBee Zigbee gateways and exposes Zigbee devices that are connected to the gateway. 
- [Conbee](https://phoscon.de/en/conbee2)
- [deCONZ websocket API](https://dresden-elektronik.github.io/deconz-rest-doc/websocket/).

## Docker image
The application is based upon Microsoft .NET Core 3.1, fully able to install and run on Windows/Mac/Linux but the main focus should be to run it as a Docker Container. Two images exists on Docker Hub:
- deconz2mqtt_x86 (Tested and run on Intel NUC running Ubuntu 18.04.4 LTS (Bionic Beaver))
- deconz2mqtt_arm32 (Tested and run on Rpi Model 3b+ running Raspian Buster)

   
