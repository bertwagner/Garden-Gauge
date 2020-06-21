/* 
    WifiService.h - Library for connecting to wifi.
    Created by Bert Wagner, 2020-05-31.
    Released under MIT license.
*/
#ifndef WifiService_h
#define WifiService_h
#include "Arduino.h"
#include <WiFiNINA.h>

class WifiService
{
    public:
        WifiService(char ssid[], char password[]);
        void connect();
        WiFiSSLClient getClient();
        long status();
    private:
        char _ssid[50];
        char _password[50];
        int _wifiStatus;
        WiFiSSLClient _client;
};

#endif