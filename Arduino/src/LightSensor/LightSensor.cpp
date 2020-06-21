/* 
    LightSensor.cpp - Library for reading in light levels.
    Created by Bert Wagner, 2020-05-23.
    Released under MIT license.
*/
#include "Arduino.h"
#include "LightSensor.h"

LightSensor::LightSensor(int pin)
{
  _pin = pin;
}

int LightSensor::read()
{
  return analogRead(_pin);
}