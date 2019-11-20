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
            var uri = new Uri("https://psdb.documents.azure.com:443/");
            var key = "8pkIvNr358TuqJbdDNN9T1sj8meQuPH5RAEdjc0GeZbuCnkaFTD3uw7PEIJHSzaBL9jjqa0vKT3yTsFjvUEMKA==";
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
