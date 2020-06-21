/* 
    AzureTableStorageService.h - Library for interacting with Azure Functions that interact with Azure Table Storage.
    Created by Bert Wagner, 2020-06-03.
    Released under MIT license.
*/
#ifndef AzureTableStorageService_h
#define AzureTableStorageService_h
#include "Arduino.h"
#include <WiFiNINA.h>

class AzureTableStorageService
{
    public:
        AzureTableStorageService(char server[]);
        void setClient(WiFiSSLClient client);
        void write(char data[]);
    private:
        char _server[100];
        WiFiSSLClient _client;
};

#endif