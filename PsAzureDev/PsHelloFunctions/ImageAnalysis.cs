using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.WindowsAzure.Storage.Blob;

namespace PsHelloFunctions
{

    public static class ImageAnalysis
    {

        public const string ENDPOINT = "https://psfaceapijobin.cognitiveservices.azure.com/face/v1.0";
        public const string SUBSCRIPTION_KEY = "64cebcb491f0443fb49817dcc2978851";


        [FunctionName("ImageAnalysis")]
        public static async Task Run(
            [BlobTrigger("images/{name}", Connection = "psazure1")]
            CloudBlockBlob blob, 
            string name, 
            TraceWriter log,
            [CosmosDB("pshelloazure", "images", ConnectionStringSetting ="psdb")]
            IAsyncCollector<FaceAnalysisResults> result)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{blob.Name} \n Size: {blob.Properties.Length} Bytes");

            var sas = GetSas(blob);
            var url = blob.Uri + sas;

            log.Info($"Blob url is {url}");

            var faces = await GetAnalysisAsync(url);
            await result.AddAsync(new FaceAnalysisResults { Faces = faces, ImageId = blob.Name });
        }

        public static async Task<Face[]> GetAnalysisAsync(string url)
        {
            //https://australiaeast.dev.cognitive.microsoft.com/docs/services/563879b61984550e40cbbe8d/operations/563879b61984550f3039523a/console

            var client = new FaceServiceClient(SUBSCRIPTION_KEY, ENDPOINT);
            var types = new[] { FaceAttributeType.Emotion };
            var result = await client.DetectAsync(url, false, false, types);
            return result;
        }

        public static string GetSas(CloudBlockBlob blob)
        {
            var sasPolicy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(15)
            };
            var sas = blob.GetSharedAccessSignature(sasPolicy);
            return sas;
        }
    }
}
