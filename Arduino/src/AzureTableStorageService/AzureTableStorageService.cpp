/* 
    AzureTableStorageService.h - Library for interacting with Azure Functions that interact with Azure Table Storage.
    Created by Bert Wagner, 2020-06-03.
    Released under MIT license.
*/
#include "secrets.h"
#include "Arduino.h"
#include "AzureTableStorageService.h"
#include <WiFiNINA.h>

//int counter =0;

AzureTableStorageService::AzureTableStorageService(char server[])
{
  strncpy(_server, server, 100);
  // move this into its own method and run 
  //_wifiClient.connect(_server, 443);
}

void AzureTableStorageService::setClient(WiFiSSLClient client)
{
  _client = client;
}

void AzureTableStorageService::write(char data[])
{
  Serial.println("BEGIN writing to table storage");
  if (!_client.connected()){
    Serial.println("Wifi not connected.  Connecting now.");
    _client.connect(_server, 443);
    Serial.println("Wifi now connected!");
  } 


  Serial.println("Wifi is connected.  Writing to Azure.");
  Serial.println(data);
  _client.print("POST /api/WriteSensor?code=");
  _client.print(API_SECRET);
  _client.println(" HTTP/1.1");
  _client.println("Host: gardengauge.azurewebsites.net");
  _client.println("Connection-Type: application/json");
  _client.print("Content-Length: ");
  _client.println(strlen(data));
  _client.println("");
  _client.println(data);
  //_client.println("Connection: close");
  _client.println();

   // if there are incoming bytes available
  // from the server, read them and print them
  // Need to read incoming data because otherwise _client.connected() method 
  // will show the device still connected even if it died.
  while (_client.available()) {
    char c = _client.read();
    Serial.write(c);
  }

  
}

