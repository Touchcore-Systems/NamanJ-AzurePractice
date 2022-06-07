using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace LearningAzure
{
    public static class UploadFile
    {
        [FunctionName("UploadFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            Stream myBlob;
            IFormFile file;
            BlobContainerClient blobContainerClient;
            BlobClient blobClient;

            string conn = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");

            using (myBlob = new MemoryStream())
            {
                try
                {
                    file = req.Form.Files["File"];
                    myBlob = file.OpenReadStream();

                    blobContainerClient = new BlobContainerClient(conn, containerName);
                    blobClient = blobContainerClient.GetBlobClient(file.FileName);

                    await blobClient.UploadAsync(myBlob);
                    return new OkObjectResult("File uploaded.");
                }
                catch (Exception ex)
                {
                    log.LogError(ex.ToString());
                    return new BadRequestObjectResult(ex.Message);
                }
            }
        }
    }
}
