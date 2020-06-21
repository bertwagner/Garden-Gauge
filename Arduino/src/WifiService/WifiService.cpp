/* 
    WifiService.cpp - Library for connecting to wifi.
    Created by Bert Wagner, 2020-05-31.
    Released under MIT license.
*/
#include "Arduino.h"
#include "WifiService.h"
#include <WiFiNINA.h>

WifiService::WifiService(char ssid[], char password[])
{
  strncpy(_ssid, ssid, 50);
  strncpy(_password, password, 50);
  _wifiStatus = WL_IDLE_STATUS;
}

void WifiService::connect()
{
  Serial.println("starting to connect to wifi");
  // check for the WiFi module:
  if (WiFi.status() == WL_NO_MODULE) {
    Serial.println("Communication with WiFi module failed!");
    // don't continue
    while (true);
  }
  

  String fv = WiFi.firmwareVersion();
  if (fv < WIFI_FIRMWARE_LATEST_VERSION) {
    Serial.println("Please upgrade the firmware");
  }

  // attempt to connect to WiFi network:
  while (_wifiStatus != WL_CONNECTED) {
    Serial.print("Attempting to connect to SSID: ");
    Serial.println(_ssid);
    // Connect to WPA/WPA2 network. Change this line if using open or WEP network:
    _wifiStatus = WiFi.begin(_ssid, _password);

    // wait .5 seconds for connection:
    delay(2000);
  }
}

WiFiSSLClient WifiService::getClient()
{
  Serial.println("inside get client");
  return _client;
}

long WifiService::status()
{
    Serial.println("Wifi Status:");

    Serial.print("SSID: ");
    Serial.println(WiFi.SSID());

    IPAddress ip = WiFi.localIP();
    Serial.print("IP Address: ");
    Serial.println(ip);

    long rssi = WiFi.RSSI();
    Serial.print("Signal strength (RSSI):");
    Serial.print(rssi);
    Serial.println(" dBm");
    Serial.println("");

    return rssi;
}