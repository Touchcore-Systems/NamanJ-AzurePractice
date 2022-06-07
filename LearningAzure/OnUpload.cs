using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace LearningAzure
{
    public class OnUpload
    {
        [FunctionName("OnUpload")]
        public void Run([BlobTrigger("file-upload/{name}", Connection = "AzureWebJobsStorage")]
            Stream myBlob, BlobProperties properties, string name, ILogger log)
        {
            // to read file
            StreamReader reader;
            string content;

            // for updating meta data
            SqlConnection myConn;
            SqlCommand cmd;

            // to read file
            using (reader = new StreamReader(myBlob))
            {
                content = reader.ReadToEnd();
                log.LogInformation(content);
            }

            // for updating meta data
            string sqlDataSource = Environment.GetEnvironmentVariable("MySqlConnectionString");

            using (myConn = new SqlConnection(sqlDataSource))
            {
                using (cmd = new SqlCommand("InsertDetails", myConn))
                {
                    try
                    {
                        myConn.Open();

                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Size", myBlob.Length);
                        cmd.Parameters.AddWithValue("@FileType", properties.ContentType);

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                        log.LogInformation("Details updated.");
                    }
                    catch (Exception ex)
                    {
                        log.LogError(ex.ToString());
                    }
                    finally
                    {
                        myConn.Close();
                    }
                }

            }
        }
    }
}
