using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace GardenGauge.Function
{
    public class SensorReading : TableEntity
    {
        public string Units {get;set;}
        public string DataType {get;set;}
        public string Value {get;set;}
    }
}