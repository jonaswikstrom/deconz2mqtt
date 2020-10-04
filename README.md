# deconz2mqtt
This is a bridge between deCONZ and a MQTT broker. deCONZ is a software that communicates with ConBee/RaspBee Zigbee gateways and exposes Zigbee devices that are connected to the gateway. 
- [Conbee](https://phoscon.de/en/conbee2)
- [deCONZ websocket API](https://dresden-elektronik.github.io/deconz-rest-doc/websocket/).

## Docker image
The application is based upon Microsoft .NET Core 3.1, fully able to install and run on Windows/Mac/Linux but the main focus should be to run it as a Docker Container. Two images exists on Docker Hub:
- deconz2mqtt_x86 (Tested and run on Intel NUC running Ubuntu 18.04.4 LTS (Bionic Beaver))
- deconz2mqtt_arm32 (Tested and run on Rpi Model 3b+ running Raspian Buster)

## Docker compose
The following docker compose can be used. Pay attention to the volume mount for the appsettings file. Best way is to create the source path e.g. */home/pi/docker/deconz2mqtt/* first and then create the file *appsettings.json* within that catalogue.

```
deconz2mqtt:
    image: jonwik/deconz2mqtt_arm32:latest
    container_name: deconz2mqtt
    restart: unless-stopped
    volumes:
      - /home/pi/docker/deconz2mqtt/appsettings.json:/app/appsettings.json
```

## Acquire API key
Find the url and port number to the phoscon web application. The port number is by default *8090* . Open the web application and navigate to: Gateway / Extended

Click on: Connect App

Execute the following command within the next 60 secounds: ```curl -H "Content-Type: application/json" -X POST -d '{"devicetype": "deconz2mqtt"}' http://<host>:<hostport>/api```

the given *username* in the JSON response can be used as API key. 

## Get the websocket port
By default the port number is *4433* but can be found defined in *websocketport* using the command:

``` curl http://<host>/api/<apikey>/config ```

## appsettings.json
The appsettings file is divided into thre parts Deconz, Mqtt and Mappings:

### Deconz
This is the settings for deConz:

| Name | Mandatory | Comment |
|-|-|-|
| ApiKey | yes | The key used for calls towards the deConz API |
| HostName | yes | The name/ip to the deConz host e.g. *10.0.0.6* |
| Port | yes | The port number to the deConz host, usually *8090* |
| WebSocketPort | yes | The port number to the deConz web socket, usually 4433 |

### Mqtt
This is the settings for the MQTT host

| Name | Mandatory | Comment |
|-|-|-|
| HostName | yes | The name/ip to the MQTT host e.g. *192.168.1.5* |
| UserName | yes | The user name to the MQTT host |
| Password | yes | The password to the MQTT host |
| TopicRoot | yes | The topic root used for all publications to MQTT |

### Mappings
This is where you defines which sensors and lights to handle and how these should be configured for requests and responses from MQTT. The configured entities should be mapped using the structure defined by deConz. 

Browse to ```http://<host>:<hostport>/api/<apikey>``` where you can get a JSON result which defines all your configured entities. A tip is to use a Chrome JSON Viewer plugIn for a better overview [JSONView] (https://shorturl.at/gjpCX)

The mappings is divided into *sensors* and *lights* where both entities share the functionality for state reading. The system monitors the payload given for each entity and publishes the defined part of this on a specific MQTT topic. The *Lights* entities subscribes to a specific MQTT command topic and performs a *PUT* statement towards the deConz-API with defined payload.

#### Sensors
Following properties must and can be used for a sensor

| Name | Mandatory | Comment |
|-|-|-|
| Id | yes | Defines the id of the sensor to monitor  |
| StatePath | yes | Defines the path to the part of the sensor payload to forward to MQTT. A complex JSON-path query can be used as a query. |
| StateTopic | yes | The topic used when publishing the payload to MQTT. This value is combined with the *TopicRoot* defined in the *MQTT* section. |
| StateUpdateInterval | no | A timespan used to periodically read the state value e.g. "00:01:00" reads the value every minute. |
| Divisor | no | Divides the payload result by the given number. If the payload is not a numeric value there will be a log warning and the original payload  will be published |
| Decimals | no | Rounds the payload value to use the given number of decimals. If the payload is not a numeric value there will be a log warning and the original payload  will be published |
| IgnoreStateUpdateAtStartup | no | Do not perform a manual state update at startup. |
| PollOnly | no | WebSocket events are ignored. |

#### Lights
Following properties must and can be used for a light entity. 

| Name | Mandatory | Comment |
|-|-|-|
| Id | yes | Defines the id of the sensor to monitor  |
| StatePath | yes | Defines the path to the part of the sensor payload to forward to MQTT. A complex JSON-path query can be used as a query. |
| StateTopic | yes | The topic used when publishing the payload to MQTT. This value is combined with the *TopicRoot* defined in the *MQTT* section. |
| StateUpdateInterval | no | A timespan used to periodically read the state value e.g. "00:01:00" reads the value every minute. |
| Divisor | no | Divides the payload result by the given number. If the payload is not a numeric value there will be a log warning and the original payload  will be published |
| Decimals | no | Rounds the payload value to use the given number of decimals. If the payload is not a numeric value there will be a log warning and the original payload  will be published |
| IgnoreStateUpdateAtStartup | no | Do not perform a manual state update at startup. |
| CommandTopic | yes | Defines the topic used for subscription.   OBSERVE! - The payload is used to control the light and should be simple, e.g. true/false/ON/OFF, NOT a JSON.  - The last part of the *StatePath* is used to create the JSON paylod for the PUT command against deConz-API e.g. *state.on* with a true payload, will result in the JSON ```{"on":true}``` |
| PollOnly | no | WebSocket events are ignored. |

### Example
Here is an example of a *appsettings.json* file:
```
{
  "Deconz": {
    "ApiKey": "ABE9917F63",
    "HostName": "10.0.0.6",
    "Port": 8090,
    "WebSocketPort": 4433
  },
  "Mqtt": {
    "HostName": "192.168.1.5",
    "Username": "pi",
    "Password": "Raspberry",
    "TopicRoot": "deconz"
  },
  "Mappings": {
    "Sensors": [
      {
        "Id": "2",
        "StatePath": "state.temperature",
        "StateTopic": "cellarroom/temperature",
        "StateUpdateInterval": "00:05:00",
        "Divisor" : 100,
        "Decimals": 1,
        "IgnoreStateUpdateAtStartup": true
      },
      {
        "Id": "2",
        "StatePath": "state.humidity",
        "StateTopic": "cellarroom/humidity",
        "StateUpdateInterval": "00:05:00",
        "Divisor" : 100,
        "Decimals": 1,
        "PollOnly": true
      },
      {
        "Id": "2",
        "StatePath": "config.battery",
        "StateTopic": "cellarroom/battery"
      }
    ],
    "Lights": [
      {
        "Name": "Lightbulb",
        "Id": "12",
        "StatePath": "state.on",
        "StateTopic": "outsidelamp/on",
        "CommandTopic": "outsidelamp/on/cmnd"
      }
    ]
  }
}
```
