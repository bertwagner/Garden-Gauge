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
    public static class ReadSensor5min
    {
        private static string connectionString = System.Environment.GetEnvironmentVariable("GardenGaugeTableStorageConnectionString");

        [FunctionName("ReadSensor5min")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Documentation - these java examples are more helpful than the .NET examples: https://docs.microsoft.com/en-us/azure/cosmos-db/table-storage-how-to-use-java
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference("SensorReading");

            // Default parameter values
            string sensorName = "LightSensor";
            string sensorId = "1";
            string startDate = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-dd");
            string startHour = DateTime.UtcNow.Hour.ToString().PadLeft(2,'0');
            string stopDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            string stopHour = startHour;
            int maxResultCount = 12*24*3; // 3 days of data

            // Passed in parameter values
            sensorName = req.Query["sensor_name"].ToString() != "" ? req.Query["sensor_name"].ToString() : sensorName;
            sensorId = req.Query["sensor_id"].ToString() != "" ? req.Query["sensor_id"].ToString() : sensorId;
            startDate = req.Query["start_date"].ToString() != "" ?  req.Query["start_date"].ToString() : startDate;
            startHour = req.Query["start_hour"].ToString() != "" ? req.Query["start_hour"].ToString().PadLeft(2,'0') : startHour;
            stopDate = req.Query["stop_date"].ToString() != "" ? req.Query["stop_date"].ToString() : stopDate;
            stopHour = req.Query["stop_hour"].ToString() != "" ? req.Query["stop_hour"].ToString().PadLeft(2,'0') : stopHour;

            String filter1 = TableQuery.GenerateFilterCondition(
                "PartitionKey",
                QueryComparisons.GreaterThanOrEqual,
                String.Format("{0}_{1}__Resolution_300__Date_{2}__Hour_{3}",sensorName,sensorId,startDate,startHour));

            String filter2 = TableQuery.GenerateFilterCondition(
                "PartitionKey",
                QueryComparisons.LessThan,
                String.Format("{0}_{1}__Resolution_300__Date_{2}__Hour_{3}",sensorName,sensorId,stopDate,stopHour));

            String combinedFilter = TableQuery.CombineFilters(filter1,
                TableOperators.And, filter2);

            
            TableQuery<SensorReading> query = new TableQuery<SensorReading>()
                    .Where(combinedFilter)
                    //.Select(new List<string>{"Value","Units","DataType","Timestamp"})
                    .Take(maxResultCount);

            IEnumerable<SensorReading> results = table.ExecuteQuery<SensorReading>(query);

            return new OkObjectResult(JsonConvert.SerializeObject(results));
        }
    }
}
