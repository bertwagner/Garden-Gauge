#include <ArduinoJson.h>

/* 
    Garden Gauge - a monitor for your garden.
    Created by Bert Wagner, 2020-05-31.
    Released under MIT license.
*/

#include "secrets.h"
#include <SPI.h>
#include "src/LightSensor/LightSensor.h"
#include "src/WifiService/WifiService.h"
#include "src/AzureTableStorageService/AzureTableStorageService.h"
#include "ArduinoJson.h"
#include "src/Arduino_LSM6DS3/Arduino_LSM6DS3.h"

LightSensor lightSensor(A0);
WifiService wifi(NETWORK_SSID,NETWORK_PASSWORD);
WiFiSSLClient client;
AzureTableStorageService tableStorage("gardengauge.azurewebsites.net");

void setup() {
  //Initialize serial and wait for port to open:
  /*Serial.begin(9600);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
  }*/

  wifi.connect();
  tableStorage.setClient(wifi.getClient());
  if (!IMU.begin()) {
    while (1);
  }
  
  
}

void loop() {

  Serial.println("===================================");
  Serial.println("");

  long wifi_signal_strength = wifi.status();

  int level = lightSensor.read();
  Serial.print("Light level:");
  Serial.println(level);  
  Serial.println("");

  float onboard_temperature_c;
  float onboard_temperature_f;
  if (IMU.temperatureAvailable()) {
    // after IMU.readTemperature() returns, t will contain the temperature reading
    IMU.readTemperature(onboard_temperature_c);
    onboard_temperature_f = (onboard_temperature_c * 1.8) + 32;

    Serial.print("Onboard temperature: ");
    Serial.print(onboard_temperature_f);
    Serial.println("°F");
    Serial.println("");
  }

  StaticJsonDocument<500> doc;

  JsonArray sensors = doc.createNestedArray("SensorData");

  JsonObject sensors_0 = sensors.createNestedObject();
  sensors_0["Id"] = 1;
  sensors_0["Name"] = "Light Sensor";
  sensors_0["Units"] = "Lumensomethings";
  sensors_0["DataType"] = "integer";
  sensors_0["Value"] = level;

  JsonObject sensors_1 = sensors.createNestedObject();
  sensors_1["Id"] = 2;
  sensors_1["Name"] = "Onboard Arduino Temperature";
  sensors_1["Units"] = "Fahrenheit";
  sensors_1["DataType"] = "float";
  sensors_1["Value"] = onboard_temperature_f;

  JsonObject sensors_2 = sensors.createNestedObject();
  sensors_2["Id"] = 3;
  sensors_2["Name"] = "Wifi Signal Strength";
  sensors_2["Units"] = "RSSI";
  sensors_2["DataType"] = "long";
  sensors_2["Value"] = wifi_signal_strength;
  
  char Serial[500];
  serializeJson(doc, Serial);

  tableStorage.write(Serial);
  delay(1000*60);
}


