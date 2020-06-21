/* 
    LightSensor.h - Library for reading in light levels.
    Created by Bert Wagner, 2020-05-23.
    Released under MIT license.
*/
#ifndef LightSensor_h
#define LightSensor_h
#include "Arduino.h"

class LightSensor
{
    public:
        LightSensor(int pin);
        int read();
    private:
        int _pin;
};

#endif