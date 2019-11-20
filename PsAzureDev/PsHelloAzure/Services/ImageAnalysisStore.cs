using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;

namespace PsHelloAzure.Services
{
    public class ImageAnalysisStore
    {
        private DocumentClient client;
        private Uri imageAnalysisLink;

        public ImageAnalysisStore()
        {
            var uri = new Uri("https://psdbjobin.documents.azure.com:443/");
            var key = "WZgq3AyH9H9VYxleldhbHFU1O0hOtorZMTE360pJib5C7R15RFkks8PNOI1mj2xsyGOYg0hz8RyOZUAbLNhI6g==";
            client = new DocumentClient(uri, key);
            imageAnalysisLink = UriFactory.CreateDocumentCollectionUri("pshelloazure", "images");
        }

        public dynamic GetImageAnalysis(string imageId)
        {
            var spec = new SqlQuerySpec();
            spec.QueryText = "SELECT * FROM c WHERE (c.ImageId = @imageid)";
            spec.Parameters.Add(new SqlParameter("@imageid", imageId));
            var result = client.CreateDocumentQuery(imageAnalysisLink, spec).ToList();
            return result; 
        }
    }
}
