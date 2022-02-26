using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace WatermarkProcessFunction
{
    public static class Function1
    {
        [Function("Function1")]
        public static void Run([QueueTrigger("watermarkqueue", Connection = "")] string myQueueItem,
            FunctionContext context)
        {
            var logger = context.GetLogger("Function1");
            logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
