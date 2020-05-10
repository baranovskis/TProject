// Base ESP8266
#include <ESP8266WiFi.h>
WiFiClient WIFI_CLIENT;

// MQTT
#include <PubSubClient.h>
PubSubClient MQTT_CLIENT;

// JSON
#include <ArduinoJson.h>

// MQTT Settings
#define MQTT_SERVER "mqtt.fluux.io"
#define MQTT_PORT 1883
#define MQTT_SUBSCRIBE "YOUR MQTT SUBSCRIBE URL"
#define MQTT_PUBLISH "YOUR MQTT PUBLISH URL"

// WiFi Settings
#define WIFI_SSID "YOUR WIFI SDID"
#define WIFI_PASSWORD "YOUR WIFI PASSWORD"

// Board PINS
#define PIN_LIGHT 5 // D1
#define PIN_MOTION 13 // D7

// Previous sensor result
bool PREV_RESULT = false;

// This function runs once on startup
void setup() {
  // Initialize the serial port
  Serial.begin(115200);

  // Configure light sensor pin as an input
  pinMode(PIN_LIGHT, INPUT);
  
  // Configure motion sensor pin as an input
  pinMode(PIN_MOTION, INPUT);
  
  // Attempt to connect to a specific access point
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);

  Serial.println("Attempt to connect to ");
  Serial.print(WIFI_SSID);

  // Keep checking the connection status until it is connected
  while (WiFi.status() != WL_CONNECTED) {
      Serial.print(".");
      delay(500);
  }

  Serial.println("WiFi connected");

  // Print the IP address of your module
  Serial.println("IP address: ");
  Serial.print(WiFi.localIP());
}

// This function connects to the MQTT broker
void reconnect() {
  // Set our MQTT broker address and port
  MQTT_CLIENT.setServer(MQTT_SERVER, MQTT_PORT);
  MQTT_CLIENT.setClient(WIFI_CLIENT);
  MQTT_CLIENT.setCallback(callback);
  
  Serial.println("Attempt to connect to MQTT broker");

  // Loop until we're reconnected
  while (!MQTT_CLIENT.connected()) {
    Serial.print(".");

    // Create a random client ID
    String clientId = "TProject-";
    clientId += String(random(0xffff), HEX);

    if (MQTT_CLIENT.connect(clientId.c_str())) {
      Serial.println("MQTT connected");
      
      // Once connected, publish an announcement...
      publish(PREV_RESULT);
      
      // ... and resubscribe
      MQTT_CLIENT.subscribe(MQTT_SUBSCRIBE);
    }
    else {
      Serial.print(".");

      // Wait some time to space out connection requests
      delay(3000);
    }
  }
}

// This function handling input topic and returns last result
void callback(char* topic, byte* payload, unsigned int length) {
  // Previous status was requested 
  Serial.println("Previous status was requested ");

  // Publish a result to a topic
  publish(PREV_RESULT);
}

// This function runs over and over again in a continuous loop
void loop() {
  // Check if we're connected to the MQTT broker
  if (!MQTT_CLIENT.connected()) {
    // If we're not, attempt to reconnect
    reconnect();
  }

  // Listenning topic
  MQTT_CLIENT.loop();

  bool result = false;
  long light = digitalRead(PIN_LIGHT);

  // When light is on, then check motion
  if (light == LOW) {
    long motion = digitalRead(PIN_MOTION);

    // If motion detected, then send light "ON" status
    if (motion == HIGH) {
      result = true;
    }
  }

  // Send changed status only
  if (result != PREV_RESULT) {
    // Previous status was requested 
    Serial.println("Status was changed");
    
    // Publish a result to a topic
    publish(result);

    // Save previous result
    PREV_RESULT = result;
  }

  // Wait five seconds
  delay(1000);
}

// This function sends data to the MQTT broker
void publish(bool status) {
  const int capacity = JSON_OBJECT_SIZE(2);
  StaticJsonBuffer<capacity> jb;
  JsonObject& jsonObj = jb.createObject();
  
  jsonObj["hash"] = random(0xffff);
  jsonObj["sensor"] = status;

  char messageBuffer[100];
  jsonObj.printTo(messageBuffer, sizeof(messageBuffer));

  // Publish a result to a topic
  MQTT_CLIENT.publish(MQTT_PUBLISH, messageBuffer);
}
