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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Documentation - these java examples are more helpful than the .NET examples: https://docs.microsoft.com/en-us/azure/cosmos-db/table-storage-how-to-use-java
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference("SensorReading");

            String filter1 = TableQuery.GenerateFilterCondition(
                "PartitionKey",
                QueryComparisons.GreaterThanOrEqual,
                "LightSensor_1__Resolution_300__Date_2020-06-20__Hour_21");

            String filter2 = TableQuery.GenerateFilterCondition(
                "PartitionKey",
                QueryComparisons.LessThan,
                "LightSensor_1__Resolution_60");

            String combinedFilter = TableQuery.CombineFilters(filter1,
                TableOperators.And, filter2);

            TableQuery<SensorReading> query = new TableQuery<SensorReading>()
                    .Where(combinedFilter);
                       

            int messageCounter = 0;
            foreach (SensorReading message in table.ExecuteQuery(query))
            {
                Console.WriteLine(message.PartitionKey);
                messageCounter++;
            }

            string ts="";

            /*
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = $"Sensor details: {JsonConvert.SerializeObject(sensorData.SensorDetail)}";
*/
            return new OkObjectResult("");
        }
    }
}
