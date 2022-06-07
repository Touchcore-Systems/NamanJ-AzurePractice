using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace LearningAzure
{
    public class RecieveMessage
    {
        [FunctionName("RecieveMessage")]
        public void Run([ServiceBusTrigger("new-queue", Connection = "AzureWebJobsServiceBus")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"Message: {myQueueItem}");
        }
    }
}
