using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;

namespace GardenGauge.Function
{
    public static class WriteSensor
    {
        private static string connectionString = System.Environment.GetEnvironmentVariable("GardenGaugeTableStorageConnectionString");

        public class ArduinoData
        {
            public List<SensorData> SensorData {get;set;}
        }
        public class SensorData
        {
            public int Id {get;set;}
            public string Name {get;set;}
            public string Units {get;set;}
            public string DataType {get;set;}
            public string Value {get;set;}
        }
        [FunctionName("WriteSensor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ArduinoData inputData = JsonConvert.DeserializeObject<ArduinoData>(requestBody);

            DateTime logDate = DateTime.UtcNow;
            List<string> outputMessages = new List<string>();

            foreach (SensorData sensor in inputData.SensorData)
            {
                // Partion Key: LightSensor_1__Resolution_1__Date_2020-05-20__Hour_00
                string partitionKey = String.Format("{0}_{1}__{2}_{3}__{4}_{5}__{6}_{7}", sensor.Name.Replace(" ",""), sensor.Id,"Resolution",60,"Date", logDate.ToString("yyyy-MM-dd"), "Hour", logDate.Hour.ToString().PadLeft(2,'0'));
                
                // Row Key: Minute_00__Second_01
                string rowKey = String.Format("Minute_{0}__Second_{1}",logDate.Minute.ToString().PadLeft(2,'0'), logDate.Second.ToString().PadLeft(2,'0'));

                SensorReading newRecord = new SensorReading();
                newRecord.PartitionKey = partitionKey;
                newRecord.RowKey = rowKey;
                newRecord.Units = sensor.Units;
                newRecord.DataType = sensor.DataType;
                newRecord.Value = sensor.Value;


                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
                CloudTable table = tableClient.GetTableReference("SensorReading");
                
                TableOperation insertOrMergeOperation = TableOperation.InsertOrReplace(newRecord);
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                SensorReading insertedSensorDetail = result.Result as SensorReading;
                string responseMessage = "Values inserted:" + JsonConvert.SerializeObject(insertedSensorDetail);
                outputMessages.Add(responseMessage);

                // Log the 5 minute entry separately
                if (logDate.Minute % 5 == 0)
                {
                    // Partion Key: LightSensor_1__Resolution_300__Date_2020-05-20__Hour_00
                    partitionKey = String.Format("{0}_{1}__{2}_{3}__{4}_{5}__{6}_{7}", sensor.Name.Replace(" ",""), sensor.Id,"Resolution",300,"Date", logDate.ToString("yyyy-MM-dd"), "Hour", logDate.Hour.ToString().PadLeft(2,'0'));
                    
                    // Row Key: Minute_00
                    rowKey = String.Format("Minute_{0}",logDate.Minute.ToString().PadLeft(2,'0'));

                    newRecord.PartitionKey = partitionKey;
                    newRecord.RowKey = rowKey;

                    result = await table.ExecuteAsync(insertOrMergeOperation);
                    insertedSensorDetail = result.Result as SensorReading;
                    responseMessage = "Values inserted:" + JsonConvert.SerializeObject(insertedSensorDetail);
                    outputMessages.Add(responseMessage);
                }
            }

       
            return new OkObjectResult(null);
        }
    }
}
