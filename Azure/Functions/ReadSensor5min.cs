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
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                     
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());


            // Create or reference an existing table
            CloudTable table = tableClient.GetTableReference("SensorReading");

            //TableOperation retrieve = TableOperation.Retrieve<dynamic>(partitionKey, rowKey);

            //TableQuery<SensorReading> query = new TableQuery<SensorReading>();
                    //.Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, "LightSensor_1__Resolution_300__Date_2020-06-20__Hour_21"))
                    //.Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.LessThan, "LightSensor_1__Resolution_60"));

            // Create a filter condition where the partition key is "Smith".
            String partitionFilter = TableQuery.GenerateFilterCondition(
                "PartitionKey",
                QueryComparisons.GreaterThanOrEqual,
                "LightSensor_1__Resolution_300__Date_2020-06-20__Hour_21");

            // Create a filter condition where the row key is less than the letter "E".
            String rowFilter = TableQuery.GenerateFilterCondition(
                "PartitionKey",
                QueryComparisons.LessThan,
                "LightSensor_1__Resolution_60");

            // Combine the two conditions into a filter expression.
            String combinedFilter = TableQuery.CombineFilters(partitionFilter,
                TableOperators.And, rowFilter);

            // Specify a range query, using "Smith" as the partition key,
            // with the row key being up to the letter "E".
            TableQuery<SensorReading> query = new TableQuery<SensorReading>()
                    .Where(combinedFilter);
                       

            int messageCounter = 0;
            foreach (SensorReading message in table.ExecuteQuery(query))
            {
                Console.WriteLine(message.PartitionKey);
                messageCounter++;
            }

            string ts="";

            /*TableOperation retrieveOperation = TableOperation.Retrieve<SensorReading>("LightSensor__2020-05-24__5min", "Hour_00");
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            SensorReading sensorData = result.Result as SensorReading;
            


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
